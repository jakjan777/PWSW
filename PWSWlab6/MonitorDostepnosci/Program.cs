using MonitorDostepnosci.Services;
using MonitorService = MonitorDostepnosci.Services.MonitorDostepnosci;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<MonitorDostepnosci.RepozytoriumWynikow>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<MonitorService>();

var app = builder.Build();
await app.RunAsync();
