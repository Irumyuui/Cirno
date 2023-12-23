namespace Cirno.Lexer;
public static class SyntaxTokenExtension
{
    public static bool TryGetValue<TValue>(this SyntaxToken token, out TValue? value) {
        if (token is SyntaxTokenWithValue<TValue> tv) {
            value = tv.Value;
            return true;
        }
        value = default;
        return false;
    }

    public static bool IsTypeToken(this SyntaxToken token) => token.Kind is SyntaxKind.Int or SyntaxKind.Void;

    public static bool IsCommentToken(this SyntaxToken token) => token.Kind is SyntaxKind.CommentsEnd;

    public static bool IsEndOfFileToken(this SyntaxToken token) => token.Kind is SyntaxKind.EndOfFile;

    public static bool IsOperatorToken(this SyntaxToken token)
        => token.Kind is SyntaxKind.Plus
                      or SyntaxKind.Minus
                      or SyntaxKind.Asterisk
                      or SyntaxKind.Slash
                      or SyntaxKind.Assign
                      or SyntaxKind.LessThan
                      or SyntaxKind.LessThanOrEqualTo
                      or SyntaxKind.EqualTo
                      or SyntaxKind.NotEqualTo
                      or SyntaxKind.GreaterThan
                      or SyntaxKind.GreaterThanOrEqualTo;

    public static bool IsRelopToken(this SyntaxToken token)
        => token.Kind is SyntaxKind.LessThan
                      or SyntaxKind.LessThanOrEqualTo
                      or SyntaxKind.EqualTo
                      or SyntaxKind.NotEqualTo
                      or SyntaxKind.GreaterThan
                      or SyntaxKind.GreaterThanOrEqualTo;

}
