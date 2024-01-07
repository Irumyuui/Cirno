using System.Collections.Immutable;
using System.Diagnostics.SymbolStore;
using Cirno.AbstractSyntaxTree;
using Cirno.Lexer;
using LLVMSharp.Interop;

namespace Cirno.SyntaxSymbol;

public enum ValueTypeKind
{
    Int,
    Void,
    Function,
    IntArray,
}

public interface ISymbol
{
    SyntaxToken Token { get; }
    string Name { get; }
    LLVMSharp.Interop.LLVMValueRef Value { get; set; }
    LLVMSharp.Interop.LLVMTypeRef Type { get; set; }
    ValueTypeKind TypeKind { get; }
}

public struct FunctionSymbol : ISymbol
{
    public SyntaxToken Token { get; }
    public string Name { get; }
    public LLVMValueRef Value { get; set; }
    public LLVMTypeRef Type { get; set; }

    public ValueTypeKind TypeKind => ValueTypeKind.Function;
    public LLVMValueRef[] Params => Value.Params;

    public FunctionSymbol(SyntaxToken token, string name, LLVMValueRef value, LLVMTypeRef type)
    {
        Token = token;
        Name = name;
        Value = value;
        Type = type;
        // TypeKind = typeKind;
    }
}

public struct ValueSymbol : ISymbol
{
    public SyntaxToken Token { get; }
    public string Name { get; }
    public LLVMValueRef Value { get; set; }
    public LLVMTypeRef Type { get; set; }
    
    public ValueTypeKind TypeKind { get; }

    public ValueSymbol(SyntaxToken token, string name, LLVMValueRef value, LLVMTypeRef type, ValueTypeKind typeKind)
    {
        Token = token;
        Name = name;
        Value = value;
        Type = type;
        TypeKind = typeKind;
    }
}

public sealed class EnvSymbolTable
{
    public EnvSymbolTable(EnvSymbolTable? prevTable) {
        PrevTable = prevTable;
    }

    private System.Collections.Generic.Dictionary<string, ISymbol> SymbolTable { get; set; } = [];

    public EnvSymbolTable? PrevTable { get; private set; } = null;

    public void Add(string key, ISymbol value) => SymbolTable.Add(key, value);

    public bool TryAdd(string key, ISymbol value) => SymbolTable.TryAdd(key, value);

    public bool ContainsKey(string key) => SymbolTable.ContainsKey(key);

    public bool TryGetSymbolFromCurrentTable(string key, out ISymbol? value)
        => SymbolTable.TryGetValue(key, out value);

    public bool TryGetSymbolFromLinkTable(string key, out ISymbol? value) {
        for (var table = this; table is not null; table = table.PrevTable) {
            if (table.TryGetSymbolFromCurrentTable(key, out value))
                return true;
        }
        value = null;
        return false;
    }
}
