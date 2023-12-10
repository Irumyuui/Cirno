using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cirno.Diagostics;

internal static class DiagosticHelper
{
    public static List<string> Diagostics { get; private set; } = [];

    public static void Raise(string diagostic) => Diagostics.Add(diagostic);

    public static TReturn RaiseWithValue<TReturn>(string diagostic, in TReturn returnValue)
    {
        Raise(diagostic);
        return returnValue;
    }

    public static void Raise(IEnumerable<string> diagostics) => Diagostics.AddRange(diagostics);

    public static void RaiseAssert(bool expression, [CallerArgumentExpression(nameof(expression))] string message = "") {
        if (!expression)
            Raise(message);
    }

    public static void PrintDiagostics()
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        foreach (var diagostic in Diagostics)
            Console.WriteLine(diagostic);

        Console.ForegroundColor = prevColor;
    }

    public static bool Any => Diagostics.Count is not 0;

    public static int Count => Diagostics.Count;
}
