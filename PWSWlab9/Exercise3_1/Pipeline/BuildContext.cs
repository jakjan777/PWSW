namespace Exercise3_1.Pipeline;

public class BuildContext
{
    public required string ProjectPath { get; set; }
    public required string Configuration { get; set; }
    public string Runtime { get; set; } = "win-x64";
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public List<string> Log { get; } = [];
    public string? OutputPath { get; set; }
}
