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

public enum ValueScopeKind
{
    Global,
    Local
}

public interface ISymbol
{
    SyntaxToken Token { get; }
    string Name { get; }
    LLVMSharp.Interop.LLVMValueRef Value { get; set; }
    LLVMSharp.Interop.LLVMTypeRef Type { get; set; }
    ValueTypeKind TypeKind { get; }
    ValueScopeKind ScopeKind { get; }
}

public struct FunctionSymbol : ISymbol
{
    public SyntaxToken Token { get; }
    public string Name { get; }
    public LLVMValueRef Value { get; set; }
    public LLVMTypeRef Type { get; set; }

    public readonly ValueTypeKind TypeKind => ValueTypeKind.Function;

    public readonly ValueScopeKind ScopeKind { get; }

    public readonly LLVMValueRef[] Params => Value.Params;

    public FunctionSymbol(SyntaxToken token, string name, LLVMValueRef value, LLVMTypeRef type, ValueScopeKind scopeKind = ValueScopeKind.Global)
    {
        Token = token;
        Name = name;
        Value = value;
        Type = type;
        ScopeKind = scopeKind;
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

    public ValueScopeKind ScopeKind { get; }

    public ValueSymbol(SyntaxToken token, string name, LLVMValueRef value, LLVMTypeRef type, ValueTypeKind typeKind, ValueScopeKind scopeKind)
    {
        Token = token;
        Name = name;
        Value = value;
        Type = type;
        TypeKind = typeKind;
        ScopeKind = scopeKind;
    }
}

public sealed class EnvSymbolTable
{
    public EnvSymbolTable(EnvSymbolTable? prevTable) {
        PrevTable = prevTable;
        // CurrentFunction = currentFunction;
    }

    private System.Collections.Generic.Dictionary<string, ISymbol> SymbolTable { get; set; } = [];

    public EnvSymbolTable? PrevTable { get; private set; } = null;

    public void Add(string key, ISymbol value) => SymbolTable.Add(key, value);

    public bool TryAdd(string key, ISymbol value) => SymbolTable.TryAdd(key, value);

    public bool ContainsKey(string key) => SymbolTable.ContainsKey(key);

    public bool TryGetSymbolFromCurrentTable(string key, out ISymbol? value)
        => SymbolTable.TryGetValue(key, out value);
    
    // public LLVMValueRef? CurrentFunction { get; }

    public bool TryGetSymbolFromLinkTable(string key, out ISymbol? value) {
        for (var table = this; table is not null; table = table.PrevTable) {
            if (table.TryGetSymbolFromCurrentTable(key, out value))
                return true;
        }
        value = null;
        return false;
    }

    public bool CurrentContains(string name) => SymbolTable.ContainsKey(name);
}
