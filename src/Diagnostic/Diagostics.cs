using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cirno.Diagnostic;

internal static class DiagnosticHelper
{
    public static List<string> Diagostics { get; private set; } = [];

    public static void Raise(string diagnostic) => Diagostics.Add(diagnostic);

    public static TReturn RaiseWithValue<TReturn>(string diagnostic, in TReturn returnValue)
    {
        Raise(diagnostic);
        return returnValue;
    }

    public static void Raise(IEnumerable<string> diagnostics) => Diagostics.AddRange(diagnostics);

    public static void RaiseAssert(bool expression, [CallerArgumentExpression(nameof(expression))] string message = "") {
        if (!expression)
            Raise(message);
    }

    public static void PrintDiagnostics()
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        foreach (var diagnostic in Diagostics)
            Console.WriteLine(diagnostic);

        Console.ForegroundColor = prevColor;
    }

    public static bool Any => Diagostics.Count is not 0;

    public static int Count => Diagostics.Count;
}
