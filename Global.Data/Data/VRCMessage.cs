namespace Global.Data.Data;
public class VRCMessage
{
    public VRCMessageType messageType;
    public string parameter;
    public string value;

    public VRCMessage(VRCMessageType messageType, string parameter)
    {
        this.messageType = messageType;
        this.parameter = parameter;
    }
    
    public VRCMessage(VRCMessageType messageType, string parameter, string value)
    {
        this.messageType = messageType;
        this.parameter = parameter;
        this.value = value;
    }
}