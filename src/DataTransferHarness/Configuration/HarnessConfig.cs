using Microsoft.Extensions.Configuration;

namespace DataTransferHarness.Configuration;

public class HarnessConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 4096;
    public string TicketPath { get; set; } = string.Empty;
    public string ContextPath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public string AgentsPath { get; set; } = string.Empty;

    public static HarnessConfig Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var apiKey = configuration["Claude:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "your-key-here")
        {
            throw new InvalidOperationException(
                "Please set your Anthropic API key in appsettings.json under Claude:ApiKey");
        }

        var modelName = configuration["Claude:Model"];
        if (string.IsNullOrWhiteSpace(modelName))
        {
            throw new InvalidOperationException(
                "Claude:Model is not configured in appsettings.json");
        }

        var maxTokens = int.TryParse(configuration["Claude:MaxTokens"], out var tokens) ? tokens : 4096;

        // Get path configurations
        var ticketsFolder = configuration["Paths:TicketsFolder"] ?? "../../tickets";
        var contextFolder = configuration["Paths:ContextFolder"] ?? "../../context";
        var outputFolder = configuration["Paths:OutputFolder"] ?? "../../output";
        var agentsFolder = configuration["Paths:AgentsFolder"] ?? "../../agents";

        // Resolve relative paths from the current directory
        var baseDirectory = Directory.GetCurrentDirectory();
        var ticketsPath = Path.GetFullPath(Path.Combine(baseDirectory, ticketsFolder));
        var contextPath = Path.GetFullPath(Path.Combine(baseDirectory, contextFolder));
        var outputPath = Path.GetFullPath(Path.Combine(baseDirectory, outputFolder));
        var agentsPath = Path.GetFullPath(Path.Combine(baseDirectory, agentsFolder));

        // Find the first .md file in tickets folder for the ticket path
        var ticketPath = string.Empty;
        if (Directory.Exists(ticketsPath))
        {
            var ticketFiles = Directory.GetFiles(ticketsPath, "*.md");
            if (ticketFiles.Length > 0)
            {
                ticketPath = ticketFiles[0]; // Use the first markdown file found
            }
        }

        if (string.IsNullOrWhiteSpace(ticketPath))
        {
            throw new InvalidOperationException(
                $"No ticket (.md) files found in: {ticketsPath}");
        }

        return new HarnessConfig
        {
            ApiKey = apiKey,
            ModelName = modelName,
            MaxTokens = maxTokens,
            TicketPath = ticketPath,
            ContextPath = contextPath,
            OutputPath = outputPath,
            AgentsPath = agentsPath
        };
    }
}
