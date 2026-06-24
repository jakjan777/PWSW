using System.Net.Http.Json;

namespace PWSW5;

public class KlientPogody(HttpClient http)
{
    public async Task<DanePogodowe?> PobierzAsync(string miasto)
    {
        var odpowiedz = await http.GetAsync($"/pogoda?miasto={miasto}");
        odpowiedz.EnsureSuccessStatusCode();
        return await odpowiedz.Content.ReadFromJsonAsync<DanePogodowe>();
    }
}
