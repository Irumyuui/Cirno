using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class SelectionStatementExpression(ExpressionNode logicExpr, ExpressionNode? firstStatement = null, ExpressionNode? secondStatement = null) : ExpressionNode
{
    public ExpressionNode LogicExpr { get; } = logicExpr;

    public ExpressionNode? FirstStatementsList { get; } = firstStatement;

    public ExpressionNode? SecondStatementsList { get; } = secondStatement;

    public override SyntaxKind Kind => SyntaxKind.SelectionStatementExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return LogicExpr;
        if (FirstStatementsList is not null)
            yield return FirstStatementsList;
        if (SecondStatementsList is not null)
            yield return SecondStatementsList;
    }
}