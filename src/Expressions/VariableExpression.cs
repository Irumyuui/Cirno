using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class VariableExpression(SyntaxToken identifier, ExpressionNode? offsetExpr = null, bool isArrayRef = false) : ExpressionNode
{
    public SyntaxToken Identifier { get; } = identifier;

    public ExpressionNode? OffsetExpr { get; } = offsetExpr;

    public bool IsArrayRef { get; } = isArrayRef;

    public override SyntaxKind Kind => SyntaxKind.VariableExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Identifier;
        if (OffsetExpr is not null)
            yield return OffsetExpr;        
    }
}