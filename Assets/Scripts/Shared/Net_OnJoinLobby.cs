
[System.Serializable]
public class Net_OnJoinLobby : NetMsg
{

    public Net_OnJoinLobby()
    {
        OP = NetOP.OnJoinLobby;
    }
    public bool succeeded;//if the lobby exists
    public int assignedPlayerID;//the player ID assigned

}

