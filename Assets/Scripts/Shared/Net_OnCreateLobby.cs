
[System.Serializable]
public class Net_OnCreateLobby : NetMsg
{
    public Net_OnCreateLobby()
    {
        OP = NetOP.OnCreateLobby;
    }

    public int assignedLobbyID;


}

