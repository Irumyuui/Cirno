namespace Cirno.Lexer;

public static class SyntaxTokenKindExtension {
    public static bool IsType(this SyntaxKind kind) => kind is SyntaxKind.Int or SyntaxKind.Void;

    public static bool IsTypeWithValue(this SyntaxKind kind) => kind is SyntaxKind.Int;
}
