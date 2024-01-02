using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class AdditiveExpression(ExpressionNode term, ExpressionNode? additiveExpressionTail = null) : ExpressionNode
{
    public ExpressionNode Term { get; } = term;
    public ExpressionNode? AdditiveExpressionTail { get; } = additiveExpressionTail;

    public override SyntaxKind Kind => SyntaxKind.AdditiveExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Term;
        if (AdditiveExpressionTail is not null)
            yield return AdditiveExpressionTail;
    }
}