using System;
using System.Collections.Generic;
using System.Linq;
using Cirno.CodeGen;
using LLVMSharp.Interop;

namespace Cirno.AbstractSyntaxTree;

public abstract class ASTNode : ICodeGenVisitable
{
    public abstract string Name { get; }
    public abstract ASTNodeType NodeType { get; }
    public abstract ASTNode? Parent { get; protected internal set; }
    public abstract IEnumerable<ASTNode> GetChildren();

    public static void PrettyPrint(ASTNode node, string indent = "", bool isLast = true)
    {
        // ├─ │ └─

        var marker = isLast ? "└───" : "├───";
        
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{indent}{marker}");
        Console.ForegroundColor = prevColor;

        Console.Write(" ");
        node.PrettyPrint();

        Console.WriteLine();

        indent += isLast ? "    " : "│   ";
        
        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            PrettyPrint(child, indent, child == lastChild);
    }

    protected virtual void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = prevColor;
    }

    public virtual LLVMValueRef? Accept(ICodeGenVisitor visitor, LLVMBasicBlockRef? entryBasicBlock,
        LLVMBasicBlockRef? exitBasicBlock)
        => visitor.Visit(this, entryBasicBlock, exitBasicBlock);
}