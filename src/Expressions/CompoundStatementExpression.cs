using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class CompoundStatementExpression(ExpressionNode? localDeclarations, ExpressionNode? statementList) : ExpressionNode
{
    public ExpressionNode? LocalDeclarations { get; } = localDeclarations;
    
    public ExpressionNode? StatementList { get; } = statementList;

    public override SyntaxKind Kind => SyntaxKind.CompoundStatementExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (LocalDeclarations is not null)
            yield return LocalDeclarations;
        if (StatementList is not null)
            yield return StatementList;
    }
}