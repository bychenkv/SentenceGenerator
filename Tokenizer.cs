using System.Text.RegularExpressions;

class Tokenizer
{
    const string NewlinePlaceholder = "§";

    const string NewlineOriginal = "\n\n";

    const string NewlinePattern = @"\r\n?|\n";

    const string ElipsisPattern = @"\.{3}";

    const string WordsPattern = @"[a-zA-Zа-яА-ЯёЁ]+";

    const string CompoundWordsPattern = $@"{WordsPattern}-{WordsPattern}";

    private static readonly string[] Punctuation = ["[", "]", "(", ")", "{", "}", "!", "?",
                                                    ".", ",", ":", ";", "'", "\"", "\\", "/",
                                                    "*", "&", "^", "%", "$", "_", "+", "-",
                                                    "–", "—", "=", "<", ">", "@", "|", "~", "§" ];

    public static List<string> Tokenize(string text)
    {
        var newlineReplacedText = Regex.Replace(text, NewlinePattern, NewlinePlaceholder);

        var punctuationPattern = string.Join("\\\\", Punctuation);
        var tokenizeRegex = new Regex($"({ElipsisPattern}|{CompoundWordsPattern}|{WordsPattern}|[{punctuationPattern}])");
        var tokens = tokenizeRegex.Split(newlineReplacedText);

        return tokens.Where(token => token.Length > 0).ToList();
    }

    public static string Join(IEnumerable<string> tokens)
    {
        var text = string.Join("", tokens);
        return Regex.Replace(text, NewlinePlaceholder, NewlineOriginal);
    }
}