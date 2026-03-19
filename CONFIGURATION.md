# Configuration Guide

## Overview

DataTransferHarness uses `appsettings.json` for all configuration. This eliminates the need for environment variables and provides a centralized configuration file.

## Setup

### First Time Setup

1. Copy the example configuration:
   ```bash
   cd src/DataTransferHarness
   cp appsettings.example.json appsettings.json
   ```

2. Edit `appsettings.json` with your settings

### Configuration File Structure

```json
{
  "Claude": {
    "ApiKey": "sk-ant-your-api-key-here",
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

## Configuration Options

### Claude Settings

#### ApiKey (Required)
Your Anthropic API key. Get one at https://console.anthropic.com/

**Example:**
```json
"ApiKey": "sk-ant-api03-your-actual-key-here"
```

#### Model (Required)
The Claude model to use for code generation.

**Recommended Models:**
- `claude-3-7-sonnet-20250219` - Best for complex agentic workflows (recommended)
- `claude-3-5-sonnet-20241022` - Good balance of speed and quality
- `claude-3-opus-20240229` - Highest quality (slower, more expensive)

**Example:**
```json
"Model": "claude-3-7-sonnet-20250219"
```

#### MaxTokens (Optional, Default: 4096)
Maximum number of tokens in the AI response.

**Guidelines:**
- `2048` - Short responses, simple bots
- `4096` - Standard responses (recommended)
- `8192` - Long, complex bots

**Example:**
```json
"MaxTokens": 4096
```

### Path Settings

All paths can be:
- **Relative** - Relative to the application working directory (`src/DataTransferHarness`)
- **Absolute** - Full paths like `C:\MyProject\tickets` or `/home/user/tickets`

#### TicketsFolder (Default: "../../tickets")
Location of your ticket markdown files.

The harness will automatically use the **first `.md` file** it finds in this folder.

**Examples:**
```json
"TicketsFolder": "../../tickets"                    // Relative path
"TicketsFolder": "C:\\MyTickets"                    // Absolute (Windows)
"TicketsFolder": "/home/user/tickets"               // Absolute (Linux/Mac)
```

#### ContextFolder (Default: "../../context")
Location of your context files (documentation, examples, etc.).

All files in this folder (`.md`, `.json`, `.ts`, etc.) will be loaded and sent to the AI as context.

**Examples:**
```json
"ContextFolder": "../../context"                    // Relative path
"ContextFolder": "C:\\MyContext"                    // Absolute (Windows)
"ContextFolder": "/home/user/context"               // Absolute (Linux/Mac)
```

#### OutputFolder (Default: "../../output")
Location where generated code and validation reports will be saved.

**Examples:**
```json
"OutputFolder": "../../output"                      // Relative path
"OutputFolder": "C:\\GeneratedBots"                 // Absolute (Windows)
"OutputFolder": "/home/user/output"                 // Absolute (Linux/Mac)
```

## Path Resolution

### How Relative Paths Work

Relative paths are resolved from the **application's working directory**, which is typically `src/DataTransferHarness` when you run `dotnet run`.

**Example:**
```
Working Directory: C:\Projects\DataTransferHarness\src\DataTransferHarness
Config Setting:    "../../tickets"
Resolved Path:     C:\Projects\DataTransferHarness\tickets
```

### Directory Structure Example

```
DataTransferHarness/
├── src/
│   └── DataTransferHarness/
│       ├── appsettings.json          ← Configuration file
│       └── ... (other files)
├── tickets/                          ← ../../tickets from src/DataTransferHarness
├── context/                          ← ../../context from src/DataTransferHarness
└── output/                           ← ../../output from src/DataTransferHarness
```

## Security

### Protecting Your API Key

The `appsettings.json` file is **automatically ignored by git** (via `.gitignore`).

**Important:**
- ✓ **DO** keep `appsettings.example.json` in version control
- ✗ **DON'T** commit `appsettings.json` with real API keys
- ✓ **DO** share configuration structure in examples
- ✗ **DON'T** share actual API keys with anyone

### Multiple Environments

You can create environment-specific configurations:

```bash
# Development
appsettings.json

# Production
appsettings.production.json

# Testing
appsettings.test.json
```

Then modify `HarnessConfig.cs` to load the appropriate file based on an environment variable.

## Validation

The harness validates your configuration on startup:

### API Key Validation
- ✓ Must be present
- ✓ Cannot be the placeholder `"your-key-here"`
- ✓ Should start with `sk-ant-`

### Model Validation
- ✓ Must be present
- ✓ Must be a valid Claude model identifier

### Path Validation
- ✓ Tickets folder must exist
- ✓ At least one `.md` file must exist in tickets folder
- ✓ Context folder must exist
- ✓ Output folder will be created if it doesn't exist

## Troubleshooting

### Error: "Please set your Anthropic API key in appsettings.json"
**Solution:** Edit `appsettings.json` and replace `"your-key-here"` with your actual API key.

### Error: "No ticket (.md) files found"
**Solution:**
1. Check that your tickets folder exists
2. Verify there's at least one `.md` file in it
3. Confirm the `TicketsFolder` path in `appsettings.json` is correct

### Error: "Could not find file appsettings.json"
**Solution:**
1. Make sure you're running from `src/DataTransferHarness` directory
2. Copy `appsettings.example.json` to `appsettings.json`

### Paths Not Resolving Correctly
**Solution:**
1. Use absolute paths for debugging
2. Verify your working directory with `pwd` (Linux/Mac) or `cd` (Windows)
3. Check that relative paths are correct from the working directory

## Example Configurations

### Standard Setup (Default)
```json
{
  "Claude": {
    "ApiKey": "sk-ant-api03-...",
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

### Custom Absolute Paths
```json
{
  "Claude": {
    "ApiKey": "sk-ant-api03-...",
    "Model": "claude-3-7-sonnet-20250219",
    "MaxTokens": 8192
  },
  "Paths": {
    "TicketsFolder": "C:\\MyProject\\tickets",
    "ContextFolder": "C:\\MyProject\\context",
    "OutputFolder": "C:\\MyProject\\generated"
  }
}
```

### Quick Bots (Lower Token Count)
```json
{
  "Claude": {
    "ApiKey": "sk-ant-api03-...",
    "Model": "claude-3-5-sonnet-20241022",
    "MaxTokens": 2048
  },
  "Paths": {
    "TicketsFolder": "../../tickets",
    "ContextFolder": "../../context",
    "OutputFolder": "../../output"
  }
}
```
