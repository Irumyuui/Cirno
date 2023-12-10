using System.Collections.Generic;
using System.Linq;

using Cirno.Expressions;

namespace Cirno;

public class SyntaxToken(SyntaxKind kind, string name, int line, int position) : Expressions.ExpressionNode
{
    public override SyntaxKind Kind { get; } = kind;

    public string Name { get; } = name;

    public int Line { get; } = line;

    public int Position { get; } = position;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        // return Enumerable.Empty<ExpressionNode>();
        yield break;
    }

    public override string ToString() => $"{{[{Line}:{Position}]; Kind: {Kind}; Name: \"{Name}\";}}";

    // public bool Equals(SyntaxToken other) => Name == other.Name;

    // public override bool Equals(object? obj)
    // {
    //     return base.Equals(obj);
    // }

    public (int Line, int Col) GetTextPosition() => (Line, Position);
}

public class SyntaxTokenWithValue<TValue>(SyntaxKind kind, string name, int line, int position, TValue value) : SyntaxToken(kind, name, line, position) {
    public TValue Value { get; set; } = value;

    public override string ToString() => $"{{[{Line}:{Position}]; Kind: {Kind}; Name: \"{Name}\"; Value: {Value}}}";
}
