using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class VarDeclarationExpression(SyntaxToken type, SyntaxToken identifier, SyntaxToken? arrayLength = null) : ExpressionNode
{
    public SyntaxToken Type { get; } = type;
    
    public SyntaxToken Identifier { get; } = identifier;
    
    public SyntaxToken? ArrayLength { get; } = arrayLength;

    public override SyntaxKind Kind => SyntaxKind.VarDeclarationExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Type;
        yield return Identifier;
        if (ArrayLength is not null)
            yield return ArrayLength;
    }
}