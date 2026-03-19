# Data Transfer Harness

A test harness for generating Medplum bot code using Claude AI.

## Project Structure

```
DataTransferHarness/
├── DataTransferHarness.sln
├── README.md
│
├── src/
│   └── DataTransferHarness/
│       ├── DataTransferHarness.csproj
│       ├── Program.cs                      ← Entry point
│       │
│       ├── Harness/
│       │   ├── HarnessRunner.cs            ← Orchestrates the pipeline
│       │   └── HarnessResult.cs            ← Result model
│       │
│       ├── Tickets/
│       │   └── TicketLoader.cs             ← Reads ticket.md from disk
│       │
│       ├── Context/
│       │   ├── ContextAssembler.cs         ← Assembles context files
│       │   └── ContextSource.cs            ← Context model
│       │
│       ├── Agent/
│       │   ├── IAgentClient.cs             ← Agent interface
│       │   ├── ClaudeAgentClient.cs        ← Anthropic API integration
│       │   ├── AgentPromptBuilder.cs       ← Builds prompts
│       │   └── AgentResponse.cs            ← Response model
│       │
│       ├── Validation/
│       │   ├── IOutputValidator.cs         ← Validator interface
│       │   ├── BotOutputValidator.cs       ← TypeScript validation
│       │   └── ValidationReport.cs         ← Validation results
│       │
│       ├── Output/
│       │   └── OutputWriter.cs             ← Saves generated code
│       │
│       └── Configuration/
│           └── HarnessConfig.cs            ← Configuration management
│
├── context/                                ← Reference documentation
│   ├── medplum-bot-basics.md
│   ├── fhir-bundle-example.json
│   └── medplum-bot-example.ts
│
├── tickets/                                ← Task definitions
│   └── patient-import-pipeline.md
│
└── output/                                 ← Generated code (created at runtime)
    ├── generated-bot-{timestamp}.ts
    └── validation-report-{timestamp}.txt
```

## Setup

### Prerequisites
- .NET 8.0 SDK or later
- Anthropic API key

### Installation

1. Clone the repository

2. Configure the application:
   ```bash
   cd src/DataTransferHarness
   cp appsettings.example.json appsettings.json
   ```

3. Edit `appsettings.json` and set your API key:
   ```json
   {
     "Claude": {
       "ApiKey": "sk-ant-your-actual-api-key-here",
       "Model": "claude-3-7-sonnet-20250219",
       "MaxTokens": 4096
     },
     "Paths": {
       "TicketsFolder": "../../tickets",
       "ContextFolder": "../../context",
       "OutputFolder": "../../output"
     }
   }
   ```

4. Restore dependencies:
   ```bash
   dotnet restore
   ```

## Usage

### Run the harness:
```bash
cd src/DataTransferHarness
dotnet run
```

### Pipeline Flow:
1. **Load Ticket**: Reads the task definition from `tickets/patient-import-pipeline.md`
2. **Assemble Context**: Loads all files from `context/` directory
3. **Call AI Agent**: Sends ticket + context to Claude to generate code
4. **Validate Output**: Checks the generated TypeScript for basic correctness
5. **Write Output**: Saves generated code and validation report to `output/`

### Output Files:
- `generated-bot-{timestamp}.ts` - The generated TypeScript code
- `validation-report-{timestamp}.txt` - Validation results and metrics

## Configuration

All configuration is managed through `appsettings.json`:

### Claude Settings
- `Claude:ApiKey` (required) - Your Anthropic API key
- `Claude:Model` (required) - Model to use (default: `claude-3-7-sonnet-20250219`)
  - For agentic flows, use: `claude-3-7-sonnet-20250219` (best for complex reasoning)
- `Claude:MaxTokens` (optional) - Max tokens for response (default: `4096`)

### Path Settings
- `Paths:TicketsFolder` - Relative or absolute path to tickets folder (default: `../../tickets`)
- `Paths:ContextFolder` - Relative or absolute path to context folder (default: `../../context`)
- `Paths:OutputFolder` - Relative or absolute path to output folder (default: `../../output`)

All relative paths are resolved from the application's working directory.

## Customization

### Adding Context Files:
Place any markdown, JSON, or TypeScript files in the `context/` directory. They will be automatically loaded and included in the agent prompt.

### Creating New Tickets:
Create a new `.md` file in the `tickets/` directory. The harness will automatically use the first `.md` file it finds.

### Creating Custom Agents:
The harness supports multiple specialized AI agents. See [AGENTS.md](AGENTS.md) for a complete guide on:
- Creating new agent types
- Agent description file format
- Built-in agents (Medplum TypeScript Developer, etc.)
- Multi-agent workflows

### Swapping AI Providers:
Implement the `IAgentClient` interface to support other LLM providers (e.g., OpenAI GPT-4).

## Validation

The `BotOutputValidator` performs basic checks:
- ✓ Exports a default function
- ✓ Contains function definitions
- ✓ Uses Medplum types (BotEvent, MedplumClient)
- ✓ Balanced braces and parentheses
- ✓ Minimum code length

For production use, consider adding:
- TypeScript compilation check
- ESLint validation
- Unit test generation
- FHIR resource validation

## License

MIT
