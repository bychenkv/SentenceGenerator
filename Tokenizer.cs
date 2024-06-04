using System.Text.RegularExpressions;

static partial class Tokenizer
{
    const string NewlinePlaceholder     = "§";
    const string NewlineOriginal        = "\n\n";
    const string NewlinePattern         = @"\r\n?|\n";
    const string ElipsisPattern         = @"\.{3}";
    const string WordsPattern           = @"[a-zA-Zа-яА-ЯёЁ]+";
    const string CompoundWordsPattern   = $@"{WordsPattern}-{WordsPattern}";

    private static readonly string[] Punctuation =
        [
            "[", "]", "(", ")", "{", "}", "!", "?",
            ".", ",", ":", ";", "'", "\"", "\\", "/",
            "*", "&", "^", "%", "$", "_", "+", "-",
            "–", "—", "=", "<", ">", "@", "|", "~", "§"
        ];

    [GeneratedRegex(NewlinePattern)]
    private static partial Regex NewlineRegex();

    [GeneratedRegex(NewlinePlaceholder)]
    private static partial Regex NewlineRegexBack();

    // Method that splits an input text into a list of tokens
    public static List<Token> Tokenize(string? text)
    {
        if (text == null)
            return [];

        var newlineReplacedText = NewlineRegex().Replace(text, NewlinePlaceholder);

        var punctuationPattern = string.Join("\\\\", Punctuation);
        var tokenizeRegex = new Regex(
            $"({ElipsisPattern}|{CompoundWordsPattern}|{WordsPattern}|[{punctuationPattern}])");

        var tokens = tokenizeRegex.Split(newlineReplacedText).Select(token => new Token(token));

        return tokens.Where(token => token.Length > 0).ToList();
    }

    // Method that that combines a list of tokens into a string 
    public static string Join(IEnumerable<Token> tokens)
    {
        var text = string.Join("", tokens.Select(token => token.Content));

        return NewlineRegexBack().Replace(text, NewlineOriginal);
    }
}