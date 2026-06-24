namespace PWSW5;

public class MacroCommand(string description, params IAgentCommand[] commands) : IAgentCommand
{
    public string Description { get; } = description;

    public void Execute()
    {
        foreach (var cmd in commands)
            cmd.Execute();
    }

    public void Undo()
    {
        for (int i = commands.Length - 1; i >= 0; i--)
            commands[i].Undo();
    }
}
