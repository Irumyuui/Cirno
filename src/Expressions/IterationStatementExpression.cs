using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class IterationStatementExpression(ExpressionNode logicExpr, ExpressionNode? block) : ExpressionNode
{
    public ExpressionNode LogicExpr { get; } = logicExpr;
    
    public ExpressionNode? Block  { get; } = block ;

    public override SyntaxKind Kind => SyntaxKind.IterationStatementExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return LogicExpr;
        if (Block is not null)
            yield return Block;
    }
}