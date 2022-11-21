namespace Global.Data.Data;

public class VRCMessageDictionary
{
    public static readonly Dictionary<string, VRCMessageType> StringTypeDictionary = new Dictionary<string, VRCMessageType>()
    {
        {"/avatar/change", VRCMessageType.AVATAR_CHANGE},
        {"/avatar/parameters/MuteSelf", VRCMessageType.MUTE_SELF},
        {"/avatar/parameters", VRCMessageType.ENABLED_PARAMETER}
    };

    public static readonly Dictionary<VRCMessageType, string> TypeValueDictionary = new Dictionary<VRCMessageType, string>()
    {
        {VRCMessageType.AVATAR_CHANGE, "string"},
        {VRCMessageType.MUTE_SELF, "bool"}
    };
}