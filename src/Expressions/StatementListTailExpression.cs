using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class StatementListTailExpression(ExpressionNode statement, ExpressionNode? statementListTail = null) : ExpressionNode
{
    public ExpressionNode Statement { get; } = statement;

    public ExpressionNode? StatementListTail { get; } = statementListTail;

    public override SyntaxKind Kind => SyntaxKind.StatementListTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Statement;
        if (StatementListTail is not null)
            yield return StatementListTail;
    }
}