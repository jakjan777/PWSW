namespace PWSW5;

public interface IAgentCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
