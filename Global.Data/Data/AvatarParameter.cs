namespace Global.Data.Data;

[System.Serializable]
public class AvatarParameter
{
    public bool enabled { get; set; }
    public string? name { get; set; }
    public ParameterValueType type { get; set; }

    public AvatarParameter() { }
    public AvatarParameter(bool enabled, string name, ParameterValueType type)
    {
        this.enabled = enabled;
        this.name = name;
        this.type = type;
    }
}