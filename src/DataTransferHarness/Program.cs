using DataTransferHarness.Configuration;
using DataTransferHarness.Harness;
using DataTransferHarness.Tickets;
using DataTransferHarness.Context;
using DataTransferHarness.Agent;
using DataTransferHarness.Validation;
using DataTransferHarness.Output;

namespace DataTransferHarness;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=== Data Transfer Harness ===\n");

        try
        {
            // Load configuration
            var config = HarnessConfig.Load();

            // Initialize components
            var ticketLoader = new TicketLoader(config);
            var contextAssembler = new ContextAssembler(config);
            var agentClient = new MedplumTypeScriptAgent(config);
            var validator = new BotOutputValidator();
            var outputWriter = new OutputWriter(config);

            // Create and run the harness
            var runner = new HarnessRunner(
                ticketLoader,
                contextAssembler,
                agentClient,
                validator,
                outputWriter
            );

            var result = await runner.RunAsync();

            // Display results
            Console.WriteLine($"\n=== Harness Complete ===");
            Console.WriteLine($"Status: {(result.Success ? "SUCCESS" : "FAILED")}");
            Console.WriteLine($"Output written to: {config.OutputPath}");

            return result.Success ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
    }
}
