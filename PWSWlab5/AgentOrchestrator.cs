namespace PWSW5;

public class AgentOrchestrator(IEnumerable<IAgentPlugin> plugins)
{
    public void ListujWtyczki()
    {
        Console.WriteLine("Zarejestrowane wtyczki agenta:");
        foreach (var p in plugins)
            Console.WriteLine($"  [{p.Nazwa}] {p.OpisFunkcji}");
    }
}
