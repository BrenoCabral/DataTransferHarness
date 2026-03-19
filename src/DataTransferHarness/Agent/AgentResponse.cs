namespace DataTransferHarness.Agent;

public class AgentResponse
{
    public string GeneratedCode { get; set; } = string.Empty;
    public string RawResponse { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int UsageInputTokens { get; set; }
    public int UsageOutputTokens { get; set; }
}
