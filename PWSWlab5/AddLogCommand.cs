namespace PWSW5;

public class AddLogCommand : IAgentCommand
{
    private readonly List<LogEntry> _log;
    private readonly LogEntry _wpis;

    public AddLogCommand(string content, string category, List<LogEntry> log)
    {
        _log = log;
        _wpis = new LogEntry(content, category, DateTime.Now);
    }

    public string Description => $"Dodaj log: {_wpis.Content}";

    public void Execute() => _log.Add(_wpis);

    public void Undo()
    {
        if (_log.Count > 0 && _log[^1] == _wpis)
            _log.RemoveAt(_log.Count - 1);
    }
}
