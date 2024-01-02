using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class ParamsExpression(ExpressionNode expr) : ExpressionNode
{
    /// <summary>
    /// maybe list or void type
    /// </summary>
    /// <value></value>
    public ExpressionNode Expr { get; } = expr;

    public override SyntaxKind Kind => SyntaxKind.ParamExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Expr;
    }
}