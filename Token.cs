// Summary:
//     A unit into which the text is divided
class Token(string content)
{
    // Summary:
    //     A sequence of characters itself
    public string Content { get; set; } = content;

    // Summary:
    //    A length of character sequence
    public int Length => Content.Length;

    public override bool Equals(object? obj)
    {
        if (obj is Token token)
            return token.Content.Equals(Content);
        
        return base.Equals(obj);
    }

    public override int GetHashCode() => HashCode.Combine(Content);
}