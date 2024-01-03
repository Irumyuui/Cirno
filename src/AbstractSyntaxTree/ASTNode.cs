using System;
using System.Collections.Generic;
using System.Linq;

namespace Cirno.AbstractSyntaxTree;

public abstract class ASTNode
{
    public abstract string Name { get; }
    public abstract ASTNodeType NodeType { get; }
    // public abstract ASTNode Parameters { get; protected set; }

    public abstract ASTNode? Parent { get; protected internal set; }

    public abstract IEnumerable<ASTNode> GetChildren();

    public virtual void CodeGen() { }

    public static void PrettyPrint(ASTNode node, string indent = "", bool isLast = true)
    {
        // ├─ │ └─

        var marker = isLast ? "└───" : "├───";
        // var marker = isLast ? "└" : "├";

        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(indent);
        Console.Write(marker);
        Console.ForegroundColor = prevColor;

        Console.Write(" ");
        node.PrettyPrint();

        Console.WriteLine();

        indent += isLast ? "    " : "│   ";
        // indent += isLast ? " " : "│";

        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            PrettyPrint(child, indent, child == lastChild);
    }

    // public override string ToString() => $"{NodeType}";

    // public abstract void PrettyPrint();
    public virtual void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        // Console.ForegroundColor = ConsoleColor.DarkYellow;
        // Console.Write(" => ");

        Console.ForegroundColor = prevColor;
    }

    // protected internal virtual ASTVisitor Accept(ASTVisitor visitor) {
           
    // }
}