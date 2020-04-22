
[System.Serializable]
public class Net_OnLoginRequest : NetMsg
{
    public Net_OnLoginRequest()
    {
        OP = NetOP.OnLoginRequest;
    }
    public byte Success { set; get; }
    public string Information { set; get; }
    public int ConnectionID { get; set; }
    public string Username { get; set; }
    public string Discriminator { get; set; }
    public string Token { get; set; }

}

