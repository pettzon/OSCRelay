namespace Global.Data.Data.VRChat;

[System.Serializable]
public class VRCData
{
    public string? id { get; set; }
    public string? name { get; set; }
    public IList<Parameter>? parameters { get; set; }

    public VRCData()
    {
        parameters = new List<Parameter>();
    }
}