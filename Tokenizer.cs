using System.Text.RegularExpressions;

// Summary:
//     A class for processing the source text: splitting into tokens, and vice versa.
static partial class Tokenizer
{
    // Regex patterns
    const string NewlinePlaceholder     = "§";
    const string NewlineOriginal        = "\n\n";
    const string NewlinePattern         = @"\r\n?|\n";
    const string ElipsisPattern         = @"\.{3}";
    const string WordsPattern           = @"[a-zA-Zа-яА-ЯёЁ]+";
    const string CompoundWordsPattern   = $@"{WordsPattern}-{WordsPattern}";

    // Punctuation characters
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

    // Summary:
    //     Splits an input text into a list of tokens
    //
    // Parameters:
    //   text:
    //     An input text
    //
    // Returns:
    //     A list of tokens into which the text is split

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

    // Summary:
    //     Combines a sequence of tokens into a string.
    //
    // Parameters:
    //   tokens:
    //     A sequence of tokens.
    //
    // Returns:
    //     A string combining a set of tokens
    public static string Join(IEnumerable<Token> tokens)
    {
        var text = string.Join("", tokens.Select(token => token.Content));

        return NewlineRegexBack().Replace(text, NewlineOriginal);
    }
}