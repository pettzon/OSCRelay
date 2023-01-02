namespace Global.Data.Data.Web;

[System.Serializable]
public class AllAvatarParametersDTO
{
    public string avatarName { get; set; }
    public List<AvatarParameterDTO> avatarParameters { get; set; }

    public AllAvatarParametersDTO(string avatarName, List<AvatarParameterDTO> avatarParameters)
    {
        this.avatarName = avatarName;
        this.avatarParameters = avatarParameters;
    }
}