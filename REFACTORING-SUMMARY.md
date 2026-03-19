# Agent System Refactoring Summary

## Overview

The agent system has been refactored to support multiple specialized AI agents with externalized descriptions. This makes the system more flexible, maintainable, and extensible.

## What Changed

### Before (Monolithic)

```
ClaudeAgentClient
├─ Hardcoded system prompt in BuildSystemPrompt()
├─ Single GenerateCodeAsync() method
└─ Tightly coupled to TypeScript/Medplum use case
```

### After (Modular)

```
BaseAgentClient (Abstract)
    ↓
ClaudeAgentClient (Abstract)
    ↓
├─ MedplumTypeScriptAgent
├─ PythonDataAnalystAgent (example)
└─ [Your custom agents...]

Agent descriptions stored in: agents/*.md
```

## Key Changes

### 1. Agent Hierarchy

**New Classes:**
- `BaseAgentClient` - Abstract base for all agents
- `ClaudeAgentClient` - Abstract Claude API integration (was concrete)
- `MedplumTypeScriptAgent` - Concrete Medplum-specific agent

**Pattern:**
```csharp
// Abstract base
public abstract class BaseAgentClient
{
    public abstract Task<AgentResponse> ExecuteAsync(...);
    protected abstract string GetAgentType();
}

// Claude implementation
public abstract class ClaudeAgentClient : BaseAgentClient
{
    public override async Task<AgentResponse> ExecuteAsync(...)
    {
        // Generic Claude API logic
    }
}

// Specific agent
public class MedplumTypeScriptAgent : ClaudeAgentClient
{
    protected override string GetAgentType() => "medplum-typescript-developer";
}
```

### 2. Externalized Agent Descriptions

**Before:**
```csharp
private static string BuildSystemPrompt()
{
    return @"You are an expert TypeScript developer..."; // Hardcoded
}
```

**After:**
```
agents/medplum-typescript-developer.md  ← Agent description
agents/python-data-analyst.md           ← Another agent
agents/your-custom-agent.md             ← Easily add more
```

**Loading:**
```csharp
AgentPromptBuilder
├─ Loads agent description from agents/{agentType}.md
├─ Caches in memory for performance
└─ Maps agent type → description file
```

### 3. Method Renaming

**Interface Change:**
```csharp
// Before
Task<AgentResponse> GenerateCodeAsync(...)

// After (generic)
Task<AgentResponse> ExecuteAsync(...)

// Specific agents can still provide domain methods:
public class MedplumTypeScriptAgent
{
    public async Task<AgentResponse> GenerateCodeAsync(...)
    {
        return await ExecuteAsync(...); // Calls base
    }
}
```

### 4. Configuration Enhancement

**appsettings.json:**
```json
{
  "Paths": {
    "AgentsFolder": "../../agents"  // ← New
  }
}
```

**HarnessConfig:**
```csharp
public string AgentsPath { get; set; }  // ← New property
```

### 5. Improved AgentPromptBuilder

**Before:**
```csharp
BuildPrompts(ticket, context)  // No agent type parameter
```

**After:**
```csharp
BuildPrompts(agentType, ticket, context)  // Agent type specified
```

**Features:**
- Loads descriptions from configurable path
- Caches descriptions for reuse
- Validates files exist
- Clear error messages

## New Files

### Implementation Files
- `src/DataTransferHarness/Agent/BaseAgentClient.cs` - Abstract base
- `src/DataTransferHarness/Agent/MedplumTypeScriptAgent.cs` - Concrete agent

### Agent Descriptions
- `agents/medplum-typescript-developer.md` - Medplum agent description
- `agents/python-data-analyst.md` - Example second agent

### Documentation
- `AGENTS.md` - Complete agent system guide
- `ARCHITECTURE.md` - System architecture overview
- `REFACTORING-SUMMARY.md` - This file

## Modified Files

### Core Changes
- `Agent/ClaudeAgentClient.cs` - Now abstract, generic Claude integration
- `Agent/IAgentClient.cs` - Method renamed to `ExecuteAsync`
- `Agent/AgentPromptBuilder.cs` - Instance-based, configurable path

### Configuration
- `Configuration/HarnessConfig.cs` - Added `AgentsPath` property
- `appsettings.json` - Added `Paths:AgentsFolder`
- `appsettings.example.json` - Updated template

### Integration
- `Harness/HarnessRunner.cs` - Uses `ExecuteAsync` instead of `GenerateCodeAsync`
- `Program.cs` - Creates `MedplumTypeScriptAgent` instead of `ClaudeAgentClient`

### Documentation
- `README.md` - Added agent system reference
- `CONFIGURATION.md` - Updated with agents path

## Benefits

