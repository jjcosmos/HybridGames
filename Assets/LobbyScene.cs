using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{

    [Header("Creation input")]
    [SerializeField] TMP_InputField createUsernameInput;
    [SerializeField] TMP_InputField createPasswordInput;
    [SerializeField] TMP_InputField createEmailInput;

    [Header("Login Input")]
    [SerializeField] TMP_InputField LoginUsernameInput;
    [SerializeField] TMP_InputField LoginPasswordInput;
    [SerializeField] Button SubmitLobbyButton;

    [Header("Output")]
    [SerializeField] TextMeshProUGUI WelcomeMessage;
    [SerializeField] TextMeshProUGUI AuthenticationMessage;
    [SerializeField] SessionIDUpdater LobbyIDMessage;

    [Header("Canvas")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] CanvasManager canvasManager;

    [Header("TurnInput")]
    [SerializeField] TMP_InputField p1Input;
    [SerializeField] TMP_InputField p2Input;
    [SerializeField] Button SubmitButton;
    [SerializeField] CanvasGroup ConnectionBlock;

    [Header("TurnOutput")]
    [SerializeField] TextMeshProUGUI p1Out;
    [SerializeField] TextMeshProUGUI p2Out;
    [SerializeField] TextMeshProUGUI p1OutConfirm;
    [SerializeField] TextMeshProUGUI p2OutConfirm;

    [Header("Lobby")]
    [SerializeField] TextMeshProUGUI lobbyDisplay;
    [SerializeField] TMP_InputField lobbyToJoin;

    [Header("Player ID's")]
    [SerializeField] TextMeshProUGUI playerAID;
    [SerializeField] TextMeshProUGUI playerBID;
    [SerializeField] Button playerAButton;
    [SerializeField] Button playerBButton;

    [Header("Player Connections")]
    [SerializeField] Image player1Connected;
    [SerializeField] Image player2Connected;
    [SerializeField] Sprite ConnectedIcon;
    [SerializeField] Sprite DisconnectedIcon;

    [Header("Board Client")]
    [SerializeField] InputManager myInputManager;

    public static LobbyScene instance;
    private bool firstFlag = true;
    private void Awake()
    {
        instance = this;
        if (ConnectionBlock != null)
        {
            ConnectionBlock.alpha = 1;
        }
    }

    public void OnClickCreateAccount()
    {
        DisableInputs();
        string username = createUsernameInput.text;
        string password = createPasswordInput.text;
        string email = createEmailInput.text;

        Client.instance.SendCreateAccount(username, password, email);
    }
    public void OnClickLoginRequest()
    {
        DisableInputs();
        string usernameOrEmail = LoginUsernameInput.text;
        string password = LoginPasswordInput.text;

        Client.instance.SendLoginRequest(usernameOrEmail, password);
    }

    public void OnClickSubmitTurn() //will not be possible without joining a lobby (maintained by UI elements)
    {
        string player1Turn = p1Input.text;
        //string player2Turn = p2Input.text;
        Debug.Log($"Submitting {player1Turn}");

        //Client.instance.SendTurnString(player1Turn, 1, Client.instance.lobbyID); //will be different with different lobbies 
        //Client.instance.SendTurnString(player2Turn, 2, Client.instance.lobbyID); //TODO playerID param should be taken from the client instance
        //TODO should only send 1 per client

        Client.instance.SendTurnString(player1Turn, Client.instance.myPlayerID, Client.instance.lobbyID); //TODO "player1Turn" should be a generic ref to an input field
    }

    public void OnClickRequestLobby()
    {
        DisableInputs();
        Client.instance.SendCreateLobbyRequest();
    }

    public void OnClickGetPlayerID()//also functions as a joinLobby
    {
        SubmitLobbyButton.interactable = false;
        DisableInputs();
        if(int.TryParse(lobbyToJoin.text, out int i))
        {
            Client.instance.SendJoinLobby(i);
        }
        else
        {
            EnableInputs();
            Debug.LogError("Please enter the number of the lobby");
            EnableLobbyButton();
        }
        
    }

    public void EnableLobbyButton()
    {
        SubmitLobbyButton.interactable = true;
    }


    public void ChangeWelcomeMessage(string message)
    {
        WelcomeMessage.text = message;
    }

    public void ChangeAuthenticationMessage(string message)
    {
        AuthenticationMessage.text = message;
    }
    
    public void EnableInputs()
    {
        canvasGroup.interactable = true;
    }

    public void DisableInputs()
    {
        canvasGroup.interactable = false;
    }

    public void UpdatePlayerIDs(int ID)
    {
        
        playerAID.text = "You are player " +  ID.ToString();
            
    }

    public void UpdateLobbyDisplay(int lobbyID)
    {
        if (Client.instance.isBoardHost)
        {
            lobbyDisplay.text = "Lobby ID: " + lobbyID.ToString();
            LobbyIDMessage.OnInitSession("Lobby ID: " + lobbyID.ToString());
            //StopAllCoroutines();
            StartCoroutine(FadeOutCanvas());
        }
        else
        {
            lobbyDisplay.text = "Lobby ID: " + lobbyID.ToString();
            
        }
        
        //also save ID to send with player turns
    }

    private IEnumerator FadeOutCanvas()
    {
        canvasGroup.interactable = false;
        while(canvasGroup.alpha > Mathf.Epsilon)
        {
            canvasGroup.alpha -= .01f;
            yield return new WaitForSeconds(.01f);
        }
        canvasGroup.alpha = 0;
    }

    public void UpdateTurnDisplay(string turn, int player)
    {
        if (!Client.instance.isBoardHost) { return; }
        if (player == 1) 
        {
            //p1Out.text = turn;
            p1OutConfirm.text = "Player 1 Ready!";
        }
        else if (player == 2)
        {
            //p2Out.text = turn;
            p2OutConfirm.text = "Player 2 Ready!";
        }
        else
        {
            Debug.LogError("Bad player recieved");
        }
    }



    public void ResetTurnDisplay()//TODO actually use this
    {
        p1OutConfirm.text = "Waiting on player 1...";
        p2OutConfirm.text = "Waiting on player 2...";
    }

    public void UpdateConnectionStatus(int player)
    {
        if(player == 1)
        {
            player1Connected.sprite = ConnectedIcon;
        }
        else if (player == 2)
        {
            player2Connected.sprite = ConnectedIcon;
        }
        if(player2Connected.sprite == ConnectedIcon && player1Connected.sprite == ConnectedIcon)
        {
            StartCoroutine(FadeOutBlocker());
        }
    }

    private IEnumerator FadeOutBlocker()
    {
        ConnectionBlock.interactable = false;
        ConnectionBlock.blocksRaycasts = false;
        while (ConnectionBlock.alpha > Mathf.Epsilon)
        {
            ConnectionBlock.alpha -= .01f;
            yield return new WaitForSeconds(.01f);
        }
        ConnectionBlock.alpha = 0;
    }

    public void ProcessPlayerInput(string turn, int playerID)
    {
        Debugger.instance.Push($"{playerID} submitting turn {turn}");
        myInputManager.ProcessInput(turn,playerID);
    }

    public void UpdateNonBoardCanvas()
    {
        canvasManager.Switch();
    }

    public void ExecuteTurn()
    {
        myInputManager.EmulateExecute();
        ResetTurnDisplay();
    }
}
