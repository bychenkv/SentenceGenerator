// Summary:
//     Performs the generation of a sequence of tokens,
//     as well as the construction of a transition matrix.
class Generator {
    const int SequenceLengthDefault = 200;
    const int SampleSizeDefault = 2;

    private List<Token> _generatedTokens;
    private readonly Dictionary<Token, List<Token>> _transitionMatrix;
    private readonly Random _random = new(); 

    // Summary:
    //     A text on the basis of which tokens are generated.
    public string SourceText { get; set; }

    // Summary:
    //     A number of tokens to be generated.
    public int SequenceLength { get; set; }

    // Summary:
    //     A number of previous tokens to take into account to predict next token.
    public int SampleSize { get; set; }

    public Generator(
        string sourceText,
        int sequenceLength = SequenceLengthDefault,
        int sampleSize = SampleSizeDefault
    ) {
        SourceText = sourceText;
        SequenceLength = sequenceLength;
        SampleSize = sampleSize;

        var tokens = Tokenizer.Tokenize(sourceText);
        var samples = GetSamples(tokens);

        _transitionMatrix = BuildTransitionMatrix(samples);
        _generatedTokens = InitializeSequence();
    }

    // Summary:
    //     Generates a sequence of tokens of the required length.
    //
    // Returns:
    //     An enumerator containing the generated sequence.
    public IEnumerable<Token> Generate() {
        while (true) {
            var nextToken = MakeTransition();

            if (nextToken is not null) {
                if (_generatedTokens.Count <= SequenceLength)
                    yield return nextToken;
                else
                    yield break;

                _generatedTokens.Add(nextToken);
            } else if (_generatedTokens.Count >= SampleSize)
                _generatedTokens.RemoveAt(_generatedTokens.Count - 1);
            else
                _generatedTokens = InitializeSequence();
        }
    }

    // Summary:
    //     Predicts the next token based on the previous ones.
    //
    // Returns:
    //     The next token to generate.
    private Token? MakeTransition() {
        var source = new Token(_generatedTokens.TakeLast(SampleSize - 1));

        if (!_transitionMatrix.ContainsKey(source))
            return null;
            
        var targets = _transitionMatrix[source];
        
        return targets[_random.Next(targets.Count)];
    }

    // Summary:
    //     Initializes the sequence with random tokens.
    //
    // Returns:
    //     A list of `sampleSize - 1` tokens.
    private List<Token> InitializeSequence() {
        var keys = new List<Token>(_transitionMatrix.Keys);
        var randomKey = keys.Count > 0 ? keys[_random.Next(keys.Count)] : null;

        return Tokenizer.Tokenize(randomKey?.Content);
    }

    // Summary:
    //     Splits a list of tokens into samples through a sliding window of the required size.
    //
    // Parameters:
    //   tokens:
    //     A list of tokens.
    //
    // Returns:
    //     A list of samples.
    private List<List<Token>> GetSamples(List<Token> tokens) {
        var samples = new List<List<Token>>();
        
        for (var i = 0; i < tokens.Count - SampleSize + 1; i++) {
            var sample = tokens.GetRange(i, SampleSize);
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
    private static Dictionary<Token, List<Token>> BuildTransitionMatrix(List<List<Token>> samples) {
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