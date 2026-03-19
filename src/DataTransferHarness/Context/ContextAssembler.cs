using DataTransferHarness.Configuration;

namespace DataTransferHarness.Context;

public class ContextAssembler
{
    private readonly HarnessConfig _config;

    public ContextAssembler(HarnessConfig config)
    {
        _config = config;
    }

    public async Task<List<ContextSource>> AssembleAsync()
    {
        var sources = new List<ContextSource>();

        if (!Directory.Exists(_config.ContextPath))
        {
            throw new DirectoryNotFoundException($"Context directory not found: {_config.ContextPath}");
        }

        // Load all files from the context directory
        var files = Directory.GetFiles(_config.ContextPath, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var content = await File.ReadAllTextAsync(file);
            var fileName = Path.GetFileName(file);
            var relativePath = Path.GetRelativePath(_config.ContextPath, file);

            sources.Add(new ContextSource
            {
                Name = relativePath,
                Content = content,
                Type = DetermineType(file)
            });
        }

        if (sources.Count == 0)
        {
            throw new InvalidOperationException("No context files found");
        }

        return sources;
    }

    private static string DetermineType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".md" => "markdown",
            ".json" => "json",
            ".ts" => "typescript",
            ".js" => "javascript",
            _ => "text"
        };
    }
}
