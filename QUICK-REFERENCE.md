# Quick Reference Card

## Creating a New Agent (3 Steps)

### 1. Create Agent Description
**File:** `agents/my-agent-type.md`
```markdown
# My Agent Title

You are an expert in [domain].

## Guidelines
- Guideline 1
- Guideline 2

## Output Format
Return only [format].
```

### 2. Create Agent Class
**File:** `src/DataTransferHarness/Agent/MyAgent.cs`
```csharp
using DataTransferHarness.Configuration;

namespace DataTransferHarness.Agent;

public class MyAgent : ClaudeAgentClient
{
    private const string AgentType = "my-agent-type"; // matches filename

    public MyAgent(HarnessConfig config) : base(config) { }

    protected override string GetAgentType() => AgentType;
}
```

### 3. Use in Program.cs
```csharp
var agentClient = new MyAgent(config);
```

---

## Project Structure Cheat Sheet

```
DataTransferHarness/
├── agents/                  ← Agent descriptions (*.md)
├── tickets/                 ← Task definitions (*.md)
├── context/                 ← Reference docs (*.md, *.json, *.ts)
├── output/                  ← Generated code (created at runtime)
│
└── src/DataTransferHarness/
    ├── appsettings.json     ← Configuration (NOT in git)
    ├── Program.cs           ← Entry point
    │
    ├── Agent/
    │   ├── BaseAgentClient.cs
    │   ├── ClaudeAgentClient.cs
    │   ├── MedplumTypeScriptAgent.cs  ← Built-in agent
    │   └── [YourAgent.cs]             ← Add custom agents here
    │
    ├── Configuration/
    │   └── HarnessConfig.cs
    │
    ├── Harness/
    │   ├── HarnessRunner.cs
    │   └── HarnessResult.cs
    │
    ├── Context/
    │   ├── ContextAssembler.cs
    │   └── ContextSource.cs
    │
    ├── Tickets/
    │   └── TicketLoader.cs
    │
    ├── Validation/
    │   ├── BotOutputValidator.cs
    │   └── ValidationReport.cs
    │
    └── Output/
        └── OutputWriter.cs
```

---

## Configuration Quick Reference

**appsettings.json:**
```json
{
  "Claude": {
    "ApiKey": "sk-ant-...",
    "Model": "claude-3-7-sonnet-20250219",
    "MaxTokens": 4096
  },
  "Paths": {
    "TicketsFolder": "../../tickets",
    "ContextFolder": "../../context",
    "OutputFolder": "../../output",
    "AgentsFolder": "../../agents"
  }
}
```

**Models:**
- `claude-3-7-sonnet-20250219` - Best for agentic flows (recommended)
- `claude-3-5-sonnet-20241022` - Good balance
- `claude-3-opus-20240229` - Highest quality

---

## Common Tasks

### Run the Harness
```bash
cd src/DataTransferHarness
dotnet run
```

### Add Context
```bash
# Just drop files in context/
cp my-example.ts context/
cp my-docs.md context/
```

### Create New Ticket
```bash
# Create a .md file in tickets/
echo "# My Task" > tickets/my-task.md
```

### Switch Agent
```csharp
// In Program.cs, change:
var agentClient = new MedplumTypeScriptAgent(config);
// To:
var agentClient = new MyCustomAgent(config);
```

---

## Agent Hierarchy

```
BaseAgentClient (abstract)
    │
    ├─ ExecuteAsync()      ← Override this
    └─ GetAgentType()      ← Override this
        │
        ▼
ClaudeAgentClient (abstract)
    │
    ├─ ExecuteAsync()      ← Already implemented (Claude API)
    └─ GetAgentType()      ← Must override
        │
        ▼
MedplumTypeScriptAgent (concrete)
    │
    ├─ ExecuteAsync()      ← Inherited
    ├─ GetAgentType()      ← Returns "medplum-typescript-developer"
    └─ GenerateCodeAsync() ← Convenience wrapper
```

---

## API Reference

### IAgentClient
```csharp
Task<AgentResponse> ExecuteAsync(string ticket, List<ContextSource> context);
```

### AgentResponse
```csharp
class AgentResponse
{
    string GeneratedCode
    string RawResponse
    string Model
    int UsageInputTokens
    int UsageOutputTokens
}
```

### HarnessConfig
```csharp
class HarnessConfig
{
    string ApiKey
    string ModelName
    int MaxTokens
    string TicketPath
    string ContextPath
    string OutputPath
    string AgentsPath
}
```

---

## Error Messages & Solutions

| Error | Solution |
|-------|----------|
| `Agent description file not found` | Create `agents/{agent-type}.md` |
| `No ticket (.md) files found` | Add a `.md` file to `tickets/` folder |
| `Please set your Anthropic API key` | Edit `appsettings.json`, set `Claude:ApiKey` |
| `Could not find file appsettings.json` | Copy `appsettings.example.json` to `appsettings.json` |

---

## File Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Agent description | `kebab-case.md` | `medplum-typescript-developer.md` |
| Agent class | `PascalCase` + `Agent` | `MedplumTypeScriptAgent.cs` |
| Agent type constant | `kebab-case` (string) | `"medplum-typescript-developer"` |
| Ticket | `kebab-case.md` | `patient-import-pipeline.md` |
| Context files | Any format | `example.ts`, `docs.md`, `sample.json` |

---

## Useful Commands

```bash
# Build
dotnet build

# Run
dotnet run

# Restore packages
dotnet restore

# Clean
dotnet clean

# List output files
ls ../../output/

# View generated code
cat ../../output/generated-bot-*.ts

# View validation report
cat ../../output/validation-report-*.txt
```

---

## Testing Checklist

Before committing:

- [ ] Agent description file exists in `agents/`
- [ ] Agent class implements `GetAgentType()`
- [ ] Agent type matches filename (without `.md`)
- [ ] `dotnet build` succeeds
- [ ] `dotnet run` generates code
- [ ] Validation report shows success
- [ ] Generated code looks correct

---

## Documentation Links

- [README.md](README.md) - Project overview
- [QUICKSTART.md](QUICKSTART.md) - Get started in 4 steps
- [AGENTS.md](AGENTS.md) - Complete agent guide
- [ARCHITECTURE.md](ARCHITECTURE.md) - System architecture
- [CONFIGURATION.md](CONFIGURATION.md) - Configuration details
- [REFACTORING-SUMMARY.md](REFACTORING-SUMMARY.md) - Recent changes

---

## Support

- Issues: Create a GitHub issue
- Questions: Check documentation above
- Examples: See `agents/` folder for agent descriptions
