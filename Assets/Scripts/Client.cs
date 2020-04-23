using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
public class Client : MonoBehaviour
{
    public bool isBoardHost;
    public bool isRecievingInput = false;
    public int lobbyID = -1;
    public int myPlayerID = -1;
    private int tempLobbyID;
    private string p1Turn;
    private string p2Turn;
    public static Client instance { get; set; }

    private byte reliableChannel;
    private int hostID;
    private byte error;
    private bool isStarted;
    private int connectionID;

    private const int MAX_USER = 2;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
#if UNITY_EDITOR
    private const string SERVER_IP = "127.0.0.1";
#else
    private const string SERVER_IP = "76.176.70.18";
#endif
    private const int BYTE_SIZE = 1024;

    #region Monobehaviour
    private void Start()
    {
        instance = this;
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

        //Client Only
        hostID = NetworkTransport.AddHost(topo, 0);
        Debug.Log($"hostID = {hostID}");

#if UNITY_WEBGL && !UNITY_EDITOR
        //web client
        connectionID = NetworkTransport.Connect(hostID, SERVER_IP, WEB_PORT, 0, out error);
        Debug.Log("Connecting from web");
#else
        //standalone client
        connectionID = NetworkTransport.Connect(hostID, SERVER_IP, PORT, 0, out error);
        Debug.Log("Connecting from standalone");
#endif
        Debug.Log($"Attempting to connect to {SERVER_IP}...");
        isStarted = true;
        if (connectionID != 0)
        {
            LobbyScene.instance.ChangeAuthenticationMessage($"Connecting...");
        }
        else
        {
            LobbyScene.instance.ChangeAuthenticationMessage("Failed to connect to the internet.");
        }
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

        NetworkEventType type = NetworkTransport.Receive(out recHostId,
            out connectionID, out channelId, recBuffer, BYTE_SIZE,
            out dataSize, out error);

        switch (type)
        {
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                OnData(connectionID, channelId, recHostId, msg);

                break;
            case NetworkEventType.ConnectEvent:
                Debug.LogError($"Connected to the server!");
                LobbyScene.instance.ChangeAuthenticationMessage($"Connected to the gameserver!");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.LogError($"Disconnected from server");
                LobbyScene.instance.ChangeAuthenticationMessage($"Disconnected from the gameserver.");
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
        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.OnCreateAccount:
                OnCreateAccount((Net_OnCreateAccount)msg);
                break;
            case NetOP.OnLoginRequest:
                OnLoginRequest((Net_OnLoginRequest)msg);
                break;
            case NetOP.OnSendTurn:
                OnSendTurn((Net_OnSendTurn)msg);
                break;
            case NetOP.OnCreateLobby:
                OnCreateLobby((Net_OnCreateLobby)msg);
                break;
            case NetOP.OnJoinLobby:
                OnJoinLobby((Net_OnJoinLobby)msg);
                break;
        }
    }

    private void OnCreateAccount(Net_OnCreateAccount oca)
    {
        LobbyScene.instance.EnableInputs();
        LobbyScene.instance.ChangeAuthenticationMessage(oca.Information);
    }

    private void OnLoginRequest(Net_OnLoginRequest olr)
    {
        LobbyScene.instance.ChangeAuthenticationMessage(olr.Information);
        if (olr.Success != 0)
        {
            //can't login
            LobbyScene.instance.EnableInputs();
        }
        else
        {
            //succeed login
        }
    }

    private void OnSendTurn(Net_OnSendTurn ost)
    {

        if (isBoardHost && isRecievingInput)
        {
            //TODO log player turns
            if (ost.playerID == 1)
            {
                p1Turn = ost.TurnString;
                LobbyScene.instance.ProcessPlayerInput(p1Turn, 1);
            }
            else
            {
                p2Turn = ost.TurnString;
                LobbyScene.instance.ProcessPlayerInput(p2Turn, 2);
            }
            if (p1Turn != "" && p2Turn != "")
            {
                //LobbyScene.instance.ExecuteTurn();
                isRecievingInput = false;
            }
            LobbyScene.instance.UpdateTurnDisplay(ost.TurnString, ost.playerID);
        }
        else if (!isRecievingInput && isBoardHost)
        {
            Debug.LogError("Not recieving input at this moment");
        }

    }
    private void OnCreateLobby(Net_OnCreateLobby ocl)
    {
        isBoardHost = true;
        LobbyScene.instance.EnableInputs();
        LobbyScene.instance.UpdateLobbyDisplay(ocl.assignedLobbyID);
        lobbyID = ocl.assignedLobbyID;
    }

    private void OnJoinLobby(Net_OnJoinLobby ojl)
    {
        if (ojl.succeeded)
        {
            if (!isBoardHost)
            {
                LobbyScene.instance.EnableInputs();
                LobbyScene.instance.UpdatePlayerIDs(ojl.assignedPlayerID);
                lobbyID = tempLobbyID;//if the ojl succeeds, the lobby is set to the one specified in the og send
                myPlayerID = ojl.assignedPlayerID;
                Debug.LogError($"Player {ojl.assignedPlayerID} joined lobby {lobbyID}!");
                LobbyScene.instance.UpdateNonBoardCanvas();
                LobbyScene.instance.UpdateLobbyDisplay(lobbyID);
            }
            else
            {
                LobbyScene.instance.UpdateConnectionStatus(ojl.assignedPlayerID);
                Debug.LogError($"Player {ojl.assignedPlayerID} joined lobby {lobbyID}!");
            }
            if (isBoardHost && ojl.assignedPlayerID == 2)
            {
                isRecievingInput = true;
            }
        }
        else
        {
            LobbyScene.instance.EnableInputs();
            Debug.LogError($"Player {ojl.assignedPlayerID} failed to join lobby {tempLobbyID}.");
        }


    }

    #endregion

    #region Send
    public void SendServer(NetMsg msg)
    {
        //where we hold data
        byte[] buffer = new byte[BYTE_SIZE];

        //where you crush data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, BYTE_SIZE, out error);
    }

    public void SendCreateAccount(string username, string password, string email)
    {
        Net_CreateAccount ca = new Net_CreateAccount();

        ca.Username = username;
        ca.Password = password;
        ca.Email = email;

        SendServer(ca);
    }
    public void SendLoginRequest(string usernameOrEmail, string password)
    {
        Net_LoginRequest ca = new Net_LoginRequest();

        ca.UsernameOrEmail = usernameOrEmail;
        ca.Password = password;

        SendServer(ca);
    }

    //actual stuff
    public void SendTurnString(string turnString, int playerID, int lobbyID)
    {
        Net_SendTurn st = new Net_SendTurn();
        st.lobbyID = lobbyID;
        st.TurnString = turnString;
        st.playerID = playerID;

        SendServer(st);
    }

    public void SendCreateLobbyRequest()//also joins the lobby as a board host if it succeeds
    {
        Net_CreateLobbyRequest cl = new Net_CreateLobbyRequest();
        SendServer(cl);
    }

    public void SendJoinLobby(int lobbyID)
    {
        Net_JoinLobby jl = new Net_JoinLobby();
        jl.lobbyID = lobbyID;
        tempLobbyID = lobbyID;
        SendServer(jl);
    }
    #endregion

    public void ThrowBadTurnError(int player)
    {
        if(player == 1)
        {
            p1Turn = "";
            Debug.LogError("Player 1's turn is invalid");
        }
        else
        {
            p2Turn = "";
            Debug.LogError("Player 2's turn is invalid");

        }
    }
    public void ResetTurns()
    {
        p1Turn = "";
        p2Turn = "";
        isRecievingInput = true;
    }
}
