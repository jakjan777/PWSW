using Challenge2.Domain;

namespace Challenge2.Application;

public interface IVersionRepository
{
    Task<AppVersion> ReadAsync(string path, CancellationToken ct = default);
    Task WriteAsync(string path, AppVersion version, CancellationToken ct = default);
}
