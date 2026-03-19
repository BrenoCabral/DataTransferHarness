namespace DataTransferHarness.Validation;

public interface IOutputValidator
{
    ValidationReport Validate(string generatedCode);
}
