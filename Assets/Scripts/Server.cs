using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
public class Server : MonoBehaviour
{
    private byte reliableChannel;
    private int hostID;
    private int webHostID;

    private bool isStarted;
    private byte error;

    private const int MAX_USER = 2;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024;

    private Dictionary<int,Lobby> Lobbies;
    private int nextAvailibleLobby;
    #region Monobehaviour
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Init();
    }
    private void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    public void Init()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        //Server Only
        hostID = NetworkTransport.AddHost(topo, PORT, null);
        webHostID = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);

        Debug.Log($"Opening connection on port {PORT} and webport {WEB_PORT}");
        Lobbies = new Dictionary<int, Lobby>();
        isStarted = true;
    }

    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    private void UpdateMessagePump()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionID;
        int channelId;

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type =  NetworkTransport.Receive(out recHostId, 
            out connectionID, out channelId, recBuffer, BYTE_SIZE, 
            out dataSize, out error);

        switch (type)
        {
            case NetworkEventType.DataEvent:
                //Unpacks the binary serialization
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                OnData(connectionID, channelId, recHostId, msg);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.LogError($"User {connectionID} has connected through host {recHostId}");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.LogError($"User {connectionID} has disconnected");
                break;
            case NetworkEventType.Nothing:
                break;
            default:
            case NetworkEventType.BroadcastEvent:
                Debug.LogError($"Unexpected network event type");
                break;
            
        }
    }

    #region on data
    private void OnData(int cnnID, int channelID, int recHostID, NetMsg msg)
    {
        Debug.Log("Recieved a mesage of type " + msg.OP);
        switch(msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;

            case NetOP.CreateAccount:
                CreateAccount(cnnID, channelID, recHostID, (Net_CreateAccount)msg);
                break;

            case NetOP.LoginRequest:
                LoginRequest(cnnID, channelID, recHostID, (Net_LoginRequest)msg);
                break;

            case NetOP.SendTurn:
                SendTurn(cnnID, channelID, recHostID, (Net_SendTurn)msg);
                break;

            case NetOP.CreateLobbyRequest:
                CreateLobby(cnnID, channelID, recHostID, (Net_CreateLobbyRequest)msg);
                break;

            case NetOP.JoinLobby:
                JoinLobby(cnnID, channelID, recHostID, (Net_JoinLobby)msg);
                break;
        }
    }

    private void CreateAccount(int cnnID, int channelID, int recHostID, Net_CreateAccount ca)
    {
        Debug.Log($"Create account {ca.Username} {ca.Password} {ca.Email}");

        Net_OnCreateAccount oca = new Net_OnCreateAccount();
        oca.Success = 0;
        oca.Information = "Account was created!";

        SendClient(recHostID, cnnID, oca);
    }

    private void LoginRequest(int cnnID, int channelID, int recHostID, Net_LoginRequest ca)
    {
        Debug.Log($"Logging in user {ca.UsernameOrEmail} with token {ca.Password}");

        Net_OnLoginRequest olr = new Net_OnLoginRequest();
        olr.Success = 0;
        olr.Information = "Success!";
        olr.Discriminator = "0000";
        olr.Token = "TOKEN";

        SendClient(recHostID, cnnID, olr);
    }

    private void SendTurn(int cnnID, int channelID, int recHostID, Net_SendTurn st)
    {
        Debug.LogError($"Recieved turn string {st.TurnString} for player {st.playerID} in lobby {st.lobbyID}");
        Net_OnSendTurn ost = new Net_OnSendTurn();
        ost.lobbyID = st.lobbyID;
        ost.playerID = st.playerID;
        ost.TurnString = st.TurnString;
        int player1cnnID;
        int player2cnnID;
        if (Lobbies.TryGetValue(st.lobbyID, out Lobby L))
        {
            player1cnnID = L.player1cnnID;
            player2cnnID = L.player2cnnID;
            if(st.playerID == 1)
            {
                L.player1Turn = st.TurnString;
            }
            else if(st.playerID == 2)
            {
                L.player2Turn = st.TurnString;
            }
            else
            {
                Debug.LogError("Invalid player ID passed to SendTurn()");
            }
            SendClient(recHostID, player1cnnID, ost);
            SendClient(recHostID, player2cnnID, ost);
            //Maybe only need to send to board host?
            SendClient(recHostID, L.boardHostcnnID, ost);
        }
        else
        {
            Debug.LogError($"No lobby exists for key {st.lobbyID}");
        }
        

    }

    private void CreateLobby(int cnnID, int channelID, int recHostID, Net_CreateLobbyRequest clr)
    {
        Debug.LogError($"Recieved request to create a lobby");
        Net_OnCreateLobby st = new Net_OnCreateLobby();
        st.assignedLobbyID = AddLobby(cnnID);
        SendClient(recHostID, cnnID, st);
    }

    private int AddLobby(int cnnID) //creates a lobby and adds the requesting client as the boardHost
    {
        int lobbyID = nextAvailibleLobby;
        nextAvailibleLobby++;
        Lobby L = new Lobby();
        L.boardHostcnnID = cnnID;
        Lobbies.Add(lobbyID,L);
        if(nextAvailibleLobby > 32000) { nextAvailibleLobby = 0; }
        Debug.LogError($"Created lobby with key {lobbyID}");
        return lobbyID;
    }

    private void JoinLobby(int cnnID, int channelID, int recHostID, Net_JoinLobby jl)
    {
        Debug.LogError($"Recieved request to join lobby {jl.lobbyID} from connection ID {cnnID}");
        int result = TryJoinLobby(cnnID, jl.lobbyID);
        Net_OnJoinLobby ojl = new Net_OnJoinLobby();
        ojl.assignedPlayerID = result;
        if(result != 0){ojl.succeeded = true;}
        else { ojl.succeeded = false; }
        Debug.LogError($"Processed request to join lobby {jl.lobbyID} from connection ID {cnnID}. Player result is {ojl.succeeded} {result}");
        SendClient(recHostID, cnnID, ojl);
    }

    private int TryJoinLobby(int cnnID, int lobbyTarget)
    {
        if (Lobbies.TryGetValue(lobbyTarget, out Lobby L))
        {
            int result = L.AddPlayer(cnnID);
            if (result != 0)
            {
                return result;
            }
            else
            {
                return 0;
            }
            
        }
        else
        {
            return 0;
        }
    }
    #endregion

    #region Send
    public void SendClient(int recHost, int cnnID, NetMsg msg)
    {
        //where we hold data
        byte[] buffer = new byte[BYTE_SIZE];

        //where you crush data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        if(recHost == 0)
        {
            NetworkTransport.Send(hostID, cnnID, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(webHostID, cnnID, reliableChannel, buffer, BYTE_SIZE, out error);
        }
        
    }
    #endregion
}

public class Lobby
{
    public int lobbyID { get; set; }
    public int boardHostcnnID { get; set; } = -1;
    public int player1cnnID { get; set; } = -1;
    public int player2cnnID { get; set; } = -1;
    public string player1Turn { get; set; }
    public string player2Turn { get; set; }

    private bool HasRoom()
    {
        if(player1cnnID !=-1 && player2cnnID != -1)
        {
            return false;
        }
        else { return true; }
    }

    //returns the player index if it can be added
    public int AddPlayer(int cnnID)
    {
        if (HasRoom())
        {
            if(player1cnnID == -1)
            {
                player1cnnID = cnnID;
                return 1;
            }
            else
            {
                player2cnnID = cnnID;
                return 2;
            }
            
        }
        else
        {
            return 0;
        }
    }
}
