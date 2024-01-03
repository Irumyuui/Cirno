using System;
using System.Collections.Generic;
using System.Linq;

namespace Cirno.AbstractSyntaxTree;

public sealed class IntegerLiteral<TValue> : LiteralNode
{
    public IntegerLiteral(in (int Line, int Col) position, ASTNode? parent, string name, LiteralType type, TValue value)
        : base(position, parent, name)
    {
        Type = type;
        Value = value;
    }

    public LiteralType Type { get; }

    public TValue Value { get; }

    public override ASTNodeType NodeType => ASTNodeType.IntegerLiteral;

    public override IEnumerable<ASTNode> GetChildren()
    {
        return Enumerable.Empty<ASTNode>();
    }

    public override void PrettyPrint()
    {
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{NodeType}");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name} = {Value} : {Type}");

        Console.ForegroundColor = prevColor;
    }
}