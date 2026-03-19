using DataTransferHarness.Configuration;

namespace DataTransferHarness.Tickets;

public class TicketLoader
{
    private readonly HarnessConfig _config;

    public TicketLoader(HarnessConfig config)
    {
        _config = config;
    }

    public async Task<string> LoadTicketAsync()
    {
        if (!File.Exists(_config.TicketPath))
        {
            throw new FileNotFoundException($"Ticket file not found: {_config.TicketPath}");
        }

        var content = await File.ReadAllTextAsync(_config.TicketPath);

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("Ticket file is empty");
        }

        return content;
    }
}
