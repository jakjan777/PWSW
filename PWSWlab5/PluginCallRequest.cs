namespace PWSW5;

public record PluginCallRequest(string PluginName, string Query) : IRequest<PluginCallResponse>;

public record PluginCallResponse(bool Success, string Result, long ElapsedMs);
