namespace Global.Data.Data;

[System.Serializable]
public class AvatarParameter
{
    public bool enabled { get; set; }
    
    public float value { get; set; }
    public string? name { get; set; }
    public ParameterValueType type { get; set; }

    public AvatarParameter() { }
    public AvatarParameter(bool enabled, string name, ParameterValueType type)
    {
        this.enabled = enabled;
        this.name = name;
        this.type = type;
    }
    
    public AvatarParameter(bool enabled, string name, float value, ParameterValueType type)
    {
        this.enabled = enabled;
        this.name = name;
        this.type = type;
        this.value = value;
    }
}