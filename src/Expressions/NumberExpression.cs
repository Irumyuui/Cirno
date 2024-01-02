using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class NumberExpression(SyntaxToken numberToken) : ExpressionNode
{
    public SyntaxToken NumberToken { get; private set; } = numberToken;

    public override SyntaxKind Kind { get; } = SyntaxKind.NumberExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return NumberToken;
    }

    // protected internal override ExpressionNode Accept(ExpressionVisitor visitor)
    // {
    //     return visitor.Visit(this);
    // }


}