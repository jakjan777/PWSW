using Challenge2.Domain;

namespace Challenge2.Application;

public class BumpVersionUseCase(IVersionRepository repo)
{
    private readonly IVersionRepository _repo = repo;

    public async Task<AppVersion> ExecuteAsync(string packagePath, CancellationToken ct = default)
    {
        var current = await _repo.ReadAsync(packagePath, ct);
        var bumped = current.BumpBuild();
        await _repo.WriteAsync(packagePath, bumped, ct);
        return bumped;
    }
}
