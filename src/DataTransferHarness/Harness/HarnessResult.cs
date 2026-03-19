using DataTransferHarness.Validation;

namespace DataTransferHarness.Harness;

public class HarnessResult
{
    public bool Success { get; set; }
    public string GeneratedCode { get; set; } = string.Empty;
    public ValidationReport ValidationReport { get; set; } = new();
}
