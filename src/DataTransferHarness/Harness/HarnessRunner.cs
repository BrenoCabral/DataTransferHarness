using DataTransferHarness.Tickets;
using DataTransferHarness.Context;
using DataTransferHarness.Agent;
using DataTransferHarness.Validation;
using DataTransferHarness.Output;

namespace DataTransferHarness.Harness;

public class HarnessRunner
{
    private readonly TicketLoader _ticketLoader;
    private readonly ContextAssembler _contextAssembler;
    private readonly IAgentClient _agentClient;
    private readonly IOutputValidator _validator;
    private readonly OutputWriter _outputWriter;

    public HarnessRunner(
        TicketLoader ticketLoader,
        ContextAssembler contextAssembler,
        IAgentClient agentClient,
        IOutputValidator validator,
        OutputWriter outputWriter)
    {
        _ticketLoader = ticketLoader;
        _contextAssembler = contextAssembler;
        _agentClient = agentClient;
        _validator = validator;
        _outputWriter = outputWriter;
    }

    public async Task<HarnessResult> RunAsync()
    {
        Console.WriteLine("Step 1: Loading ticket...");
        var ticket = await _ticketLoader.LoadTicketAsync();
        Console.WriteLine($"✓ Loaded ticket: {ticket.Length} characters\n");

        Console.WriteLine("Step 2: Assembling context...");
        var context = await _contextAssembler.AssembleAsync();
        Console.WriteLine($"✓ Assembled {context.Count} context sources\n");

        Console.WriteLine("Step 3: Calling AI agent...");
        var agentResponse = await _agentClient.ExecuteAsync(ticket, context);
        Console.WriteLine($"✓ Received response: {agentResponse.GeneratedCode.Length} characters\n");

        Console.WriteLine("Step 4: Validating output...");
        var validationReport = _validator.Validate(agentResponse.GeneratedCode);
        Console.WriteLine($"✓ Validation complete: {(validationReport.IsValid ? "PASSED" : "FAILED")}\n");

        Console.WriteLine("Step 5: Writing output...");
        await _outputWriter.WriteOutputAsync(agentResponse, validationReport);
        Console.WriteLine($"✓ Output written\n");

        return new HarnessResult
        {
            Success = validationReport.IsValid,
            GeneratedCode = agentResponse.GeneratedCode,
            ValidationReport = validationReport
        };
    }
}
