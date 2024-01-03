using System;
using System.Collections.Generic;

namespace Cirno.AbstractSyntaxTree;

public class LiteralNode : ExprNode
{
    public LiteralNode(in (int Line, int Col) position, ASTNode? parent, string name)
    {
        Position = position;
        Parent = parent;
        Name = name;
    }

    public override string Name { get; }

    public override ASTNodeType NodeType => ASTNodeType.LiteralExpression;

    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        // yield return (ASTNode)Enumerable.Empty<ASTNode>();
        yield break;
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
        Console.Write(Name);

        Console.ForegroundColor = prevColor;
    }
}