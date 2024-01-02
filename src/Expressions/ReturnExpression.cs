using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class ReturnExpression(ExpressionNode? expression = null) : ExpressionNode
{
    public ExpressionNode? Expression { get; } = expression;

    public override SyntaxKind Kind => SyntaxKind.ReturnExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}