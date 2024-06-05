using Microsoft.Extensions.Configuration;

class Program {
    private static int Main(string[] args) {
        var config = Configure(args);

        if (TryParseArgs(config,
            out int sentenceLength, out int sampleSize, out string sourceText)
        ) {
            var generator = new Generator(sourceText, sentenceLength, sampleSize);
            var generatedSequence = generator.Generate();
            var output = Tokenizer.Join(generatedSequence);

            Console.WriteLine("Generated output: ");
            Console.WriteLine(output);
        }

        return 0;
    }

    // Summary:
    //     Configures an application: processes command-line arguments.
    //
    // Parameters:
    //   args:
    //     An array with raw values of command line arguments.
    //
    // Returns:
    //     A config with raw command-line argument values.
    private static IConfigurationRoot Configure(string[] args) {
        var builder = new ConfigurationBuilder();
        Dictionary<string, string> switchMappings = new() {
            { "--length", "sentenceLength" },
            { "--size", "sampleSize" },
            { "--file", "sourceFile" },
        };

        builder.AddCommandLine(args, switchMappings);

        return builder.Build();
    }

    // Summary:
    //     Tries to parse command-line arguments.
    //     Also prints an error message in console.
    //
    // Parameters:
    //   config:
    //     Keeps raw command-line argument values.
    //   (out) sentenceLength:
    //     A length of the sentence to be generated.
    //   (out) sampleSize:
    //     A number of previous tokens to take into account to predict next token.
    //   (out) sourceText:
    //     A text on the basis of which tokens are generated.
    //
    // Returns:
    //     true if parsing was successful; false otherwise.
    private static bool TryParseArgs(IConfigurationRoot config,
        out int sentenceLength, out int sampleSize, out string sourceText
    ) {
        sampleSize = 0;
        sourceText = "";

        if (!int.TryParse(config["sentenceLength"], out sentenceLength)) {
            Console.WriteLine("Use `--length` option to specify sentence length");
            return false;
        }

        if (!int.TryParse(config["sampleSize"], out sampleSize)) {
            Console.WriteLine("Use `--size` option to specify sample size");
            return false;
        }

        return TryParseSourceFile(config, out sourceText);
    }

    private static bool TryParseSourceFile(IConfigurationRoot config, out string sourceText) {
        sourceText = "";

        if (config["sourceFile"] is null) {
            Console.WriteLine("Use `--file` option to specify source file");
            return false;
        }

        var sourceFile = config["sourceFile"]!;
        var currentDirectory = Directory.GetCurrentDirectory();
        var pathToSourceFile = Path.Combine(currentDirectory, "samples", sourceFile);
        try {
            sourceText = File.ReadAllText(pathToSourceFile);
        } catch (Exception e) {
            Console.WriteLine($"Error while reading source file:\n{e.Message}");
            return false;
        }

        if (sourceText.Length == 0) {
            Console.WriteLine("File with source text is empty!");
            return false;
        }

        return true;
    }
}