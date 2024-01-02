using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class CallFunctionExpression(SyntaxToken identifier, ExpressionNode args) : ExpressionNode
{
    public SyntaxToken Identifier { get; } = identifier;
    public ExpressionNode Args { get; } = args;

    public override SyntaxKind Kind => SyntaxKind.CallFunctionExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Identifier;
        yield return Args;
    }
}