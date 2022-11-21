namespace Global.Data.Data.Web;

[System.Serializable]
public class AvatarParameterDTO
{
    public float value { get; set; }
    public string? name { get; set; }
    public ParameterValueType type { get; set; }

    public AvatarParameterDTO() { }
    
    public AvatarParameterDTO(float value, string name, ParameterValueType type)
    {
        this.value = value;
        this.name = name;
        this.type = type;
    }
}