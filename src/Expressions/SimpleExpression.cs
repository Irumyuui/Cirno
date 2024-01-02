using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class SimpleExpression(ExpressionNode additionExpr1, SyntaxToken? op = null, ExpressionNode? additionExpr2 = null) : ExpressionNode
{
    public ExpressionNode AdditionExpr1 { get; } = additionExpr1;

    public SyntaxToken? Op { get; } = op;

    public ExpressionNode? AdditionExpr2 { get; } = additionExpr2;

    public override SyntaxKind Kind => SyntaxKind.SimpleExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return AdditionExpr1;
        if (Op is not null)
            yield return Op;
        if (AdditionExpr2 is not null)
            yield return AdditionExpr2;
    }
}