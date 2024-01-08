using System.Text;
using Cirno.DiagnosticTools;

namespace Cirno.Lexer;

public sealed class Lexer
{
    private string[] Texts { get; }

    private int _line = 0;
    
    private int _pos = 0;

    private (int Line, int Pos) Position => (_line, _pos);
    
    private readonly StringBuilder _builder = new();
    
    private readonly Cirno.DiagnosticTools.DiagnosticList _diagnosticList;

    public Lexer(in string[] texts)
    {
        _diagnosticList = new DiagnosticList();
        Texts = texts;
    }

    private char Current {
        get {
            if (Position.Line >= Texts.Length || Position.Pos >= Texts[Position.Line].Length) 
                return '\0';
            return Texts[Position.Line][Position.Pos];
        }
    }

    public DiagnosticList Diagnostics => _diagnosticList;

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

        var state = 0;
        if (Automata.NextState(state, Current) is null) {
            var utext = Current.ToString();
            MoveNextPosition();

            // DiagnosticTools.DiagnosticHelper.Raise($"Unknown token {utext} in [{_line}:{_pos}].");
            _diagnosticList.ReportLexerError(new TextLocation(_line, _pos), SyntaxKind.Unknown, utext);
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

            if (Current is '\r' or '\n' or '\t')
            {
                while (Current is '\r' or '\n' or '\t') {
                    MoveNextPosition();
                }
                break;
            }
        }

        var text = _builder.ToString();
        var kind = Automata.GetStateKind(state);

        switch (kind)
        {
            case SyntaxKind.IdentifierOrKeyword when SyntaxKindHelper.TryGetKeywordKind(text, out kind):
                return new SyntaxToken(kind, text, _line, _pos);
            case SyntaxKind.IdentifierOrKeyword:
                return new SyntaxToken(SyntaxKind.Identifier, text, _line, _pos);
            case SyntaxKind.Number when int.TryParse(text, out var result):
                return new SyntaxTokenWithValue<int>(SyntaxKind.Number, text, _line, _pos, result);
            case SyntaxKind.Number:
                // DiagnosticTools.DiagnosticHelper.Raise(
                //     $"Unexpected token {text} in [{_line}:{_pos}], except constant interger."
                // );
                _diagnosticList.ReportLexerError(new TextLocation(_line, _pos), SyntaxKind.Number, text);
                return new SyntaxToken(SyntaxKind.Unknown, text, _line, _pos);
            default:
                return new SyntaxToken(kind, text, _line, _pos);
        }
    }

    public void Reset()
    {
        _line = _pos = 0;
    }
    
    public SyntaxToken[] GetTokens()
    {
        Reset();
        SyntaxToken currentToken;
        System.Collections.Generic.List<SyntaxToken> tokenList = [];
        do
        {
            currentToken = NextToken();
            if (currentToken.Kind is not SyntaxKind.Unknown
                and not SyntaxKind.CommentsEnd)
            {
                tokenList.Add(currentToken);
            }
        } while (currentToken.Kind is not SyntaxKind.EndOfFile);

        return tokenList.ToArray();
    }
}
