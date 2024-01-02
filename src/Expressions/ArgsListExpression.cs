using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class ArgsListExpression(ExpressionNode expr, ExpressionNode? argsListTail) : ExpressionNode
{
    public ExpressionNode Expr { get; } = expr;

    public ExpressionNode? ArgsListTail { get; } = argsListTail;

    public override SyntaxKind Kind => SyntaxKind.ArgsListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Expr;
        if (ArgsListTail is not null)
            yield return ArgsListTail;
    }
}