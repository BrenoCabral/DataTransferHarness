namespace DataTransferHarness.Validation;

public class ValidationReport
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    public override string ToString()
    {
        var lines = new List<string>
        {
            $"Validation: {(IsValid ? "PASSED" : "FAILED")}"
        };

        if (Errors.Count > 0)
        {
            lines.Add("\nErrors:");
            lines.AddRange(Errors.Select(e => $"  - {e}"));
        }

        if (Warnings.Count > 0)
        {
            lines.Add("\nWarnings:");
            lines.AddRange(Warnings.Select(w => $"  - {w}"));
        }

        return string.Join("\n", lines);
    }
}
