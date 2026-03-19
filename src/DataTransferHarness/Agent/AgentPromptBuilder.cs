using System.Text;
using DataTransferHarness.Context;

namespace DataTransferHarness.Agent;

public class AgentPromptBuilder
{
    private readonly Dictionary<string, string> _agentDescriptionCache = new();
    private readonly string _agentsBasePath;

    public AgentPromptBuilder(string agentsBasePath)
    {
        _agentsBasePath = agentsBasePath;
    }

    public (string systemPrompt, string userPrompt) BuildPrompts(
        string agentType,
        string ticket,
        List<ContextSource> context)
    {
        var systemPrompt = BuildSystemPrompt(agentType);
        var userPrompt = BuildUserPrompt(ticket, context);

        return (systemPrompt, userPrompt);
    }

    private string BuildSystemPrompt(string agentType)
    {
        // Check cache first
        if (_agentDescriptionCache.TryGetValue(agentType, out var cachedPrompt))
        {
            return cachedPrompt;
        }

        // Build file path based on agent type
        var fileName = $"{agentType}.md";
        var filePath = Path.Combine(_agentsBasePath, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Agent description file not found: {filePath}. " +
                $"Please create a markdown file describing the '{agentType}' agent.");
        }

        // Load and cache the system prompt
        var systemPrompt = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            throw new InvalidOperationException(
                $"Agent description file is empty: {filePath}");
        }

        _agentDescriptionCache[agentType] = systemPrompt;

        return systemPrompt;
    }

    private static string BuildUserPrompt(string ticket, List<ContextSource> context)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# Task");
        sb.AppendLine();
        sb.AppendLine(ticket);
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("# Context");
        sb.AppendLine();

        foreach (var source in context)
        {
            sb.AppendLine($"## {source.Name}");
            sb.AppendLine();
            sb.AppendLine($"```{source.Type}");
            sb.AppendLine(source.Content);
            sb.AppendLine("```");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("Please generate the complete Medplum bot implementation based on the task and context above.");

        return sb.ToString();
    }
}
