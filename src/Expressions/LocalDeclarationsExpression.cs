using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class LocalDeclarationsExpression(ExpressionNode varDeclaration, ExpressionNode? localDeclarationTail) : ExpressionNode
{
    public ExpressionNode VarDeclaration { get; } = varDeclaration;
    
    public ExpressionNode? LocalDeclarationTail { get; } = localDeclarationTail;

    public override SyntaxKind Kind => SyntaxKind.LocalDeclarationsExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return VarDeclaration;
        if (LocalDeclarationTail is not null)
            yield return LocalDeclarationTail;
    }
}