using System;
using System.Collections.Generic;

namespace Cirno.AbstractSyntaxTree;

public sealed class BinaryOperatorNode : ExprNode
{
    public BinaryOperatorNode(in (int Line, int Col) position, ASTNode? parent, string name, ExprNode left, BinaryOperatorKind? opKind, ExprNode? right)
    {
        Position = position;
        Parent = parent;
        Name = name;
        Left = left;
        OpKind = opKind;
        Right = right;
    }

    public override string Name { get; }

    public override ASTNodeType NodeType => ASTNodeType.BinaryOperator;

    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }

    public ASTNode Left { get; }

    public BinaryOperatorKind? OpKind { get; }

    public ASTNode? Right { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        yield return Left;
        if (Right is not null)
            yield return Right;
    }

    public override void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name} : {OpKind}");

        Console.ForegroundColor = prevColor;
    }
}