### 1. Separation of Concerns
- **Agent Logic** (C# code) separated from **Agent Expertise** (markdown)
- Non-developers can modify agent behavior by editing markdown
- Code remains stable while prompts evolve

### 2. Extensibility
```csharp
// Adding a new agent is easy:
public class SqlQueryAgent : ClaudeAgentClient
{
    protected override string GetAgentType() => "sql-query-builder";
}
```

### 3. Reusability
- Claude API logic written once
- All agents inherit error handling, token tracking, etc.
- Common patterns abstracted to base classes

### 4. Maintainability
- Agent descriptions are version-controllable
- Easy to A/B test different prompts
- Clear structure for new team members

### 5. Testability
- Mock different agent types
- Test prompt building independently
- Validate agent descriptions separately

## Migration Guide

### For Existing Code

If you have code using the old API:

```csharp
// Before
var agent = new ClaudeAgentClient(config);
var response = await agent.GenerateCodeAsync(ticket, context);

// After
var agent = new MedplumTypeScriptAgent(config);
var response = await agent.ExecuteAsync(ticket, context);
// OR (convenience method)
var response = await agent.GenerateCodeAsync(ticket, context);
```

### For Custom Implementations

If you extended `ClaudeAgentClient`:

1. Create an agent description file in `agents/`
2. Update your class to inherit from `ClaudeAgentClient`
3. Implement `GetAgentType()` to return your agent type
4. Remove hardcoded prompts from your code

## Usage Examples

### Example 1: Using Built-in Agent

```csharp
var config = HarnessConfig.Load();
var agent = new MedplumTypeScriptAgent(config);

var ticket = "Create a patient import bot";
var context = assembler.AssembleAsync().Result;

var response = await agent.ExecuteAsync(ticket, context);
Console.WriteLine(response.GeneratedCode);
```

### Example 2: Creating Custom Agent

**Step 1:** Create `agents/data-pipeline-builder.md`
```markdown
# Data Pipeline Builder

You are an expert in Apache Airflow and data engineering.
Generate production-ready DAG definitions.

## Guidelines
- Use best practices for Airflow 2.x
- Include error handling and retries
- Add monitoring and alerting
```

**Step 2:** Create implementation
```csharp
public class DataPipelineAgent : ClaudeAgentClient
{
    private const string AgentType = "data-pipeline-builder";

    public DataPipelineAgent(HarnessConfig config) : base(config) { }

    protected override string GetAgentType() => AgentType;
}
```

**Step 3:** Use it
```csharp
var agent = new DataPipelineAgent(config);
var response = await agent.ExecuteAsync(pipelineTicket, context);
```

### Example 3: Multi-Agent Workflow

```csharp
// Generate code with one agent
var devAgent = new MedplumTypeScriptAgent(config);
var code = await devAgent.ExecuteAsync(ticket, context);

// Review with another agent
var reviewContext = new List<ContextSource>
{
    new() { Name = "code.ts", Content = code.GeneratedCode }
};

var reviewAgent = new CodeReviewAgent(config);
var review = await reviewAgent.ExecuteAsync("Review this code", reviewContext);
```

## Best Practices

### 1. Agent Naming
- Use kebab-case for agent types: `my-agent-name`
- Match filename to agent type: `my-agent-name.md`
- Be descriptive: `typescript-react-component` not `ts-comp`

### 2. Agent Descriptions
- Keep focused on a single domain
- Include clear guidelines
- Specify output format
- Add examples if helpful

### 3. Class Naming
- Use PascalCase: `MyAgentName`
- Suffix with `Agent`: `SqlQueryAgent`
- Be descriptive of domain: `MedplumTypeScriptAgent`

### 4. Organization
```
agents/
├── medplum-typescript-developer.md
├── python-data-analyst.md
└── sql-query-builder.md

src/DataTransferHarness/Agent/
├── BaseAgentClient.cs
├── ClaudeAgentClient.cs
├── MedplumTypeScriptAgent.cs
├── PythonDataAnalystAgent.cs
└── SqlQueryBuilderAgent.cs
```

## Testing

### Unit Test Example

```csharp
[Test]
public async Task MedplumAgent_LoadsCorrectDescription()
{
    var config = new HarnessConfig
    {
        AgentsPath = "path/to/agents",
        // ... other config
    };

    var agent = new MedplumTypeScriptAgent(config);

    // Agent type should match file
    var agentType = agent.GetType()
        .GetMethod("GetAgentType", BindingFlags.NonPublic | BindingFlags.Instance)
        .Invoke(agent, null);

    Assert.AreEqual("medplum-typescript-developer", agentType);
}
```

## Performance Impact

### Positive
- **Caching:** Agent descriptions loaded once, cached forever
- **Async I/O:** File reading is async
- **Minimal overhead:** Description loading < 1ms after cache

### Neutral
- **Same API calls:** No change to Claude API interaction
- **Same token usage:** Prompts are equivalent

## Breaking Changes

⚠️ **API Changes:**

1. `IAgentClient.GenerateCodeAsync()` → `IAgentClient.ExecuteAsync()`
2. `ClaudeAgentClient` is now abstract
3. `AgentPromptBuilder` requires `agentsPath` constructor parameter

✅ **Migration Path:**

1. Update method calls to `ExecuteAsync()`
2. Replace `new ClaudeAgentClient()` with `new MedplumTypeScriptAgent()`
3. Add `AgentsFolder` to configuration

## Future Enhancements

### Possible Extensions

1. **Agent Registry**
   ```csharp
   var registry = new AgentRegistry(config);
   registry.Register<MedplumTypeScriptAgent>("medplum");
   registry.Register<PythonDataAnalystAgent>("python");

   var agent = registry.Get("medplum");
   ```

2. **Dynamic Agent Loading**
   ```csharp
   // Load agents from plugins
   var agent = AgentLoader.LoadFromAssembly("MyCustomAgents.dll", "SqlAgent");
   ```

3. **Agent Versioning**
   ```csharp
   var agent = new MedplumTypeScriptAgent(config, version: "v2");
   // Loads agents/medplum-typescript-developer-v2.md
   ```

4. **Multi-Provider Support**
   ```csharp
   public class OpenAITypeScriptAgent : OpenAIAgentClient { }
   public class ClaudeTypeScriptAgent : ClaudeAgentClient { }
   ```

## Conclusion

The refactored agent system provides:

✅ **Flexibility** - Easy to add new agent types
✅ **Maintainability** - Clear separation of concerns
✅ **Extensibility** - Multiple inheritance points
✅ **Reusability** - Shared Claude API logic
✅ **Testability** - Mockable interfaces
✅ **Clarity** - Self-documenting agent descriptions

The system is now ready for production use and future expansion.
