using Global.Data.Data.Web;

namespace Global.Data.Data;

[System.Serializable]
public class UserSettings
{
    public string AccountID { get; set; }
    public Token Token { get; set; }
    public string XToysToken { get; set; }
    
    public int PortReceive { get; set; }
    public int PortSend { get; set; }
    
    public string? LastActiveAvatarID { get; set; }

    // public UserSettings(string accountId, string token, string xToysToken)
    // {
    //     AccountID = accountId;
    //     Token = token;
    //     XToysToken = xToysToken;
    // }
}