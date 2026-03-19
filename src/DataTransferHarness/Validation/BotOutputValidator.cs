using System.Text.RegularExpressions;

namespace DataTransferHarness.Validation;

public class BotOutputValidator : IOutputValidator
{
    public ValidationReport Validate(string generatedCode)
    {
        var report = new ValidationReport();

        if (string.IsNullOrWhiteSpace(generatedCode))
        {
            report.Errors.Add("Generated code is empty");
            return report;
        }

        // Check for export default
        if (!Regex.IsMatch(generatedCode, @"export\s+default\s+"))
        {
            report.Errors.Add("Missing 'export default' statement");
        }

        // Check for handler function
        if (!Regex.IsMatch(generatedCode, @"(async\s+)?function\s+\w+\s*\(|const\s+\w+\s*=\s*(async\s+)?\("))
        {
            report.Warnings.Add("No function definition found");
        }

        // Check for BotEvent or MedplumClient usage (common Medplum patterns)
        if (!generatedCode.Contains("BotEvent") && !generatedCode.Contains("MedplumClient"))
        {
            report.Warnings.Add("No BotEvent or MedplumClient usage detected - may not be a valid Medplum bot");
        }

        // Check for basic TypeScript syntax issues
        var openBraces = generatedCode.Count(c => c == '{');
        var closeBraces = generatedCode.Count(c => c == '}');
        if (openBraces != closeBraces)
        {
            report.Errors.Add($"Mismatched braces: {openBraces} opening, {closeBraces} closing");
        }

        var openParens = generatedCode.Count(c => c == '(');
        var closeParens = generatedCode.Count(c => c == ')');
        if (openParens != closeParens)
        {
            report.Errors.Add($"Mismatched parentheses: {openParens} opening, {closeParens} closing");
        }

        // Check minimum code length (arbitrary but reasonable)
        if (generatedCode.Length < 100)
        {
            report.Warnings.Add("Generated code seems unusually short");
        }

        report.IsValid = report.Errors.Count == 0;

        return report;
    }
}
