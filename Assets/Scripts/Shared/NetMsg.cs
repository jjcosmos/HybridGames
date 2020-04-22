public static class NetOP
{
    public const int None = 0;
    public const int CreateAccount = 1;
    public const int LoginRequest = 2;
    public const int OnCreateAccount = 3;
    public const int OnLoginRequest = 4;
    public const int SendTurn = 5;
    public const int OnSendTurn = 6;
    public const int CreateLobbyRequest = 7;
    public const int OnCreateLobby = 8;
    public const int JoinLobby = 9;
    public const int OnJoinLobby = 10;
}

[System.Serializable]
public abstract class NetMsg
{
    public byte OP { set; get; }

    public NetMsg()
    {
        OP = NetOP.None;
    }
}
