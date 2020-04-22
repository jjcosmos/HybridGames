
[System.Serializable]
public class Net_CreateLobbyRequest : NetMsg
{
    //asks to create a lobby & wants to recieve a lobbyID
    public Net_CreateLobbyRequest()
    {
        OP = NetOP.CreateLobbyRequest; //also adds the board host as a connection
    }

}

