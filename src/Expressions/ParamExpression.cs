using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class ParamExpression(SyntaxToken typeToken, SyntaxToken identifierToken) : ExpressionNode
{
    public SyntaxToken TypeToken { get; } = typeToken;

    public SyntaxToken IdentifierToken { get; } = identifierToken;

    public override SyntaxKind Kind => SyntaxKind.ParamExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return TypeToken;
        yield return IdentifierToken;
    }
}