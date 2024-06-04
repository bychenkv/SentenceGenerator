class Generator
{
    const int SequenceLengthDefault = 200;
    const int SampleSizeDefault = 2;

    private List<Token> _generatedTokens;
    private readonly Dictionary<Token, List<Token>> _transitionMatrix;
    private readonly Random _random = new(); 

    public string SourceText { get; set; }
    public int SequenceLength { get; set; }
    public int SampleSize { get; set; }

    public Generator(
        string sourceText, int sequenceLength = SequenceLengthDefault, int sampleSize = SampleSizeDefault)
    {
        SourceText = sourceText;
        SequenceLength = sequenceLength;
        SampleSize = sampleSize;

        var tokens = Tokenizer.Tokenize(sourceText);
        var samples = GetSamples(tokens);

        _transitionMatrix = GetTransitionMatrix(samples);
        _generatedTokens = InitializeSequence();
    }

    public IEnumerable<Token> Generate()
    {
        while (true)
        {
            var nextToken = MakeTransition();

            if (nextToken is not null)
            {
                if (_generatedTokens.Count <= SequenceLength)
                    yield return nextToken;
                else
                    yield break;

                _generatedTokens.Add(nextToken);
            }
            else
            {
                if (_generatedTokens.Count >= SampleSize)
                    _generatedTokens.RemoveAt(_generatedTokens.Count - 1);
                else
                    _generatedTokens = InitializeSequence();
            }   
        }
    }

    private Token? MakeTransition()
    {
        var contents = _generatedTokens.TakeLast(SampleSize - 1).Select(token => token.Content);
        var source = new Token(string.Join("", contents));

        if (!_transitionMatrix.ContainsKey(source))
            return null;
            
        var targets = _transitionMatrix[source];
        
        return targets[_random.Next(targets.Count)];
    }

    private List<Token> InitializeSequence()
    {
        var keys = new List<Token>(_transitionMatrix.Keys);
        var randomKey = keys.Count > 0 ? keys[_random.Next(keys.Count)] : null;

        return Tokenizer.Tokenize(randomKey?.Content);
    }

    private List<List<Token>> GetSamples(List<Token> tokens)
    {
        var samples = new List<List<Token>>();
        
        for (var i = 0; i < tokens.Count - SampleSize + 1; i++)
        {
            var sample = tokens.GetRange(i, SampleSize);
            samples.Add(sample);
        }

        return samples;
    }

    private static Dictionary<Token, List<Token>> GetTransitionMatrix(List<List<Token>> samples)
    {
        var transitionMatrix = new Dictionary<Token, List<Token>>();

        foreach (var sample in samples)
        {
            var contents = sample.SkipLast(1).Select(token => token.Content);
            var source = new Token(string.Join("", contents));
            var target = sample[^1];

            if (!transitionMatrix.ContainsKey(source))
                transitionMatrix[source] = [];
            
            transitionMatrix[source].Add(target);
        }

        return transitionMatrix;
    }
}