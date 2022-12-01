﻿using Ookii.CommandLine;

namespace ParserSample;

public static class Program
{
    // No need to use the Main(string[] args) overload (though you can if you want), because
    // CommandLineParser can take the arguments directly from Environment.GetCommandLineArgs().
    public static int Main()
    {
        // Parse the arguments. See ProgramArguments.cs for the definitions.
        var arguments = ProgramArguments.Parse();

        // No need to do anything when the value is null; Parse() already printed errors and
        // usage help to the console. We return a non-zero value to indicate failure.
        if (arguments == null)
        {
            return 1;
        }

        // We use the LineWrappingTextWriter to neatly white-space wrap console output.
        using var writer = LineWrappingTextWriter.ForConsoleOut();

        // Print the values of the arguments.
        writer.WriteLine("The following argument values were provided:");
        writer.WriteLine($"Source: {arguments.Source}");
        writer.WriteLine($"Destination: {arguments.Destination}");
        writer.WriteLine($"OperationIndex: {arguments.OperationIndex}");
        writer.WriteLine($"Date: {arguments.Date?.ToString() ?? "(null)"}");
        writer.WriteLine($"Count: {arguments.Count}");
        writer.WriteLine($"Verbose: {arguments.Verbose}");
        var values = arguments.Values == null ? "(null)" : "{ " + string.Join(", ", arguments.Values) + " }";
        writer.WriteLine($"Values: {values}");
        writer.WriteLine($"Day: {arguments.Day?.ToString() ?? "(null)"}");

        return 0;
    }
}