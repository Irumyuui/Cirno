using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class DeclarationListExpression(ExpressionNode declaration, ExpressionNode? declarationListTail) : ExpressionNode
{
    public ExpressionNode Declaration { get; } = declaration;
    public ExpressionNode? DeclarationListTail { get; } = declarationListTail;

    public override SyntaxKind Kind { get; } = SyntaxKind.DeclarationListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Declaration;
        if (DeclarationListTail is not null)
            yield return DeclarationListTail;
    }

    // protected internal override ExpressionNode Accept(ExpressionVisitor visitor)
    // {
    //     return visitor.Visit(this);
    // }
}