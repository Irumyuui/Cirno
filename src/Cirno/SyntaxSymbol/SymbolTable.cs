using System.Collections.Immutable;
using Cirno.AbstractSyntaxTree;

namespace Cirno.SyntaxSymbol;

public class Symbol
{
    public Symbol(in (int Line, int Col) position, LiteralType symbolType)
    {
        Position = position;
        SymbolType = symbolType;
        // Value = value;
        // ValueRef = valueRef;
    }

    public (int Line, int Col) Position { get; }

    public LiteralType SymbolType { get; set; }

    public LLVMSharp.Interop.LLVMValueRef? ValueRef { get; set; } = null;

    // public object? Value { get; set; } = null;

    public string GetTextPositionString() => $"[Line: {Position.Line}; Col: {Position.Col}]";
}

public sealed class SymbolWithIntValue : Symbol
{
    public SymbolWithIntValue(in (int Line, int Col) position, LiteralType symbolType, in int? value = null) : base(position, symbolType) {
        Value = value;
    }

    public int? Value { get; }
}

public sealed class FunctionSymbol : Symbol
{
    public FunctionSymbol(in (int Line, int Col) position, LiteralType symbolType, ImmutableArray<LiteralType> functionType, ImmutableArray<(string Name, LiteralType Type)> parameters) : base(position, symbolType)
    {
        FunctionType = functionType;
        Parameters = parameters;
    }

    public ImmutableArray<LiteralType> FunctionType { get; private set; }

    public ImmutableArray<(string Name, LiteralType Type)> Parameters { get; private set; }
}

// public class Symbol<TValue> : Symbol
// {    
//     public Symbol(in (int Line, int Col) position, LiteralType symbolType, TValue? value) : base(position, symbolType) {
//         Value = value;
//     }

//     public TValue? Value { get; set; }
// }

public sealed class EnvSymbolTable
{
    public EnvSymbolTable(EnvSymbolTable? prevTable) {
        PrevTable = prevTable;
    }

    private System.Collections.Generic.Dictionary<string, Symbol> SymbolTable { get; set; } = [];

    public EnvSymbolTable? PrevTable { get; private set; } = null;

    public void Add(string key, Symbol value) => SymbolTable.Add(key, value);

    public bool TryAdd(string key, Symbol value) => SymbolTable.TryAdd(key, value);

    public bool ContainsKey(string key) => SymbolTable.ContainsKey(key);

    public bool TryGetSymbolFromCurrentTable(string key, out Symbol? value) => SymbolTable.TryGetValue(key, out value);

    public bool TryGetSymbolFromLinkTable(string key, out Symbol? value) {
        for (var table = this; table is not null; table = table.PrevTable) {
            if (table.TryGetSymbolFromCurrentTable(key, out value))
                return true;
        }
        value = null;
        return false;
    }
}
