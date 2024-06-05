// Summary:
//     A unit into which the text is divided
class Token {
    // Summary:
    //     A sequence of characters itself
    public string Content { get; set; }

    // Summary:
    //    A length of character sequence
    public int Length => Content.Length;

    public Token(string content) {
        Content = content;
    }

    public Token(IEnumerable<Token> tokens) {
        Content = string.Join("", tokens);
    }

    public override bool Equals(object? obj) {
        if (obj is Token token)
            return token.Content.Equals(Content);
        
        return base.Equals(obj);
    }

    public override int GetHashCode() => HashCode.Combine(Content);

    public override string ToString() => Content;
}