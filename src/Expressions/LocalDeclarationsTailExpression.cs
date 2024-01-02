using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class LocalDeclarationsTailExpression(ExpressionNode varDeclaration, ExpressionNode? localDeclarationTail) : ExpressionNode
{
    public ExpressionNode VarDeclaration { get; } = varDeclaration;

    public ExpressionNode? LocalDeclarationTail { get; } = localDeclarationTail;

    public override SyntaxKind Kind => SyntaxKind.LocalDeclarationTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return VarDeclaration;
        if (LocalDeclarationTail is not null)
            yield return LocalDeclarationTail;
    }
}