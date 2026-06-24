namespace PWSW5;

public class LogCommandHistory
{
    private const int MaxCofniec = 10;

    private readonly List<LogEntry> _log = [];
    private readonly List<IAgentCommand> _historia = [];

    public void Log(string content, string category)
    {
        var command = new AddLogCommand(content, category, _log);
        command.Execute();
        _historia.Add(command);

        if (_historia.Count > MaxCofniec)
            _historia.RemoveAt(0);
    }

    public void Undo()
    {
        if (_historia.Count == 0)
        {
            Console.WriteLine("Brak akcji do cofniecia.");
            return;
        }

        var command = _historia[^1];
        _historia.RemoveAt(_historia.Count - 1);
        command.Undo();
        Console.WriteLine($"Cofnieto: {command.Description}");
    }

    public IEnumerable<LogEntry> GetLog() => _log;
}
