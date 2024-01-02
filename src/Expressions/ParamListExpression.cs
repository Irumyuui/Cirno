using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class ParamListExpression(ExpressionNode param, ExpressionNode? paramListTail) : ExpressionNode
{
    public ExpressionNode Param { get; } = param;

    public ExpressionNode? ParamListTail { get; } = paramListTail;

    public override SyntaxKind Kind => SyntaxKind.ParamListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Param;
        if (ParamListTail is not null)
            yield return ParamListTail;
    }
}