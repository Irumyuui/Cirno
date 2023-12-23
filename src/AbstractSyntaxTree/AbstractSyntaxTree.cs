using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Cirno.AbstractSyntaxTreeVisitor;
using Cirno.Expressions;
using Cirno.Lexer;

namespace Cirno.AbstractSyntaxTree;

public enum ASTNodeType
{
    VariableDeclaration,
    FunctionDeclaration,
    StatementBody,
    LiteralExpression,
    CallFunction,
    ReturnExpr,
    BinaryOperator,
    CompoundStatement,
    WhileStatement,
    IfStatement,
    ArraySubscriptExpr,
    ProgramNode,
    IntegerLiteral
}

public enum LiteralType
{
    Void,  // is error
    Int,
    IntPtr,  // Is type as prt? 

    Function,
}

// public static class LiteralTypeHelper
// {
//     public static LiteralType TryParseFromSyntaxKind()
// }

public enum BinaryOperatorKind
{
    Assignment,
    Addition,
    Subtraction,
    Multiplication,
    Division,
    LessThan,
    LessThanOrEqualTo,
    EqualTo,
    NotEqualTo,
    GreaterThanOrEqualTo,
    GreaterThan,
    Unknown,
}

public static class BinaryOperatorKindHelper
{
    public static bool TryParse(SyntaxKind kind, out BinaryOperatorKind result)
    {
        result = kind switch
        {
            // _ =>
            SyntaxKind.Plus => BinaryOperatorKind.Addition,
            SyntaxKind.Minus => BinaryOperatorKind.Subtraction,
            SyntaxKind.Asterisk => BinaryOperatorKind.Multiplication,
            SyntaxKind.Slash => BinaryOperatorKind.Division,
            SyntaxKind.LessThan => BinaryOperatorKind.LessThan,
            SyntaxKind.LessThanOrEqualTo => BinaryOperatorKind.LessThanOrEqualTo,
            SyntaxKind.EqualTo => BinaryOperatorKind.EqualTo,
            SyntaxKind.NotEqualTo => BinaryOperatorKind.NotEqualTo,
            SyntaxKind.GreaterThanOrEqualTo => BinaryOperatorKind.GreaterThanOrEqualTo,
            SyntaxKind.GreaterThan => BinaryOperatorKind.GreaterThan,
            _ => BinaryOperatorKind.Unknown
        };
        return result is not BinaryOperatorKind.Unknown;
    }

    public static bool TryParse(BinaryOperatorKind kind, out string result)
    {
        result = kind switch
        {
            BinaryOperatorKind.Addition => "+",
            BinaryOperatorKind.Subtraction => "-",
            BinaryOperatorKind.Multiplication => "*",
            BinaryOperatorKind.Division => "/",
            BinaryOperatorKind.LessThan => "<",
            BinaryOperatorKind.LessThanOrEqualTo => "<=",
            BinaryOperatorKind.EqualTo => "==",
            BinaryOperatorKind.NotEqualTo => "!=",
            BinaryOperatorKind.GreaterThanOrEqualTo => ">=",
            BinaryOperatorKind.GreaterThan => ">",
            _ => string.Empty
        };
        return result != string.Empty;
    }
}

public abstract class ASTNode
{
    public abstract string Name { get; }
    public abstract ASTNodeType NodeType { get; }
    // public abstract ASTNode Parameters { get; protected set; }

    public abstract ASTNode? Parent { get; protected internal set; }

    public abstract IEnumerable<ASTNode> GetChildren();

    public virtual void CodeGen() { }

    public static void PrettyPrint(ASTNode node, string indent = "", bool isLast = true)
    {
        // ├─ │ └─

        var marker = isLast ? "└───" : "├───";
        // var marker = isLast ? "└" : "├";

        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(indent);
        Console.Write(marker);
        Console.ForegroundColor = prevColor;

        Console.Write(" ");
        node.PrettyPrint();

        Console.WriteLine();

        indent += isLast ? "    " : "│   ";
        // indent += isLast ? " " : "│";

        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            PrettyPrint(child, indent, child == lastChild);
    }

    // public override string ToString() => $"{NodeType}";

    // public abstract void PrettyPrint();
    public virtual void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        // Console.ForegroundColor = ConsoleColor.DarkYellow;
        // Console.Write(" => ");

        Console.ForegroundColor = prevColor;
    }

    // protected internal virtual ASTVisitor Accept(ASTVisitor visitor) {
           
    // }
}

public abstract class DeclarationNode : ASTNode
{

}

public abstract class StatementNode : ASTNode
{

}

public abstract class ExprNode : StatementNode
{

}

public sealed class ProgramNode : ASTNode
{
    public ProgramNode(ImmutableArray<DeclarationNode> declarationNodes)
    {
        DeclarationNodes = declarationNodes;
        Parent = this;
    }

