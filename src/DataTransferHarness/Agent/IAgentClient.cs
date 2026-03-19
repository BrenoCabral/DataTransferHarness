using DataTransferHarness.Context;

namespace DataTransferHarness.Agent;

public interface IAgentClient
{
    Task<AgentResponse> ExecuteAsync(string ticket, List<ContextSource> context);
}
