using System;
using System.Collections.Generic;
using Cirno.CodeGen;
using LLVMSharp.Interop;

namespace Cirno.AbstractSyntaxTree;

public sealed class VariableDeclarationNode : DeclarationNode
{
    public VariableDeclarationNode(in (int Line, int Col) position, ASTNode? parent, string name, LiteralType type, int? arrayLength = null)
    {
        Position = position;
        Parent = parent;
        Name = name;
        Type = type;
        ArrayLength = arrayLength;
    }

    public override ASTNodeType NodeType => ASTNodeType.VariableDeclaration;

    public (int Line, int Col) Position { get; }

    public override ASTNode? Parent { get; protected internal set; }

    public override string Name { get; }

    public LiteralType Type { get; }

    /// <summary>
    /// It's not null when the variable is array type.
    /// </summary>
    /// <value></value>
    public int? ArrayLength { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        // yield return Enumerable.Empty<ASTNode>();
        yield break;
    }

    public bool IsArrayType() => Type is LiteralType.IntPtr;

    protected override void PrettyPrint()
    {
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{NodeType}");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name} : {Type}");
        if (ArrayLength is not null)
        {
            Console.Write($" [{ArrayLength}]");
        }

        Console.ForegroundColor = prevColor;
    }
    
    public override LLVMValueRef? Accept(ICodeGenVisitor visitor)
        => visitor.Visit(this);
}