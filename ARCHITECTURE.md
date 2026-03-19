# Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Program.cs                              │
│                      (Entry Point)                              │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                      HarnessRunner                              │
│                   (Pipeline Orchestrator)                       │
└─┬───────┬───────┬────────┬────────┬──────────────────────────┬─┘
  │       │       │        │        │                          │
  ▼       ▼       ▼        ▼        ▼                          ▼
┌────┐ ┌────┐  ┌────┐  ┌────┐  ┌────────┐              ┌────────┐
│Tick│ │Ctx │  │Agnt│  │Vald│  │Output  │              │Config  │
│et  │ │Asm │  │Clnt│  │ator│  │Writer  │              │        │
└────┘ └────┘  └─┬──┘  └────┘  └────────┘              └────────┘
                 │
                 │ Agent Hierarchy
                 ▼
       ┌──────────────────────┐
       │   BaseAgentClient    │
       │     (Abstract)       │
       └──────────┬───────────┘
                  │
                  ▼
       ┌──────────────────────┐
       │  ClaudeAgentClient   │
       │     (Abstract)       │
       │  - Claude API Logic  │
       └──────────┬───────────┘
                  │
          ┌───────┴───────┐
          ▼               ▼
┌──────────────────┐ ┌──────────────────┐
│ MedplumTypeScript│ │  [Your Custom]   │
│      Agent       │ │      Agent       │
└──────────────────┘ └──────────────────┘
          │                   │
          │                   │
          └───────┬───────────┘
                  │
                  ▼
       ┌──────────────────────┐
       │ AgentPromptBuilder   │
       │  - Loads agent desc  │
       │  - Builds prompts    │
       │  - Caches results    │
       └──────────┬───────────┘
                  │
                  ▼
       ┌──────────────────────┐
       │   agents/ folder     │
       │  *.md description    │
       │      files           │
       └──────────────────────┘
```

## Pipeline Flow

```
1. Load Configuration
   appsettings.json → HarnessConfig

