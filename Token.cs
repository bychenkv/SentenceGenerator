class Token(string content)
{
    public string Content { get; set; } = content;

    public int Length => Content.Length;

    public override bool Equals(object? obj)
    {
        if (obj is Token token)
            return token.Content.Equals(Content);
        
        return base.Equals(obj);
    }

    public override int GetHashCode() => HashCode.Combine(Content);
}