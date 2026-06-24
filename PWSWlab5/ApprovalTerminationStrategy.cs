using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

namespace PWSW5;

public class ApprovalTerminationStrategy : TerminationStrategy
{
    protected override Task<bool> ShouldAgentTerminateAsync(
        Agent agent,
        IReadOnlyList<ChatMessageContent> history,
        CancellationToken cancellationToken)
    {
        var lastMessage = history.LastOrDefault()?.Content ?? "";
        return Task.FromResult(
            lastMessage.Contains("ZATWIERDZAM", StringComparison.OrdinalIgnoreCase));
    }
}
