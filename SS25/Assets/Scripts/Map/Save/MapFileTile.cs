using System.Runtime.Serialization;

public enum MapFileTile
{
    [EnumMember(Value = "N")]
    None,
    [EnumMember(Value = "G")]
    Grass,
    [EnumMember(Value = "S")]
    Sand,
    [EnumMember(Value = "W")]
    Water,
}