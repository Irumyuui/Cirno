using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class ArgsExpression(ExpressionNode? argsList = null) : ExpressionNode
{
    public ExpressionNode? ArgsList { get; } = argsList;

    public override SyntaxKind Kind => SyntaxKind.ArgsExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (ArgsList is not null)
            yield return ArgsList;
    }
}