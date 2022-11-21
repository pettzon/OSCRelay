namespace Global.Data.Data.Web;

[System.Serializable]
public class Token
{
    public string token { get; private set; }

    public Token(string token)
    {
        this.token = token;
    }
}