using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cirno.AbstractSyntaxTree;

public sealed class CallFunctionNode : ExprNode
{
    public CallFunctionNode(in (int Line, int Col) position, ASTNode? parent, string name, ImmutableArray<ExprNode> args)
    {
        Position = position;
        Parent = parent;
        Name = name;
        Args = args;
    }

    public override string Name { get; }

    public ImmutableArray<ExprNode> Args { get; }

    public override ASTNodeType NodeType => ASTNodeType.CallFunction;

    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }


    public override IEnumerable<ASTNode> GetChildren()
    {
        foreach (var item in Args)
            yield return item;
    }

    protected override void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name}");

        Console.ForegroundColor = prevColor;
    }
}