class Generator {
    const int SequenceLengthDefault = 200;
    const int SampleSizeDefault = 2;

    public string SourceText { get; set; }
    public int SequenceLength { get; set; }
    public int SampleSize { get; set; }

    private List<string> _generatedTokens;
    private readonly Dictionary<string, List<string>> _transitionMatrix;
    private readonly Random _random = new(); 

    public Generator(string sourceText,
                     int sequenceLength = SequenceLengthDefault, 
                     int sampleSize = SampleSizeDefault)
    {
        SourceText = sourceText;
        SequenceLength = sequenceLength;
        SampleSize = sampleSize;

        var tokens = Tokenizer.Tokenize(sourceText);
        var samples = GetSamples(tokens);

        _transitionMatrix = GetTransitionMatrix(samples);
        _generatedTokens = InitializeSequence();
    }

    public IEnumerable<string> Generate() {
        while (true)
        {
            var nextToken = MakeTransition();

            if (nextToken != null)
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

    private string? MakeTransition()
    {
        var source = string.Join("", _generatedTokens.TakeLast(SampleSize - 1));

        if (!_transitionMatrix.TryGetValue(source, out var targets))
            return null;

        return targets[_random.Next(targets.Count)];
    }

    private List<string> InitializeSequence()
    {
        var keys = new List<string>(_transitionMatrix.Keys);
        var randomKey = keys[_random.Next(keys.Count)];

        return Tokenizer.Tokenize(randomKey);
    }

    private List<List<string>> GetSamples(List<string> tokens)
    {
        var samples = new List<List<string>>();

        for (var i = 0; i < tokens.Count - SampleSize + 1; i++)
        {
            var sample = tokens.GetRange(i, SampleSize);
            samples.Add(sample);
        }

        return samples;
    }

    private static Dictionary<string, List<string>> GetTransitionMatrix(List<List<string>> samples)
    {
        var transitionMatrix = new Dictionary<string, List<string>>();

        foreach (var sample in samples)
        {
            var source = string.Join("", sample[..^1]);
            var target = sample[^1];

            if (!transitionMatrix.ContainsKey(source))
                transitionMatrix[source] = [];
            
            transitionMatrix[source].Add(target);
        }

        return transitionMatrix;
    }
}