using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class AssignmentExpression(ExpressionNode variable, ExpressionNode expr) : ExpressionNode
{
    public ExpressionNode Variable { get; } = variable;
    
    public ExpressionNode Expr { get; } = expr;

    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Variable;
        yield return Expr;
    }
}