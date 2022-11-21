using Global.Data.Data.VRChat;

namespace Global.Data.Data;

[System.Serializable]
public class ExposedParameters
{
    public string avatarID { get; set; }
    public List<AvatarParameter> avatarParameters { get; set; }

    public ExposedParameters(string avatarID, List<AvatarParameter> avatarParameters)
    {
        this.avatarID = avatarID;
        this.avatarParameters = avatarParameters;
    }
}