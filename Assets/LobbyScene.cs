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

    [Header("Output")]
    [SerializeField] TextMeshProUGUI WelcomeMessage;
    [SerializeField] TextMeshProUGUI AuthenticationMessage;
    [SerializeField] SessionIDUpdater LobbyIDMessage;

    [Header("Canvas")]
    [SerializeField] CanvasGroup canvasGroup;

    [Header("TurnInput")]
    [SerializeField] TMP_InputField p1Input;
    [SerializeField] TMP_InputField p2Input;

    [Header("TurnOutput")]
    [SerializeField] TextMeshProUGUI p1Out;
    [SerializeField] TextMeshProUGUI p2Out;

    [Header("Lobby")]
    [SerializeField] TextMeshProUGUI lobbyDisplay;
    [SerializeField] TMP_InputField lobbyToJoin;

    [Header("Player ID's")]
    [SerializeField] TextMeshProUGUI playerAID;
    [SerializeField] TextMeshProUGUI playerBID;
    [SerializeField] Button playerAButton;
    [SerializeField] Button playerBButton;

    public static LobbyScene instance;
    private bool firstFlag = true;
    private void Awake()
    {
        instance = this;
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
        DisableInputs();
        if(int.TryParse(lobbyToJoin.text, out int i))
        {
            Client.instance.SendJoinLobby(i);
        }
        else
        {
            EnableInputs();
            Debug.LogError("Please enter the number of the lobby");
        }
        
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
        if (firstFlag)
        {
            playerAID.text = ID.ToString();
            firstFlag = false;
            playerAButton.interactable = false;
            
        }
        else
        {
            playerBID.text = ID.ToString();
            firstFlag = false;
            playerBButton.interactable = false;
        }
    }

    public void UpdateLobbyDisplay(int lobbyID)
    {
        lobbyDisplay.text = "Lobby ID: " + lobbyID.ToString();
        LobbyIDMessage.OnInitSession("Lobby ID: " + lobbyID.ToString());
        StopAllCoroutines();
        StartCoroutine(FadeOutCanvas());
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
        if (player == 1) 
        {
            p1Out.text = turn;
        }
        else if (player == 2)
        {
            p2Out.text = turn;
        }
        else
        {
            Debug.LogError("Bad player recieved");
        }
    }
}
