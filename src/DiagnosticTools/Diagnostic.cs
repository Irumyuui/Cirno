using System;
using System.Collections;
using Cirno.Lexer;

namespace Cirno.DiagnosticTools;

public readonly record struct TextLocation(int Line, int Col)
{
    public override string ToString() => $"[{Line}:{Col}]";
}

public enum DiagnosticKind
{
    LexerError,
    ParseError,
    SemanticError,
    EmitError,
}

public readonly record struct Diagnostic(
    in TextLocation Location,
    DiagnosticKind Kind,
    string Message = "")
{
    public override string ToString() => $"{Kind}: {Message} In {Location}";
    
    public static Diagnostic RaiseError(in TextLocation location, DiagnosticKind kind, string message = "")
        => new Diagnostic(location, kind, message);
}

public sealed class DiagnosticList : System.Collections.Generic.IEnumerable<Diagnostic>
{
    private readonly System.Collections.Generic.List<Diagnostic> _diagnostics;

    // private readonly string[] _text;
    
    public DiagnosticList(in DiagnosticList list)
    {
        _diagnostics = [..list._diagnostics];
        // _text = list._text;
    }

    public DiagnosticList(System.Collections.Generic.IEnumerable<Diagnostic> diagnostics)
    {
        _diagnostics = [..diagnostics];
        // _text = text;
    }

    public DiagnosticList() => _diagnostics = [];

    // public DiagnosticList(in string[] text)
    // {
    //     _diagnostics = [];
    //     // _text = text;
    // }

    public int Count => _diagnostics.Count;

    public System.Collections.Generic.IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public void Report(in Diagnostic diagnostic) => _diagnostics.Add(diagnostic);

    public void Report(in TextLocation location, DiagnosticKind kind, string message = "") => Report(new Diagnostic(location, kind, message));
    
    public void ReportLexerError(in TextLocation location, SyntaxKind expectTokenKind, string text)
    {
        var reportMessage = $"Expect {expectTokenKind}, found {text}.";
        Report(location, DiagnosticKind.LexerError, reportMessage);
    }
    
    public void ReportLexerError(in TextLocation location, SyntaxKind expectTokenKind, in SyntaxToken currentToken)
    {
        ReportLexerError(location, expectTokenKind, currentToken.Name);    
    }

    public void ReportParserError(in TextLocation location, SyntaxKind expectTokenKind, in SyntaxToken currentToken)
    {
        var reportMessage = $"Expect {expectTokenKind}, found {currentToken}.";
        Report(location, DiagnosticKind.ParseError, reportMessage);
        
    }
    
    public static void PrintDiagnostics(System.Collections.Generic.IEnumerable<Diagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            Console.WriteLine(diagnostics);
        }
    }
}