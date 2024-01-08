using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using Cirno.Lexer;
using Cirno.SyntaxSymbol;

namespace Cirno.DiagnosticTools;

public readonly record struct TextLocation(int Line, int Col)
{
    public override string ToString() => $"[{Line}:{Col}]";

    public static TextLocation NoPosition => new TextLocation(-1, -1);
}

public enum DiagnosticKind
{
    LexerError,
    ParseError,
    SemanticError,
    CodeGenError,
    SemanticWarning
}

public readonly record struct Diagnostic(
    in TextLocation Location,
    DiagnosticKind Kind,
    string Message = "")
{
    public override string ToString() => $"{Kind}: {Message} In {Location}";
    
    public static Diagnostic RaiseError(in TextLocation location, DiagnosticKind kind, string message = "")
        => new Diagnostic(location, kind, message);

    public void Dump()
    {
        Console.ResetColor();

        Console.Write($"{Location} ");
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{Kind}");
        Console.ResetColor();
        
        Console.WriteLine($": {Message}");
    }

    public void Dump(in string[] text)
    {
        Console.ForegroundColor = ConsoleColor.White;
        var position = Location.ToString();
        Console.Write(position);
        Console.Write(" ");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{Kind}");
        Console.ResetColor();
        
        Console.Write(": ");
        Console.WriteLine(Message);

        var tabBlock = new string(' ', Math.Max(0, position.Length - 1));

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"{tabBlock}|  ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{text[Location.Line]}");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"{tabBlock}| ");
        
        Console.ResetColor();
    }
}

public sealed class DiagnosticList : System.Collections.Generic.IEnumerable<Diagnostic>
{
    private readonly System.Collections.Generic.List<Diagnostic> _diagnostics;
    
    public DiagnosticList(in DiagnosticList list)
    {
        _diagnostics = [..list._diagnostics];
    }

    public DiagnosticList(System.Collections.Generic.IEnumerable<Diagnostic> diagnostics)
    {
        _diagnostics = [..diagnostics];
    }

    public DiagnosticList() => _diagnostics = [];

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

    public void ReportSemanticError(in TextLocation location, string message)
    {
        Report(location, DiagnosticKind.SemanticError, message);
    }
    
    public void ReportSemanticWarning(in TextLocation location, string message)
    {
        Report(location, DiagnosticKind.SemanticWarning, message);
    }

    public void ReportCallFunctionArgsNotFullError(in TextLocation location, string funcName, int argc)
    {
        ReportSemanticError(location,
            $"Calling function {funcName} requires {argc} parameters.");
    }
    
    public void ReportUnknownValueTypeError(in TextLocation location)
    {
        Report(location, DiagnosticKind.SemanticError, "Unknown value type.");
    }
    
    public void ReportUndefinedVariableError(in TextLocation location, string expectVariableName)
    {
        Report(location, DiagnosticKind.SemanticError, $"Undefined variable {expectVariableName}");
    }

    public void ReportNotExpectType(in TextLocation location, string variableName, ValueTypeKind expectType, ValueTypeKind foundType)
    {
        Report(location, DiagnosticKind.SemanticError, $"Expect type {expectType}, but found type {foundType} on variable {variableName}.");
    }
    
    public void ReportNotLeftValueError(in TextLocation location, string expectVariableName)
    {
        Report(location, DiagnosticKind.SemanticError, $"Expect variable {expectVariableName} is not left value.");
    }

    public void Dump()
    {
        foreach (var item in _diagnostics)
            item.Dump();
    }

    public void Dump(string[] text)
    {
        foreach (var item in _diagnostics)
        {
            item.Dump(text);
        }
    }
}