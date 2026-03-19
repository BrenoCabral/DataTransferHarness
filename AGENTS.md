# Agent Architecture Guide

## Overview

The DataTransferHarness uses a flexible agent architecture that allows you to create specialized AI agents for different types of code generation tasks. Each agent is defined by:

1. **Agent Description File** - A markdown file describing the agent's expertise and guidelines
2. **Agent Implementation Class** - A C# class that implements the agent's behavior

## Agent Architecture

```
BaseAgentClient (Abstract)
    ↓
ClaudeAgentClient (Abstract) - Claude API integration
    ↓
MedplumTypeScriptAgent - Specific implementation
PythonDataAnalystAgent - Another example
[YourCustomAgent] - Add your own!
```

## Creating a New Agent

### Step 1: Create the Agent Description File

Create a markdown file in the `agents/` folder that describes your agent:

**File:** `agents/your-agent-type.md`

```markdown
# Your Agent Title

You are an expert [language] developer specializing in [domain].

Your task is to generate [type of output] based on the provided requirements.

## Guidelines

- Follow [standard/framework] best practices
- Use proper [language] features
- Include error handling
- Write clean, maintainable code

## Output Format

Return only the [language] code.
```

### Step 2: Create the Agent Implementation Class

Create a new class that inherits from `ClaudeAgentClient`:

**File:** `src/DataTransferHarness/Agent/YourCustomAgent.cs`

```csharp
using DataTransferHarness.Configuration;
using DataTransferHarness.Context;

namespace DataTransferHarness.Agent;

/// <summary>
/// Specialized agent for [purpose]
/// </summary>
public class YourCustomAgent : ClaudeAgentClient
{
    // This must match the filename (without .md) in the agents/ folder
    private const string AgentType = "your-agent-type";

    public YourCustomAgent(HarnessConfig config) : base(config)
    {
    }

    protected override string GetAgentType() => AgentType;

    /// <summary>
    /// Optional: Add domain-specific methods
    /// </summary>
    public async Task<AgentResponse> YourSpecificMethodAsync(string ticket, List<ContextSource> context)
    {
        return await ExecuteAsync(ticket, context);
    }
}
```

### Step 3: Use Your Agent in Program.cs

Update `Program.cs` to use your new agent:

```csharp
// Replace this:
var agentClient = new MedplumTypeScriptAgent(config);

// With this:
var agentClient = new YourCustomAgent(config);
```

## Built-in Agents

### MedplumTypeScriptAgent

**Type:** `medplum-typescript-developer`
**Purpose:** Generate TypeScript code for Medplum healthcare bots
**Output:** TypeScript bot implementations following Medplum best practices

**Usage:**
```csharp
var agent = new MedplumTypeScriptAgent(config);
var response = await agent.GenerateCodeAsync(ticket, context);
```

### PythonDataAnalystAgent (Example)

**Type:** `python-data-analyst`
**Purpose:** Generate Python scripts for data analysis
**Output:** Python code using pandas, numpy, matplotlib

**Note:** This is an example to demonstrate extensibility. You would need to create the implementation class following Step 2 above.

## Agent System Components

### BaseAgentClient (Abstract Base)

Provides the foundation for all agents:

```csharp
public abstract class BaseAgentClient
{
    protected readonly AgentPromptBuilder PromptBuilder;

    public abstract Task<AgentResponse> ExecuteAsync(string ticket, List<ContextSource> context);
    protected abstract string GetAgentType();
}
```

**Key Methods:**
- `ExecuteAsync` - Main execution method (must override)
- `GetAgentType` - Returns the agent type identifier (must override)

### ClaudeAgentClient (Claude Integration)

Handles communication with the Anthropic Claude API:

```csharp
public abstract class ClaudeAgentClient : BaseAgentClient, IAgentClient
{
    protected readonly HarnessConfig Config;

    public override async Task<AgentResponse> ExecuteAsync(string ticket, List<ContextSource> context)
    {
        // Loads agent description, builds prompts, calls Claude API
    }
}
```

**Features:**
- Automatic system prompt loading from agent description files
- Claude API integration with proper error handling
- Token usage tracking
- Response validation

### AgentPromptBuilder

Manages agent descriptions and builds prompts:

```csharp
public class AgentPromptBuilder
{
    public (string systemPrompt, string userPrompt) BuildPrompts(
        string agentType,
        string ticket,
        List<ContextSource> context)
}
```

**Features:**
- Loads agent descriptions from markdown files
- Caches descriptions for performance
- Builds structured prompts with ticket and context
- Validates that agent description files exist

## Agent Description File Format

Agent descriptions are written in Markdown and should include:

### Required Sections

1. **Title (H1)** - Agent name and role
2. **Task Description** - What the agent does
3. **Guidelines** - Best practices and rules to follow
4. **Output Format** - Expected output format

### Example Structure

```markdown
# [Agent Role]

You are an expert in [domain].

Your task is to [specific task].

## Guidelines

- Guideline 1
- Guideline 2
- Guideline 3

## Output Format

Return only [output type].
```

