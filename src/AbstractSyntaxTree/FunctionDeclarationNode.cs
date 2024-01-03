using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Cirno.AbstractSyntaxTree;

public sealed class FunctionDeclarationNode : DeclarationNode
{
    public FunctionDeclarationNode(in (int Line, int Col) position,
        ASTNode? parent,
        string name,
        ImmutableArray<LiteralType> functionType,
        ImmutableArray<(string Name, LiteralType Type)> parameters,
        CompoundStatementNode body
    )
    {
        Position = position;
        Parent = parent;
        Name = name;
        FunctionType = functionType;
        Parameters = parameters;
        Body = body;
    }

    public override ASTNodeType NodeType => ASTNodeType.FunctionDeclaration;

    public ImmutableArray<LiteralType> FunctionType { get; private set; }

    public ImmutableArray<(string Name, LiteralType Type)> Parameters { get; private set; }

    public CompoundStatementNode Body { get; }

    public override string Name { get; }
    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        yield return Body;
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
        Console.Write($"{Name} -> {FunctionType.First()}::({string.Join(',', Parameters.Select(item => $"{item.Name}:{item.Type}"))})");

        Console.ForegroundColor = prevColor;
    }
}