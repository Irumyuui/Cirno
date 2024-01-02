using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class ArgsListTailExpression(ExpressionNode? expr, ExpressionNode? argsListTail) : ExpressionNode
{
    public ExpressionNode? Expr { get; } = expr;

    public ExpressionNode? ArgsListTail { get; } = argsListTail;

    public override SyntaxKind Kind => SyntaxKind.ArgsListTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (Expr is not null)
            yield return Expr;
        if (ArgsListTail is not null)
            yield return ArgsListTail;
    }
}