### Best Practices for Descriptions

1. **Be Specific** - Clearly define the agent's expertise
2. **Set Expectations** - Describe the expected output quality
3. **Include Examples** - If helpful, show code patterns to follow
4. **Define Constraints** - Specify what to avoid or restrictions
5. **Keep It Focused** - Each agent should have a single, clear purpose

## Advanced: Multi-Agent Workflows

You can orchestrate multiple agents for complex tasks:

```csharp
// Example: Generate and then review code
var medplumAgent = new MedplumTypeScriptAgent(config);
var reviewAgent = new CodeReviewAgent(config);

var codeResponse = await medplumAgent.ExecuteAsync(ticket, context);

var reviewContext = new List<ContextSource>
{
    new ContextSource
    {
        Name = "generated-code.ts",
        Content = codeResponse.GeneratedCode,
        Type = "typescript"
    }
};

var reviewResponse = await reviewAgent.ExecuteAsync("Review this code", reviewContext);
```

## Configuration

Agent paths are configured in `appsettings.json`:

```json
{
  "Paths": {
    "AgentsFolder": "../../agents"
  }
}
```

This can be a relative or absolute path.

## Error Handling

### Common Errors

**FileNotFoundException: Agent description file not found**
- Ensure the agent type matches the filename (without .md)
- Check that the file exists in the agents folder
- Verify the `AgentsFolder` path in `appsettings.json`

**InvalidOperationException: Agent description file is empty**
- Make sure the markdown file has content
- Check file encoding (should be UTF-8)

## Testing Agents

### Unit Testing

```csharp
[Test]
public async Task Agent_GeneratesValidCode()
{
    var config = new HarnessConfig
    {
        AgentsPath = "path/to/agents",
        ApiKey = "test-key",
        ModelName = "claude-3-7-sonnet-20250219"
    };

    var agent = new YourCustomAgent(config);
    var context = new List<ContextSource>();

    var response = await agent.ExecuteAsync("Your ticket", context);

    Assert.IsNotNull(response.GeneratedCode);
}
```

### Integration Testing

Run the full harness with your agent to verify end-to-end functionality.

## Agent Versioning

Consider versioning your agent descriptions:

```
agents/
├── medplum-typescript-developer.md       ← Current version
├── medplum-typescript-developer-v1.md    ← Legacy version
└── medplum-typescript-developer-v2.md    ← Experimental version
```

Update the `AgentType` constant in your implementation to switch versions.

## Performance Considerations

### Caching

Agent descriptions are automatically cached after the first load. No need to worry about repeated file reads.

### Token Usage

Monitor token usage in the `AgentResponse`:
- `UsageInputTokens` - Tokens sent to the API
- `UsageOutputTokens` - Tokens received from the API

Adjust `MaxTokens` in configuration based on your needs.

## Future Extensions

The architecture supports:

1. **Multiple AI Providers** - Implement agents using OpenAI, Google, etc.
2. **Streaming Responses** - Add streaming support for real-time feedback
3. **Tool Use** - Integrate function calling/tool use capabilities
4. **Multi-Modal Agents** - Agents that process images, audio, etc.
5. **Agent Chains** - Automatically orchestrate multiple agents

## Examples

### Example 1: SQL Query Generator

**agents/sql-query-builder.md**
```markdown
# SQL Query Builder Agent

You are an expert database engineer specializing in SQL query optimization.

Your task is to generate efficient, secure SQL queries based on requirements.

## Guidelines
- Use parameterized queries to prevent SQL injection
- Optimize for performance with proper indexes
- Include helpful comments
- Handle edge cases

## Output Format
Return only the SQL query.
```

**Implementation:**
```csharp
public class SqlQueryBuilderAgent : ClaudeAgentClient
{
    private const string AgentType = "sql-query-builder";

    public SqlQueryBuilderAgent(HarnessConfig config) : base(config) { }

    protected override string GetAgentType() => AgentType;
}
```

### Example 2: API Documentation Generator

**agents/api-doc-generator.md**
```markdown
# API Documentation Generator

You are a technical writer specializing in API documentation.

Your task is to generate clear, comprehensive API documentation in Markdown format.

## Guidelines
- Include endpoint descriptions
- Document all parameters
- Provide request/response examples
- Add authentication requirements
- List error codes and meanings

## Output Format
Return Markdown documentation.
```

## Contributing

When creating new agents:

1. Follow the naming convention: `your-agent-type.md`
2. Use lowercase with hyphens for agent type identifiers
3. Keep descriptions focused and clear
4. Test thoroughly before committing
5. Document your agent in this file

## Summary

The agent architecture provides:
- ✓ Easy extensibility for new agent types
- ✓ Separation of agent behavior (code) and expertise (description)
- ✓ Reusable Claude API integration
- ✓ Built-in caching and error handling
- ✓ Type-safe implementation in C#
