# Quick Start Guide

## Get Running in 4 Steps

### 1. Copy Configuration Template

```bash
cd src/DataTransferHarness
cp appsettings.example.json appsettings.json
```

### 2. Set Your API Key

Edit `appsettings.json` and replace `your-api-key-here` with your actual Anthropic API key:

```json
{
  "Claude": {
    "ApiKey": "sk-ant-your-actual-key-here",
    "Model": "claude-3-7-sonnet-20250219",
    "MaxTokens": 4096
  }
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Harness

```bash
dotnet run
```

## What Happens Next?

The harness will:
1. Load the ticket from `tickets/patient-import-pipeline.md`
2. Gather context files from `context/`
3. Send everything to Claude AI
4. Generate a Medplum bot (TypeScript)
5. Validate the generated code
6. Save results to `output/`

Check the `output/` directory for:
- `generated-bot-{timestamp}.ts` - Your generated bot code
- `validation-report-{timestamp}.txt` - Validation results

## Example Output

```
=== Data Transfer Harness ===

Step 1: Loading ticket...
✓ Loaded ticket: 1243 characters

Step 2: Assembling context...
✓ Assembled 3 context sources

Step 3: Calling AI agent...
✓ Received response: 2156 characters

Step 4: Validating output...
✓ Validation complete: PASSED

Step 5: Writing output...
Code written to: output/generated-bot-20260319_143052.ts
Report written to: output/validation-report-20260319_143052.txt
✓ Output written

=== Harness Complete ===
Status: SUCCESS
Output written to: C:\...\output
```

## Next Steps

### Customize the Ticket
Edit `tickets/patient-import-pipeline.md` to define your own task.

### Add More Context
Drop any `.md`, `.json`, or `.ts` files into `context/` to provide more examples or documentation.

### Review Generated Code
Open the generated TypeScript file in your editor and review the bot logic.

### Deploy to Medplum
Copy the generated code into your Medplum bot editor and test it.

## Troubleshooting

**Error: Please set your Anthropic API key in appsettings.json**
- Make sure you copied `appsettings.example.json` to `appsettings.json`
- Edit `appsettings.json` and replace the placeholder with your actual API key
- The key must start with `sk-ant-`

**Error: No ticket (.md) files found**
- Ensure at least one `.md` file exists in the `tickets/` folder
- Check the `Paths:TicketsFolder` setting in `appsettings.json`
- Verify you're running from the correct directory (`src/DataTransferHarness`)

**Validation Failed**
- Check the validation report in `output/`
- Review the errors and warnings
- The code may still be usable even with warnings

## Questions?

See the full [README.md](README.md) for detailed documentation.
