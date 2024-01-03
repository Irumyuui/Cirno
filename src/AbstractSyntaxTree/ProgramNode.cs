using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cirno.AbstractSyntaxTree;

public sealed class ProgramNode : ASTNode
{
    public ProgramNode(ImmutableArray<DeclarationNode> declarationNodes)
    {
        DeclarationNodes = declarationNodes;
        Parent = this;
    }

    public override string Name { get; } = "Program";

    public override ASTNodeType NodeType => ASTNodeType.ProgramNode;

    public override ASTNode? Parent { get; protected internal set; }

    public ImmutableArray<DeclarationNode> DeclarationNodes { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        foreach (var item in DeclarationNodes)
            yield return item;
    }

    protected override void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        // Console.ForegroundColor = ConsoleColor.DarkYellow;
        // Console.Write(" => ");

        Console.ForegroundColor = prevColor;
    }
}