2. Load Ticket
   tickets/*.md → TicketLoader → ticket string

3. Assemble Context
   context/*.* → ContextAssembler → List<ContextSource>

4. Execute Agent
   ticket + context → Agent.ExecuteAsync() → AgentResponse
   │
   ├─ Load agent description (agents/*.md)
   ├─ Build system prompt
   ├─ Build user prompt (ticket + context)
   ├─ Call Claude API
   └─ Return AgentResponse

5. Validate Output
   AgentResponse → BotOutputValidator → ValidationReport

6. Write Output
   AgentResponse + ValidationReport → OutputWriter → output/*.ts + *.txt
```

## Component Responsibilities

### Program.cs
- Entry point
- Initializes all components
- Handles top-level exceptions
- Returns exit code

### HarnessRunner
- Orchestrates the 5-step pipeline
- Logs progress to console
- Returns HarnessResult

### TicketLoader
- Reads ticket markdown files
- Validates file exists and has content
- Returns ticket as string

### ContextAssembler
- Loads all files from context folder
- Determines file types
- Returns List<ContextSource>

### Agent System

#### BaseAgentClient (Abstract)
- Defines `ExecuteAsync()` interface
- Defines `GetAgentType()` interface
- Holds AgentPromptBuilder instance

#### ClaudeAgentClient (Abstract)
- Implements Claude API integration
- Loads agent descriptions
- Builds prompts using AgentPromptBuilder
- Handles API errors
- Tracks token usage

#### MedplumTypeScriptAgent (Concrete)
- Specifies agent type: "medplum-typescript-developer"
- Provides domain-specific method: `GenerateCodeAsync()`
- Inherits all Claude API logic

#### AgentPromptBuilder
- Loads agent descriptions from markdown files
- Maps agent types to description files
- Caches descriptions for performance
- Builds system + user prompts

### BotOutputValidator
- Validates generated TypeScript
- Checks syntax (braces, parens)
- Checks for required patterns
- Returns ValidationReport

### OutputWriter
- Writes generated code to timestamped files
- Writes validation reports
- Creates output directory if needed

### HarnessConfig
- Loads appsettings.json
- Validates configuration
- Resolves relative paths
- Provides configuration to all components

## Data Flow

```
┌──────────────┐
│ appsettings  │
│    .json     │
└──────┬───────┘
       │
       ▼
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  tickets/    │────▶│  HarnessRunner│────▶│  agents/     │
│  *.md        │     │                │     │  *.md        │
└──────────────┘     └───────┬────────┘     └──────────────┘
                             │
┌──────────────┐             │
│  context/    │─────────────┘
│  *.*         │
└──────────────┘
       │
       │
       ▼
┌──────────────┐     ┌────────────────┐     ┌──────────────┐
│   Claude     │────▶│ValidationReport│────▶│   output/    │
│    API       │     │                │     │  *.ts, *.txt │
└──────────────┘     └────────────────┘     └──────────────┘
```

## Configuration Flow

```
appsettings.json
    │
    ├─ Claude.ApiKey ──────────────────────────────────────┐
    ├─ Claude.Model ───────────────────────────────────────┤
    ├─ Claude.MaxTokens ───────────────────────────────────┤
    │                                                      │
    ├─ Paths.TicketsFolder → TicketLoader                  │
    ├─ Paths.ContextFolder → ContextAssembler              │
    ├─ Paths.OutputFolder → OutputWriter                   │
    └─ Paths.AgentsFolder → AgentPromptBuilder             │
                                                           │
                                                           ▼
                                                    ClaudeAgentClient
                                                  (API communication)
```

## Agent Extension Pattern

To add a new agent:

1. **Create description file**: `agents/my-agent.md`
2. **Create implementation**: `src/.../Agent/MyAgent.cs`
3. **Inherit from**: `ClaudeAgentClient`
4. **Override**: `GetAgentType()` → return `"my-agent"`
5. **Use in**: `Program.cs` → `new MyAgent(config)`

```csharp
// Step 2-4
public class MyAgent : ClaudeAgentClient
{
    private const string AgentType = "my-agent"; // matches my-agent.md

    public MyAgent(HarnessConfig config) : base(config) { }

    protected override string GetAgentType() => AgentType;
}

// Step 5
var agent = new MyAgent(config);
var response = await agent.ExecuteAsync(ticket, context);
```

## Error Handling Strategy

```
Program.cs (try-catch)
    │
    ├─ Configuration errors → Exit code 1
    ├─ File not found → Exit code 1
    │
    └─ HarnessRunner
        │
        ├─ TicketLoader errors → Exception thrown
        ├─ ContextAssembler errors → Exception thrown
        │
        └─ Agent.ExecuteAsync()
            │
            ├─ Agent description not found → FileNotFoundException
            ├─ API errors → InvalidOperationException
            │
            └─ Success → Continue
                │
                └─ Validation (no throw, collects errors)
                    │
                    └─ Output writing → Creates files
```

## Security Considerations

1. **API Key Protection**
   - Stored in `appsettings.json`
   - Ignored by git (`.gitignore`)
   - Never logged or written to output

2. **File System Access**
   - All paths validated before use
   - Relative paths resolved safely
   - Output directory created if missing

3. **Input Validation**
   - Configuration validated on startup
   - File existence checked before reading
   - Empty files rejected

## Performance Optimizations

1. **Agent Description Caching**
   - Descriptions loaded once
   - Cached in memory
   - Reused for multiple executions

2. **File System**
   - Async I/O operations
   - Minimal file system traversal

3. **API Calls**
   - Single API call per execution
   - Token usage tracked
   - Configurable max tokens

## Extensibility Points

1. **Custom Agents** - Inherit from `ClaudeAgentClient`
2. **Custom Validators** - Implement `IOutputValidator`
3. **Custom Providers** - Implement `IAgentClient`
4. **Custom Output** - Modify `OutputWriter`
5. **Custom Context Sources** - Add to `ContextAssembler`

## Technology Stack

- **.NET 8.0** - Runtime
- **C# 12** - Language
- **Anthropic.SDK 5.10.0** - Claude API client
- **Microsoft.Extensions.Configuration** - Configuration management

## Future Architecture Considerations

1. **Dependency Injection** - Move to DI container for better testability
2. **Logging Framework** - Add structured logging (Serilog, NLog)
3. **Agent Registry** - Dynamic agent discovery and registration
4. **Plugin System** - Load agents from external assemblies
5. **Async Streaming** - Support streaming responses from Claude
6. **Multi-Provider** - Support multiple AI providers simultaneously
