using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class TermExpression(ExpressionNode factor, ExpressionNode? termTail) : ExpressionNode
{
    public ExpressionNode Factor { get; } = factor;
    
    public ExpressionNode? TermTail { get; } = termTail;

    public override SyntaxKind Kind => SyntaxKind.TermExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Factor;
        if (TermTail is not null)
            yield return TermTail;
    }
}