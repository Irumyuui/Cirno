using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class StatementListExpression(ExpressionNode statementListTail) : ExpressionNode
{
    public ExpressionNode StatementListTail { get; } = statementListTail;

    public override SyntaxKind Kind => SyntaxKind.StatementListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return StatementListTail;
    }
}