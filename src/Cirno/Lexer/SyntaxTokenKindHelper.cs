using System.ComponentModel;
using System.Security.Cryptography;

namespace Cirno;

public static class SyntaxKindHelper
{
    public static bool IsDropChar(char x) => x is '\n' or '\t' or ' '; 

    public static bool IsEndOfFile(char x) => x is '\0';

    public static bool TryGetKeywordKind(string test, out SyntaxKind kind) {
        kind = test switch {
            "void" => SyntaxKind.Void,
            "int" => SyntaxKind.Int,
            "if" => SyntaxKind.If,
            "else" => SyntaxKind.Else,
            "while" => SyntaxKind.While,
            "return" => SyntaxKind.Return,
            _ => SyntaxKind.Unknown
        };
        return kind is not SyntaxKind.Unknown;
    }

    public static int GetOperatorPriority(SyntaxKind kind)
        => kind switch {
            SyntaxKind.LessThan
                or SyntaxKind.LessThanOrEqualTo
                or SyntaxKind.EqualTo
                or SyntaxKind.NotEqualTo
                or SyntaxKind.GreaterThan
                or SyntaxKind.GreaterThanOrEqualTo => 10,
                
            SyntaxKind.Plus 
                or SyntaxKind.Minus => 20,

            SyntaxKind.Asterisk 
                or SyntaxKind.Slash => 40,

            _ => -1,
        };
}
