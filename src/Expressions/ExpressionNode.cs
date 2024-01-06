using System;
using System.Collections.Generic;
using System.Linq;
using Cirno.Lexer;

namespace Cirno.Expressions;

public abstract class ExpressionNode {
    public abstract SyntaxKind Kind { get; }

    // protected internal virtual ExpressionNode VisitChildren(ExpressionVisitor visitor) {
    //     return visitor.Visit(this);
    // }

    // protected internal virtual ExpressionNode Accept(ExpressionVisitor visitor) {
    //     return visitor.VisitExtension(this);
    // }

    public abstract IEnumerable<ExpressionNode> GetChildren();

    // public static void Dump(ExpressionNode node) {
    //     Dump(node);
    // }

    public static void PrettyPrint(ExpressionNode node, string indent = "", bool isLast = true)
    {
        // ├─ │ └─

        var marker = isLast ? "└───" : "├───";
        // var marker = isLast ? "└" : "├";

        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(indent);
        Console.Write(marker);
        Console.ForegroundColor = prevColor;


        if (node is SyntaxToken token) {
            prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" " + node.Kind);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($" =>");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($" Word: \"{token.Name}\";");

            if (token is SyntaxTokenWithValue<int> t) {
                Console.Write($" Value: {t.Value};");
            }
            Console.Write($" In {token.Line}:{token.Position}");

            Console.ForegroundColor = prevColor;
        } else {
            Console.Write(" " + node.Kind);
            if (node is VariableExpression varNode && varNode.IsArrayRef) {
                Console.Write(" " + "IsArrayRef");
            }
        }

        Console.WriteLine();

        indent += isLast ? "    " : "│   ";
        // indent += isLast ? " " : "│";

        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            PrettyPrint(child, indent, child == lastChild);
    }
}

// public sealed class DeclarationExpression : ExpressionNode
// {


//     public override SyntaxKind Kind { get; } = SyntaxKind.DeclarationExpression;
// }

// wtf ExpressionStatementExpression

// public sealed class SimpleExpression(ExpressionNode additionExpr, SyntaxToken op, ExpressionNode termExpr) : ExpressionNode
// {
//     public ExpressionNode AdditionExpr { get; } = additionExpr;
//     public SyntaxToken Op { get; } = op;
//     public ExpressionNode TermExpr { get; } = termExpr;

//     public override SyntaxKind Kind => throw new System.NotImplementedException();

//     public override IEnumerable<ExpressionNode> GetChildren()
//     {
//         throw new System.NotImplementedException();
//     }
// }
