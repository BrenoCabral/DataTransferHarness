using DataTransferHarness.Configuration;
using DataTransferHarness.Context;

namespace DataTransferHarness.Agent;

/// <summary>
/// Specialized agent for generating Medplum TypeScript bots
/// </summary>
public class MedplumTypeScriptAgent : ClaudeAgentClient
{
    private const string AgentType = "medplum-typescript-developer";

    public MedplumTypeScriptAgent(HarnessConfig config) : base(config)
    {
    }

    protected override string GetAgentType() => AgentType;

    /// <summary>
    /// Generates TypeScript code for a Medplum bot
    /// </summary>
    public async Task<AgentResponse> GenerateCodeAsync(string ticket, List<ContextSource> context)
    {
        return await ExecuteAsync(ticket, context);
    }
}