    public override string Name { get; } = "Program";

    public override ASTNodeType NodeType => ASTNodeType.ProgramNode;

    public override ASTNode? Parent { get; protected internal set; }

    public ImmutableArray<DeclarationNode> DeclarationNodes { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        foreach (var item in DeclarationNodes)
            yield return item;
    }

    public override void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        // Console.ForegroundColor = ConsoleColor.DarkYellow;
        // Console.Write(" => ");

        Console.ForegroundColor = prevColor;
    }
}

public sealed class VariableDeclarationNode : DeclarationNode
{
    public VariableDeclarationNode(in (int Line, int Col) position, ASTNode? parent, string name, LiteralType type, int? arrayLength = null)
    {
        Position = position;
        Parent = parent;
        Name = name;
        Type = type;
        ArrayLength = arrayLength;
    }

    public override ASTNodeType NodeType => ASTNodeType.VariableDeclaration;

    public (int Line, int Col) Position { get; }

    public override ASTNode? Parent { get; protected internal set; }

    public override string Name { get; }

    public LiteralType Type { get; }

    /// <summary>
    /// It's not null when the variable is array type.
    /// </summary>
    /// <value></value>
    public int? ArrayLength { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        // yield return Enumerable.Empty<ASTNode>();
        yield break;
    }

    public bool IsArrayType() => Type is LiteralType.IntPtr;

    public override void PrettyPrint()
    {
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{NodeType}");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name} : {Type}");
        if (ArrayLength is not null)
        {
            Console.Write($" [{ArrayLength}]");
        }

        Console.ForegroundColor = prevColor;
    }
}

public sealed class FunctionDeclarationNode : DeclarationNode
{
    public FunctionDeclarationNode(in (int Line, int Col) position,
                                   ASTNode? parent,
                                   string name,
                                   ImmutableArray<LiteralType> functionType,
                                   ImmutableArray<(string Name, LiteralType Type)> parameters,
                                   CompoundStatementNode body
    )
    {
        Position = position;
        Parent = parent;
        Name = name;
        FunctionType = functionType;
        Parameters = parameters;
        Body = body;
    }

    public override ASTNodeType NodeType => ASTNodeType.FunctionDeclaration;

    public ImmutableArray<LiteralType> FunctionType { get; private set; }

    public ImmutableArray<(string Name, LiteralType Type)> Parameters { get; private set; }

    public CompoundStatementNode Body { get; }

    public override string Name { get; }
    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        yield return Body;
    }

    public override void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name} -> {FunctionType.First()}::({string.Join(',', Parameters.Select(item => $"{item.Name}:{item.Type}"))})");

        Console.ForegroundColor = prevColor;
    }
}

public sealed class CompoundStatementNode : ASTNode
{
    public CompoundStatementNode(ASTNode? parent, ImmutableArray<DeclarationNode> declarationStatement, ImmutableArray<StatementNode> expressionStatement)
    {
        Parent = parent;
        DeclarationStatement = declarationStatement;
        ExpressionStatement = expressionStatement;
    }

    public ImmutableArray<DeclarationNode> DeclarationStatement { get; }

    public ImmutableArray<StatementNode> ExpressionStatement { get; }

    public override string Name => "CompoundStatement";

    public override ASTNodeType NodeType => ASTNodeType.CompoundStatement;

    public override ASTNode? Parent { get; protected internal set; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        foreach (var nex in DeclarationStatement)
            yield return nex;
        foreach (var nex in ExpressionStatement)
            yield return nex;
    }
}

public class LiteralNode : ExprNode
{
    public LiteralNode(in (int Line, int Col) position, ASTNode? parent, string name)
    {
        Position = position;
        Parent = parent;
        Name = name;
    }

    public override string Name { get; }

    public override ASTNodeType NodeType => ASTNodeType.LiteralExpression;

    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        // yield return (ASTNode)Enumerable.Empty<ASTNode>();
        yield break;
    }

    public override void PrettyPrint()
    {
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{NodeType}");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write(Name);

        Console.ForegroundColor = prevColor;
    }
}

public sealed class ArraySubscriptExprNode : LiteralNode
{
    public ArraySubscriptExprNode(in (int Line, int Col) position, ASTNode? parent, string name, ExprNode offsetExpr) : base(position, parent, name)
    {
        // Parent = parent;
        // Name = name;
        // RefType = refType;
        // ResultType = resultType;
        OffsetExpr = offsetExpr;
    }

    // public override string Name { get; }

    // public VariantType RefType { get; }

    // public VariantType ResultType { get; }

    public ExprNode OffsetExpr { get; }

    public override ASTNodeType NodeType => ASTNodeType.ArraySubscriptExpr;

    public override ASTNode? Parent { get; protected internal set; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        yield return OffsetExpr;
    }

