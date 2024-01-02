using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class TermTailExpression(SyntaxToken op, ExpressionNode factor, ExpressionNode? termTail) : ExpressionNode
{
    public SyntaxToken Op { get; } = op;

    public ExpressionNode Factor { get; } = factor;

    public ExpressionNode? TermTail { get; } = termTail;

    public override SyntaxKind Kind => SyntaxKind.TermTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Op;
        yield return Factor;
        if (TermTail is not null)
            yield return TermTail;
    }
}