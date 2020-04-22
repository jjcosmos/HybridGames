
[System.Serializable]
public class Net_SendTurn : NetMsg
{
    public Net_SendTurn()
    {
        OP = NetOP.SendTurn;
    }

    public int lobbyID { get; set; }
    public int playerID { get; set; }
    public string TurnString { get; set; }

}

