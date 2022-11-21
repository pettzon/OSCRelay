namespace Global.Data.Data.Web;

[System.Serializable]
public class AllAvatarParametersDTO
{
    public string avatarID { get; set; }
    public List<AvatarParameterDTO> avatarParameters { get; set; }

    public AllAvatarParametersDTO(string avatarID, List<AvatarParameterDTO> avatarParameters)
    {
        this.avatarID = avatarID;
        this.avatarParameters = avatarParameters;
    }
}