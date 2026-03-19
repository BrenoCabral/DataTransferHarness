using System.Text;
using DataTransferHarness.Agent;
using DataTransferHarness.Configuration;
using DataTransferHarness.Validation;

namespace DataTransferHarness.Output;

public class OutputWriter
{
    private readonly HarnessConfig _config;

    public OutputWriter(HarnessConfig config)
    {
        _config = config;
    }

    public async Task WriteOutputAsync(AgentResponse agentResponse, ValidationReport validationReport)
    {
        // Ensure output directory exists
        if (!Directory.Exists(_config.OutputPath))
        {
            Directory.CreateDirectory(_config.OutputPath);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        // Write the generated code
        var codePath = Path.Combine(_config.OutputPath, $"generated-bot-{timestamp}.ts");
        await File.WriteAllTextAsync(codePath, agentResponse.GeneratedCode);

        // Write the validation report
        var reportPath = Path.Combine(_config.OutputPath, $"validation-report-{timestamp}.txt");
        var reportContent = BuildReport(agentResponse, validationReport);
        await File.WriteAllTextAsync(reportPath, reportContent);

        Console.WriteLine($"Code written to: {codePath}");
        Console.WriteLine($"Report written to: {reportPath}");
    }

    private static string BuildReport(AgentResponse agentResponse, ValidationReport validationReport)
    {
        var sb = new StringBuilder();

        sb.AppendLine("=== Data Transfer Harness - Validation Report ===");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        sb.AppendLine("## Agent Response");
        sb.AppendLine($"Model: {agentResponse.Model}");
        sb.AppendLine($"Input Tokens: {agentResponse.UsageInputTokens}");
        sb.AppendLine($"Output Tokens: {agentResponse.UsageOutputTokens}");
        sb.AppendLine($"Code Length: {agentResponse.GeneratedCode.Length} characters");
        sb.AppendLine();

        sb.AppendLine("## Validation Results");
        sb.AppendLine(validationReport.ToString());
        sb.AppendLine();

        if (!validationReport.IsValid)
        {
            sb.AppendLine("⚠️  WARNING: Generated code did not pass validation!");
            sb.AppendLine("Please review the errors above before using this code.");
        }
        else if (validationReport.Warnings.Count > 0)
        {
            sb.AppendLine("⚠️  Code passed validation but has warnings.");
            sb.AppendLine("Please review the warnings above.");
        }
        else
        {
            sb.AppendLine("✓ Code passed all validation checks!");
        }

        return sb.ToString();
    }
}
