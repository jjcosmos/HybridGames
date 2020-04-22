
[System.Serializable]
public class Net_JoinLobby : NetMsg
{

    public Net_JoinLobby()
    {
        OP = NetOP.JoinLobby;
    }
    public int lobbyID;//the lobby the player is trying to join

}

