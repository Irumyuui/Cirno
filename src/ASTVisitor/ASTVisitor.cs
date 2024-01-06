using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using Cirno.AbstractSyntaxTree;
using Cirno.SyntaxSymbol;

namespace Cirno.AbstractSyntaxTreeVisitor;

// public interface ASTVisitor
// {
//     void Visit(ASTNode node, params object[] args);

//     void Visitor(ProgramNode node, params object[] args);

//     void Visitor(DeclarationNode node, params object[] args);

//     void 
// }

public static class ASTChecker
{
    private static void RaiseError(in (int Line, int Col) textPosition, string? message) {
        DiagnosticTools.DiagnosticHelper.Raise($"[Line: {textPosition.Line}; Col: {textPosition.Col}] {message}");
    }

    public static bool Check(in ProgramNode root) {
        var symbolTable = new EnvSymbolTable(null);
        
        foreach (var declaration in root.DeclarationNodes) {
            switch (declaration)
            {
                case VariableDeclarationNode varDecl:
                    if (!Check(varDecl, ref symbolTable))
                        return false;
                    break;
                case FunctionDeclarationNode funcDecl:
                    if (!Check(funcDecl, ref symbolTable))
                        return false;
                    break;
                default:
                    // return false;
                    throw new System.Exception($"Unexpected type in {nameof(ProgramNode)}, expected {nameof(VariableDeclarationNode)} or {nameof(FunctionDeclarationNode)}");
            }
        }

        throw new NotImplementedException();
    }

    private static bool Check(in VariableDeclarationNode node, ref EnvSymbolTable symbolTable) {
        if (symbolTable.TryGetSymbolFromLinkTable(node.Name, out var prevSymbol)) {
            RaiseError(node.Position, $"Redefind {node.Name}");
            return false;
        }

        if (node.Type is not LiteralType.Int and not LiteralType.IntPtr) {
            RaiseError(node.Position, $"Unexpected type {node.Type} with {node.Name}");
            return false;
        }

        var symbol = new SymbolWithIntValue(node.Position, node.Type);
        symbolTable.Add(node.Name, symbol);

        return true;
    }

    private static bool Check(in FunctionDeclarationNode node, ref EnvSymbolTable symbolTable) {
        if (symbolTable.TryGetSymbolFromLinkTable(node.Name, out _)) {
            RaiseError(node.Position, $"Redefind {node.Name}");
            return false;
        }

        if (node.FunctionType.Length is 0) {
            RaiseError(node.Position, $"Undefine function {node.Name} return type");
            return false;
        }

        if (node.FunctionType.First() is not LiteralType.Int or LiteralType.Void) {
            RaiseError(node.Position, $"Unexpected function {node.Name} return type {node.FunctionType.First()}, expected {LiteralType.Int} or {LiteralType.Void}");
            return false;
        }

        if (node.Parameters.Length is 0) {
            symbolTable.Add(node.Name, new FunctionSymbol(node.Position, LiteralType.Function, node.FunctionType, []));
        }

        throw new NotImplementedException();
    }
}

// public sealed class ASTChecher
// {
    
// }
