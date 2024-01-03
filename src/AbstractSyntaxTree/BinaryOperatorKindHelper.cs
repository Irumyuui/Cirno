using Cirno.AbstractSyntaxTreeVisitor;
using Cirno.Expressions;
using Cirno.Lexer;

namespace Cirno.AbstractSyntaxTree;

// public static class LiteralTypeHelper
// {
//     public static LiteralType TryParseFromSyntaxKind()
// }

public static class BinaryOperatorKindHelper
{
    public static bool TryParse(SyntaxKind kind, out BinaryOperatorKind result)
    {
        result = kind switch
        {
            // _ =>
            SyntaxKind.Plus => BinaryOperatorKind.Addition,
            SyntaxKind.Minus => BinaryOperatorKind.Subtraction,
            SyntaxKind.Asterisk => BinaryOperatorKind.Multiplication,
            SyntaxKind.Slash => BinaryOperatorKind.Division,
            SyntaxKind.LessThan => BinaryOperatorKind.LessThan,
            SyntaxKind.LessThanOrEqualTo => BinaryOperatorKind.LessThanOrEqualTo,
            SyntaxKind.EqualTo => BinaryOperatorKind.EqualTo,
            SyntaxKind.NotEqualTo => BinaryOperatorKind.NotEqualTo,
            SyntaxKind.GreaterThanOrEqualTo => BinaryOperatorKind.GreaterThanOrEqualTo,
            SyntaxKind.GreaterThan => BinaryOperatorKind.GreaterThan,
            _ => BinaryOperatorKind.Unknown
        };
        return result is not BinaryOperatorKind.Unknown;
    }

    public static bool TryParse(BinaryOperatorKind kind, out string result)
    {
        result = kind switch
        {
            BinaryOperatorKind.Addition => "+",
            BinaryOperatorKind.Subtraction => "-",
            BinaryOperatorKind.Multiplication => "*",
            BinaryOperatorKind.Division => "/",
            BinaryOperatorKind.LessThan => "<",
            BinaryOperatorKind.LessThanOrEqualTo => "<=",
            BinaryOperatorKind.EqualTo => "==",
            BinaryOperatorKind.NotEqualTo => "!=",
            BinaryOperatorKind.GreaterThanOrEqualTo => ">=",
            BinaryOperatorKind.GreaterThan => ">",
            _ => string.Empty
        };
        return result != string.Empty;
    }
}