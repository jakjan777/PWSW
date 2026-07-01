using DiagnosticsApi;
using DiagnosticsApi.Contracts;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/diagnostics", () =>
{
    var info = ArchInfo.Collect();
    return Results.Ok(ArchInfoMapper.ToResponse(info));
});

app.Run();
