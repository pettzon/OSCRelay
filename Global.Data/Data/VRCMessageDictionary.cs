namespace Global.Data.Data;

public class VRCMessageDictionary
{
    public static readonly Dictionary<string, VRCMessageType> StringTypeDictionary = new Dictionary<string, VRCMessageType>()
    {
        {"parameters", VRCMessageType.AVATAR_PARAMETER},
        {"change", VRCMessageType.AVATAR_CHANGE},
        {"VelocityX", VRCMessageType.MUTE_SELF},
        {"VelocityY", VRCMessageType.MUTE_SELF},
        {"VelocityZ", VRCMessageType.MUTE_SELF},
        {"AngularY", VRCMessageType.MUTE_SELF},
        {"TrackingType", VRCMessageType.MUTE_SELF},
        {"MuteSelf", VRCMessageType.MUTE_SELF}
    };
}