using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace Cirno;

public sealed class Lexer {
    public Lexer(in string[] texts) {
        Texts = texts;
    }

    public string[] Texts { get; }

    private int _line = 0;

    private int _pos = 0;

    private (int Line, int Pos) Position => (_line, _pos);
    
    private StringBuilder _builder = new();

    public char Current {
        get {
            if (Position.Line >= Texts.Length || Position.Pos >= Texts[Position.Line].Length) 
                return '\0';
            return Texts[Position.Line][Position.Pos];
        }
    }

    private void MoveNextPosition() {
        if (_line >= Texts.Length)
            return;
        
        _pos ++;
        if (_pos >= Texts[_line].Length) {
            _line ++;
            _pos = 0;
        }
    }

    private bool OutOfRangePosition => _line >= Texts.Length;

    public SyntaxToken NextToken()
    {
        while (SyntaxKindHelper.IsEndOfFile(Current) && !OutOfRangePosition) {
            MoveNextPosition();
        }

        while (SyntaxKindHelper.IsDropChar(Current)) {
            MoveNextPosition();
        }

        if (SyntaxKindHelper.IsEndOfFile(Current)) {
            return new SyntaxToken(SyntaxKind.EndOfFile, string.Empty, _line, _pos);
        }

        int state = 0;
        if (Automata.NextState(state, Current) is null) {
            var utext = Current.ToString();
            MoveNextPosition();

            Diagostics.DiagosticHelper.Raise($"Unknown token {utext} in [{_line}:{_pos}].");
            return new SyntaxToken(SyntaxKind.Unknown, utext, _line, _pos);
        }

        _builder.Clear();
        while (true) {
            var next = Automata.NextState(state, Current);
            if (next is null)
                break;

            _builder.Append(Current);
            MoveNextPosition();
            state = next.Value;

            while (Current is '\r' or '\n' or '\t') {
                MoveNextPosition();
            }
        }

        var text = _builder.ToString();
        var kind = Automata.GetStateKind(state);

        if (kind is SyntaxKind.IdentifierOrKeyword)
        {
            if (SyntaxKindHelper.TryGetKeywordKind(text, out kind)) {
                return new SyntaxToken(kind, text, _line, _pos);
            }
            return new SyntaxToken(SyntaxKind.Identifier, text, _line, _pos);
        }

        if (kind is SyntaxKind.Number) {
            if (int.TryParse(text, out var result)) {
                return new SyntaxTokenWithValue<int>(SyntaxKind.Number, text, _line, _pos, result);
            }
            Diagostics.DiagosticHelper.Raise(
                $"Unexpected token {text} in [{_line}:{_pos}], except constant interger."
            );
            return new SyntaxToken(SyntaxKind.Unknown, text, _line, _pos);
        }

        return new SyntaxToken(kind, text, _line, _pos);
    }
}
