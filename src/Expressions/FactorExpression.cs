using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class FactorExpression(ExpressionNode expr) : ExpressionNode
{
    public ExpressionNode Expr { get; } = expr;

    public override SyntaxKind Kind => SyntaxKind.FactorExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Expr;
    }
}