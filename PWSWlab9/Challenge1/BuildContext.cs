namespace Challenge1;

public class BuildContext
{
    public string ProjectPath { get; init; } = "";
    public Dictionary<string, string> Artifacts { get; init; } = [];
    public bool Success { get; set; } = true;
    public List<string> Log { get; } = [];
}
