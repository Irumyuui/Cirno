using System.Collections.Generic;
using Cirno.Lexer;

namespace Cirno.Expressions;

public sealed class BinaryExpression(ExpressionNode left, SyntaxToken opToken, ExpressionNode right) : ExpressionNode {
    public override SyntaxKind Kind { get; } = opToken.Kind switch {
        SyntaxKind.Plus => SyntaxKind.AdditionExpression,
        SyntaxKind.Minus => SyntaxKind.SubtractionExpression,
        SyntaxKind.Asterisk => SyntaxKind.MultiplicationExpression,
        SyntaxKind.Slash => SyntaxKind.DivisionExpression,
        SyntaxKind.LessThan => SyntaxKind.LessThanExpression,
        SyntaxKind.LessThanOrEqualTo => SyntaxKind.LessThanOrEqualToExpression,
        SyntaxKind.EqualTo => SyntaxKind.EqualToExpression,
        SyntaxKind.GreaterThanOrEqualTo => SyntaxKind.GreaterThanOrEqualToExpression,
        SyntaxKind.GreaterThan => SyntaxKind.GreaterThanExpression,
        SyntaxKind.NotEqualTo => SyntaxKind.NotEqualToExpression,
        _ => throw new System.Exception($"Unknown expression {opToken}"),
    };

    public ExpressionNode Left { get; private set; } = left;

    public SyntaxToken Operator { get; private set; } = opToken;

    public ExpressionNode Right { get; private set; } = right;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }

    // public static int GetOperatorPriority(SyntaxKind)
    
    // public static Dictionary<SyntaxKind, int> OpertorPriority { get; private set; }

    // static BinaryExpression() {
    //     OpertorPriority = new Dictionary<SyntaxKind, int>() {
    //         {SyntaxKind.Plus, 1},
    //         {SyntaxKind.Minus, 1},
    //         {SyntaxKind.Asterisk, 50},
    //         {SyntaxKind.Slash, 50},
    //     };
    // }
}