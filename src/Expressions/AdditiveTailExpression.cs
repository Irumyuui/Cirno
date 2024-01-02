using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class AdditiveTailExpression(SyntaxToken op, ExpressionNode term, ExpressionNode? additiveExpressionTail) : ExpressionNode
{
    public SyntaxToken Op { get; } = op;
    public ExpressionNode Term { get; } = term;
    public ExpressionNode? AdditiveExpressionTail { get; } = additiveExpressionTail;

    public override SyntaxKind Kind => SyntaxKind.AdditiveTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Op;
        yield return Term;
        if (AdditiveExpressionTail is not null)
            yield return AdditiveExpressionTail;
    }
}