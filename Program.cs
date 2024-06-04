using Microsoft.Extensions.Configuration;

class Program
{
    private static int Main(string[] args)
    {
        var config = Configure(args);

        if (TryParseArgs(config, out int sentenceLength, out int sampleSize, out string sourceText))
        {
            var generator = new Generator(sourceText, sentenceLength, sampleSize);
            var generatedSequence = generator.Generate();
            var output = Tokenizer.Join(generatedSequence);

            Console.WriteLine("Generated output: ");
            Console.WriteLine(output);
        }

        return 0;
    }

    // Summary
    // ----------
    // Method that configures an application
    // ----------
    // Parameters
    // ----------
    // Input:
    //  string[] `args` - array with raw values of command line arguments
    // ----------
    // Return
    // ----------
    // IConfigurationRoot object which keeps info about command-line argument values
    // ----------
    private static IConfigurationRoot Configure(string[] args)
    {
        var builder = new ConfigurationBuilder();
        Dictionary<string, string> switchMappings = new()
        {
            { "--length", "sentenceLength" },
            { "--size", "sampleSize" },
            { "--file", "sourceFile" },
        };

        builder.AddCommandLine(args, switchMappings);

        return builder.Build();
    }

    // Summary
    // ----------
    // Method that tries to parse command-line args
    // Also prints an error message in console
    // ----------
    // Parameters
    // ----------
    // Input:
    //  IConfigurationRoot `config` - keeps raw command-line argument values
    // Output:
    //  int `sentenceLength`    - length of the sentence to be generated
    //  int `sampleSize`        - how many previous tokens to take into account to predict next token
    //  string `sourceText`     - text on the basis of which tokens are generated
    // ----------
    // Return
    // ----------
    // `true` if parsing was successful and `false` otherwise
    // ----------
    private static bool TryParseArgs(IConfigurationRoot config,
        out int sentenceLength, out int sampleSize, out string sourceText)
    {
        sampleSize = 0;
        sourceText = "";

        if (!int.TryParse(config["sentenceLength"], out sentenceLength))
        {
            Console.WriteLine("Use `--length` option to specify sentence length");
            return false;
        }

        if (!int.TryParse(config["sampleSize"], out sampleSize))
        {
            Console.WriteLine("Use `--size` option to specify sample size");
            return false;
        }

        return TryParseSourceFile(config, out sourceText);
    }

    private static bool TryParseSourceFile(IConfigurationRoot config, out string sourceText)
    {
        sourceText = "";

        if (config["sourceFile"] is null)
        {
            Console.WriteLine("Use `--file` option to specify source file");
            return false;
        }

        var pathToSourceFile = Path.Combine(Directory.GetCurrentDirectory(), config["sourceFile"]!);
        try
        {
            sourceText = File.ReadAllText(pathToSourceFile);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while reading source file:\n{e.Message}");
            return false;
        }

        if (sourceText.Length == 0)
        {
            Console.WriteLine("File with source text is empty!");
            return false;
        }

        return true;
    }
}