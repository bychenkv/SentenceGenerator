class Generator
{
    public string SourceText { get; set; }

    public int Count { get; set; }

    public int SampleSize { get; set; }

    private readonly List<string> _generatedTokens;
    private readonly Dictionary<string, List<string>> _transitionMatrix;
    
    private readonly Random _random = new(); 

    public Generator(string sourceText, int count, int sampleSize = 2)
    {
        SourceText = sourceText;
        Count = count;
        SampleSize = sampleSize;

        var tokens = Tokenizer.Tokenize(sourceText);
        var samples = GetSamples(tokens);

        _transitionMatrix = GetTransitionMatrix(samples);
        _generatedTokens = InitializeSequence(null);
    }

    public IEnumerable<string> Generate()
    {
        while (true)
        {
            var prediction = PredictNext();

            if (_generatedTokens.Count <= Count)
                yield return prediction;
            else
                yield break;

            _generatedTokens.Add(prediction);
        }
    }

    private string PredictNext()
    {
        var lastPrediction = string.Join("", _generatedTokens.TakeLast(SampleSize));
        if (!_transitionMatrix.TryGetValue(lastPrediction, out List<string>? value))
            return "";
        var targets = value;

        var index = _random.Next(targets.Count);

        return targets[index];
    }

    private List<string> InitializeSequence(string? startText)
    {
        var keys = new List<string>(_transitionMatrix.Keys);
        var randomKey = keys[_random.Next(keys.Count)];
        var initial = startText ?? randomKey;

        return Tokenizer.Tokenize(initial);
    }

    private List<List<string>> GetSamples(List<string> tokens)
    {
        var samples = new List<List<string>>();

        for (var i = 0; i < tokens.Count - SampleSize + 1; i++)
        {
            var sample = new List<string>();

            for (int j = 0; j < SampleSize; j++)
                sample.Add(tokens[i + j]);

            samples.Add(sample);
        }

        return samples;
    }

    private static Dictionary<string, List<string>> GetTransitionMatrix(List<List<string>> samples)
    {
        var transitionMatrix = new Dictionary<string, List<string>>();

        foreach (var sample in samples)
        {
            var source = string.Join("", sample.GetRange(0, sample.Count - 1));
            var target = sample[^1];

            if (!transitionMatrix.ContainsKey(source))
                transitionMatrix[source] = [];
            
            transitionMatrix[source].Add(target);
        }

        return transitionMatrix;
    }
}