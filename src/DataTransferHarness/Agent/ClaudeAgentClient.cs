using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using DataTransferHarness.Configuration;
using DataTransferHarness.Context;

namespace DataTransferHarness.Agent;

/// <summary>
/// Base class for all Claude-powered agents
/// </summary>
public abstract class ClaudeAgentClient(HarnessConfig config) : BaseAgentClient(config.AgentsPath), IAgentClient
{
    protected readonly HarnessConfig Config = config;

    public override async Task<AgentResponse> ExecuteAsync(string ticket, List<ContextSource> context)
    {
        var client = new AnthropicClient(new APIAuthentication(Config.ApiKey));

        var agentType = GetAgentType();
        var (systemPrompt, userPrompt) = PromptBuilder.BuildPrompts(agentType, ticket, context);

        var messages = new List<Message>
        {
            new(RoleType.User, userPrompt)
        };

        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = Config.MaxTokens,
            Model = Config.ModelName,
            Stream = false,
            Temperature = 0.7m,
            System = [new(systemPrompt)]
        };

        var response = await client.Messages.GetClaudeMessageAsync(parameters);
        

        if (response.Content == null || response.Content.Count == 0)
        {
            throw new InvalidOperationException("Agent returned empty response");
        }

        // Extract text from ContentBase objects (SDK v5+ uses TextContent type)
        var generatedCode = string.Empty;
        foreach (var content in response.Content)
        {
            if (content is TextContent textContent)
            {
                generatedCode = textContent.Text;
                break;
            }
        }

        if (string.IsNullOrEmpty(generatedCode))
        {
            throw new InvalidOperationException("No text content found in agent response");
        }

        return new AgentResponse
        {
            GeneratedCode = generatedCode,
            RawResponse = response.ToString() ?? string.Empty,
            Model = response.Model,
            UsageInputTokens = response.Usage.InputTokens,
            UsageOutputTokens = response.Usage.OutputTokens
        };
    }
}
