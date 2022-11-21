namespace Global.Data.Data;
public class VRCMessage
{
    public VRCMessageType key;
    public string value;

    public VRCMessage(VRCMessageType key, string value)
    {
        this.key = key;
        this.value = value;
    }
}