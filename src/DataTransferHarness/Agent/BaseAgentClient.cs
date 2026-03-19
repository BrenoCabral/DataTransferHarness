using DataTransferHarness.Context;

namespace DataTransferHarness.Agent;

public abstract class BaseAgentClient
{
    protected readonly AgentPromptBuilder PromptBuilder;

    protected BaseAgentClient(string agentsPath)
    {
        PromptBuilder = new AgentPromptBuilder(agentsPath);
    }

    /// <summary>
    /// Executes the agent with the given ticket and context
    /// </summary>
    public abstract Task<AgentResponse> ExecuteAsync(string ticket, List<ContextSource> context);

    /// <summary>
    /// Returns the agent type identifier used to load the system prompt
    /// </summary>
    protected abstract string GetAgentType();
}
