namespace PWSW5;

public class LogActionCommand(string actionName, List<string> log) : IAgentCommand
{
    public string Description => $"Zapisz akcje: {actionName}";

    public void Execute()
    {
        log.Add($"[{DateTime.Now:HH:mm:ss}] Akcja: {actionName}");
        Console.WriteLine($"[LOG] Dodano wpis: {actionName}");
    }

    public void Undo()
    {
        if (log.Count > 0)
        {
            log.RemoveAt(log.Count - 1);
            Console.WriteLine($"[LOG] Usunieto ostatni wpis (cofnieto: {actionName})");
        }
    }
}
