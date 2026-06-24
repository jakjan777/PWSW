namespace PWSW5;

public class CommandHistory
{
    private readonly Stack<IAgentCommand> _undoStack = new();
    private readonly Stack<IAgentCommand> _redoStack = new();

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public void ExecuteCommand(IAgentCommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
        Console.WriteLine($"[Historia] Wykonano: {command.Description}");
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            Console.WriteLine("Brak polecen do cofniecia.");
            return;
        }

        var cmd = _undoStack.Pop();
        cmd.Undo();
        _redoStack.Push(cmd);
        Console.WriteLine($"[Historia] Cofnieto: {cmd.Description}");
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            Console.WriteLine("Brak polecen do ponowienia.");
            return;
        }

        var cmd = _redoStack.Pop();
        cmd.Execute();
        _undoStack.Push(cmd);
        Console.WriteLine($"[Historia] Ponowiono: {cmd.Description}");
    }
}
