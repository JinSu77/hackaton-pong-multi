public enum Team
{
    None,
    Left,
    Right
}

[System.Serializable]
public class PlayerTeamInfo
{
    public string ClientId;
    public Team Team;
}