static class GeneratorUtils {
    // Summary:
    //     Splits a list of tokens into samples through a sliding window of the required size.
    //
    // Parameters:
    //   tokens:
    //     A list of tokens.
    //
    // Returns:
    //     A list of samples.
    public static List<List<Token>> GetSamples(List<Token> tokens, int sampleSize) {
        var samples = new List<List<Token>>();
        
        for (var i = 0; i < tokens.Count - sampleSize + 1; i++) {
            var sample = tokens.GetRange(i, sampleSize);
            samples.Add(sample);
        }

        return samples;
    }

    // Summary:
    //     Constructs a transition matrix to predict the next token.
    //
    // Parameters:
    //   samples:
    //     A list of samples.
    //
    // Returns:
    //     A transition matrix.
    public static Dictionary<Token, List<Token>> BuildTransitionMatrix(List<List<Token>> samples) {
        var transitionMatrix = new Dictionary<Token, List<Token>>();

        foreach (var sample in samples) {
            var source = new Token(sample.SkipLast(1));
            var target = sample[^1];

            if (!transitionMatrix.ContainsKey(source))
                transitionMatrix[source] = [];
            
            transitionMatrix[source].Add(target);
        }

        return transitionMatrix;
    }
}