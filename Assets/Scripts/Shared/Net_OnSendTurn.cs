
[System.Serializable]
public class Net_OnSendTurn : NetMsg
{
    public Net_OnSendTurn()
    {
        OP = NetOP.OnSendTurn;
    }

    public int lobbyID { get; set; }
    public int playerID { get; set; }
    public string TurnString { get; set; }
    

}