    // public override void PrettyPrint() {
    //     Console.Write($"{NodeType}");
    //     var prevColor = Console.ForegroundColor;

    //     Console.ForegroundColor = ConsoleColor.DarkYellow;
    //     Console.Write(" => ");

    //     Console.ForegroundColor = ConsoleColor.DarkBlue;
    //     Console.Write($"{Name");

    //     Console.ForegroundColor = prevColor;
    // }
}

public sealed class IntegerLiteral<TValue> : LiteralNode
{
    public IntegerLiteral(in (int Line, int Col) position, ASTNode? parent, string name, LiteralType type, TValue value)
        : base(position, parent, name)
    {
        Type = type;
        Value = value;
    }

    public LiteralType Type { get; }

    public TValue Value { get; }

    public override ASTNodeType NodeType => ASTNodeType.IntegerLiteral;

    public override IEnumerable<ASTNode> GetChildren()
    {
        return Enumerable.Empty<ASTNode>();
    }

    public override void PrettyPrint()
    {
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{NodeType}");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name} = {Value} : {Type}");

        Console.ForegroundColor = prevColor;
    }
}

public sealed class CallFunctionNode : ExprNode
{
    public CallFunctionNode(in (int Line, int Col) position, ASTNode? parent, string name, ImmutableArray<ExprNode> args)
    {
        Position = position;
        Parent = parent;
        Name = name;
        Args = args;
    }

    public override string Name { get; }

    public ImmutableArray<ExprNode> Args { get; }

    public override ASTNodeType NodeType => ASTNodeType.CallFunction;

    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }


    public override IEnumerable<ASTNode> GetChildren()
    {
        foreach (var item in Args)
            yield return item;
    }
    public override void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name}");

        Console.ForegroundColor = prevColor;
    }
}

public sealed class BinaryOperatorNode : ExprNode
{
    public BinaryOperatorNode(in (int Line, int Col) position, ASTNode? parent, string name, ExprNode left, BinaryOperatorKind? opKind, ExprNode? right)
    {
        Position = position;
        Parent = parent;
        Name = name;
        Left = left;
        OpKind = opKind;
        Right = right;
    }

    public override string Name { get; }

    public override ASTNodeType NodeType => ASTNodeType.BinaryOperator;

    public (int Line, int Col) Position { get; }
    public override ASTNode? Parent { get; protected internal set; }

    public ASTNode Left { get; }

    public BinaryOperatorKind? OpKind { get; }

    public ASTNode? Right { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        yield return Left;
        if (Right is not null)
            yield return Right;
    }

    public override void PrettyPrint()
    {
        Console.Write($"{NodeType}");
        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($" [Line: {Position.Line}; Col: {Position.Col}]");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(" => ");

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write($"{Name} : {OpKind}");

        Console.ForegroundColor = prevColor;
    }
}

public sealed class ReturnStatementNode : StatementNode
{
    public ReturnStatementNode(ASTNode? parent, ExprNode? returnExpr)
    {
        Parent = parent;
        ReturnExpr = returnExpr;
    }

    public override string Name => "Return Expr";

    public override ASTNodeType NodeType => ASTNodeType.ReturnExpr;

    public override ASTNode? Parent { get; protected internal set; }

    public ExprNode? ReturnExpr { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        if (ReturnExpr is not null)
            yield return ReturnExpr;
    }
}

public sealed class WhileStatementNode : StatementNode
{
    public WhileStatementNode(ASTNode? parent, ExprNode expr, CompoundStatementNode compoundStatement)
    {
        Parent = parent;
        Expr = expr;
        CompoundStatement = compoundStatement;
    }

    public override string Name { get; } = string.Empty;

    public override ASTNodeType NodeType => ASTNodeType.WhileStatement;

    public override ASTNode? Parent { get; protected internal set; }

    public ExprNode Expr { get; }

    public CompoundStatementNode CompoundStatement { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        yield return Expr;
        yield return CompoundStatement;
    }
}

public sealed class IfStatementNode : StatementNode
{
    public IfStatementNode(ASTNode? parent, ExprNode expr, params CompoundStatementNode[] body)
    {
        Parent = parent;
        Expr = expr;
        Body = body;
    }

    public override string Name => string.Empty;

    public override ASTNodeType NodeType => ASTNodeType.IfStatement;

    public override ASTNode? Parent { get; protected internal set; }

    public ExprNode Expr { get; }

    public CompoundStatementNode[] Body { get; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        yield return Expr;
        foreach (var item in Body)
            yield return item;
    }
}

public sealed class AST
{
    public AST(Cirno.Expressions.ExpressionTree tree)
    {
        ASTBuildVisitor.VisitProgram(tree.Root, out var root);
        Root = root;
    }

    public ASTNode Root { get; private set; }
}
