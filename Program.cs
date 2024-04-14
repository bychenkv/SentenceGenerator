internal class Program
{
    private static void Main(string[] args)
    {
        var path = @"/Users/mac/Projects/dotnet/markov-chain-generator/source.txt";
        string text = File.ReadAllText(path);

        Console.WriteLine("Enter number of words to generate: ");
        var sentenceLength = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter sample size: ");
        var sampleSize = Convert.ToInt32(Console.ReadLine());

        var generator = new Generator(text, sentenceLength, sampleSize);
        var generatedSequence = generator.Generate();
        var output = Tokenizer.Join(generatedSequence);

        Console.WriteLine("Generated output: ");
        Console.WriteLine(output);
    }
}