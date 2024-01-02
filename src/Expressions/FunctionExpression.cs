using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class FunctionExpression(SyntaxToken retType, SyntaxToken fnName, ExpressionNode paramsList, ExpressionNode compoundStatement) : ExpressionNode
{
    public SyntaxToken RetType { get; } = retType;
    public SyntaxToken FnName { get; } = fnName;
    public ExpressionNode ParamsList { get; } = paramsList;
    public ExpressionNode CompoundStatement { get; } = compoundStatement;

    public override SyntaxKind Kind => SyntaxKind.FunctionExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return RetType;
        yield return FnName;
        yield return ParamsList;
        yield return CompoundStatement;
    }
}