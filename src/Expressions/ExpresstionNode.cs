using System;
using System.Collections.Generic;
using System.Linq;
using Cirno.Lexer;

namespace Cirno.Expressions;

public abstract class ExpressionNode {
    public abstract SyntaxKind Kind { get; }

    // protected internal virtual ExpressionNode VisitChildren(ExpressionVisitor visitor) {
    //     return visitor.Visit(this);
    // }

    // protected internal virtual ExpressionNode Accept(ExpressionVisitor visitor) {
    //     return visitor.VisitExtension(this);
    // }

    public abstract IEnumerable<ExpressionNode> GetChildren();

    // public static void PrettyPrint(ExpressionNode node) {
    //     PrettyPrint(node);
    // }

    public static void PrettyPrint(ExpressionNode node, string indent = "", bool isLast = true)
    {
        // ├─ │ └─

        var marker = isLast ? "└───" : "├───";
        // var marker = isLast ? "└" : "├";

        var prevColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(indent);
        Console.Write(marker);
        Console.ForegroundColor = prevColor;


        if (node is SyntaxToken token) {
            prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" " + node.Kind);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($" =>");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($" Word: \"{token.Name}\";");

            if (token is SyntaxTokenWithValue<int> t) {
                Console.Write($" Value: {t.Value};");
            }
            Console.Write($" In {token.Line}:{token.Position}");

            Console.ForegroundColor = prevColor;
        } else {
            Console.Write(" " + node.Kind);
            if (node is VariableExpression varNode && varNode.IsArrayRef) {
                Console.Write(" " + "IsArrayRef");
            }
        }

        Console.WriteLine();

        indent += isLast ? "    " : "│   ";
        // indent += isLast ? " " : "│";

        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            PrettyPrint(child, indent, child == lastChild);
    }
}

public sealed class NumberExpression(SyntaxToken numberToken) : ExpressionNode
{
    public SyntaxToken NumberToken { get; private set; } = numberToken;

    public override SyntaxKind Kind { get; } = SyntaxKind.NumberExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return NumberToken;
    }

    // protected internal override ExpressionNode Accept(ExpressionVisitor visitor)
    // {
    //     return visitor.Visit(this);
    // }


}

public sealed class BinaryExpression(ExpressionNode left, SyntaxToken opToken, ExpressionNode right) : ExpressionNode {
    public override SyntaxKind Kind { get; } = opToken.Kind switch {
            SyntaxKind.Plus => SyntaxKind.AdditionExpression,
            SyntaxKind.Minus => SyntaxKind.SubtractionExpression,
            SyntaxKind.Asterisk => SyntaxKind.MultiplicationExpression,
            SyntaxKind.Slash => SyntaxKind.DivisionExpression,
            SyntaxKind.LessThan => SyntaxKind.LessThanExpression,
            SyntaxKind.LessThanOrEqualTo => SyntaxKind.LessThanOrEqualToExpression,
            SyntaxKind.EqualTo => SyntaxKind.EqualToExpression,
            SyntaxKind.GreaterThanOrEqualTo => SyntaxKind.GreaterThanOrEqualToExpression,
            SyntaxKind.GreaterThan => SyntaxKind.GreaterThanExpression,
            SyntaxKind.NotEqualTo => SyntaxKind.NotEqualToExpression,
            _ => throw new System.Exception($"Unknown expression {opToken}"),
        };

    public ExpressionNode Left { get; private set; } = left;

    public SyntaxToken Operator { get; private set; } = opToken;

    public ExpressionNode Right { get; private set; } = right;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }

    // public static int GetOperatorPriority(SyntaxKind)
    
    // public static Dictionary<SyntaxKind, int> OpertorPriority { get; private set; }

    // static BinaryExpression() {
    //     OpertorPriority = new Dictionary<SyntaxKind, int>() {
    //         {SyntaxKind.Plus, 1},
    //         {SyntaxKind.Minus, 1},
    //         {SyntaxKind.Asterisk, 50},
    //         {SyntaxKind.Slash, 50},
    //     };
    // }
}

public sealed class DeclarationListExpression(ExpressionNode declaration, ExpressionNode? declarationListTail) : ExpressionNode
{
    public ExpressionNode Declaration { get; } = declaration;
    public ExpressionNode? DeclarationListTail { get; } = declarationListTail;

    public override SyntaxKind Kind { get; } = SyntaxKind.DeclarationListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Declaration;
        if (DeclarationListTail is not null)
            yield return DeclarationListTail;
    }

    // protected internal override ExpressionNode Accept(ExpressionVisitor visitor)
    // {
    //     return visitor.Visit(this);
    // }
}

public sealed class DeclarationListTailExpression(ExpressionNode declaration, ExpressionNode? declarationListTail) : ExpressionNode
{
    public ExpressionNode Declaration { get; } = declaration;
 
    public ExpressionNode? DeclarationListTail { get; } = declarationListTail;

    public override SyntaxKind Kind => SyntaxKind.DeclarationListTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Declaration;
        if (DeclarationListTail is not null)
                yield return DeclarationListTail;
    }
}

// public sealed class DeclarationExpression : ExpressionNode
// {


//     public override SyntaxKind Kind { get; } = SyntaxKind.DeclarationExpression;
// }

public sealed class ParamsExpression(ExpressionNode expr) : ExpressionNode
{
    /// <summary>
    /// maybe list or void type
    /// </summary>
    /// <value></value>
    public ExpressionNode Expr { get; } = expr;

    public override SyntaxKind Kind => SyntaxKind.ParamExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Expr;
    }
}

public sealed class ParamExpression(SyntaxToken typeToken, SyntaxToken identifierToken) : ExpressionNode
{
    public SyntaxToken TypeToken { get; } = typeToken;

    public SyntaxToken IdentifierToken { get; } = identifierToken;

    public override SyntaxKind Kind => SyntaxKind.ParamExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return TypeToken;
        yield return IdentifierToken;
    }
}

public sealed class ParamListExpression(ExpressionNode param, ExpressionNode? paramListTail) : ExpressionNode
{
    public ExpressionNode Param { get; } = param;

    public ExpressionNode? ParamListTail { get; } = paramListTail;

    public override SyntaxKind Kind => SyntaxKind.ParamListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Param;
        if (ParamListTail is not null)
            yield return ParamListTail;
    }
}

public sealed class ParamListTailExpression(ExpressionNode param, ExpressionNode? paramListTail) : ExpressionNode
{
    public ExpressionNode Param { get; } = param;
    public ExpressionNode? ParamListTail { get; } = paramListTail;

    public override SyntaxKind Kind => SyntaxKind.ParamListTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Param;
        if (ParamListTail is not null)
            yield return ParamListTail;
    }
}

public sealed class FunctionExpression(SyntaxToken retType, SyntaxToken fnName, ExpressionNode paramsList, ExpressionNode compoundStatement) : ExpressionNode
{
    public SyntaxToken RetType { get; } = retType;
    public SyntaxToken FnName { get; } = fnName;
    public ExpressionNode ParamsList { get; } = paramsList;
    public ExpressionNode CompoundStatement { get; } = compoundStatement;

    public override SyntaxKind Kind => SyntaxKind.FunctionExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return RetType;
        yield return FnName;
        yield return ParamsList;
        yield return CompoundStatement;
    }
}

public sealed class VarDeclarationExpression(SyntaxToken type, SyntaxToken identifier, SyntaxToken? arrayLength = null) : ExpressionNode
{
    public SyntaxToken Type { get; } = type;
    
    public SyntaxToken Identifier { get; } = identifier;
    
    public SyntaxToken? ArrayLength { get; } = arrayLength;

    public override SyntaxKind Kind => SyntaxKind.VarDeclarationExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Type;
        yield return Identifier;
        if (ArrayLength is not null)
            yield return ArrayLength;
    }
}

public sealed class CompoundStatementExpression(ExpressionNode? localDeclarations, ExpressionNode? statementList) : ExpressionNode
{
    public ExpressionNode? LocalDeclarations { get; } = localDeclarations;
    
    public ExpressionNode? StatementList { get; } = statementList;

    public override SyntaxKind Kind => SyntaxKind.CompoundStatementExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (LocalDeclarations is not null)
            yield return LocalDeclarations;
        if (StatementList is not null)
            yield return StatementList;
    }
}

public sealed class LocalDeclarationsExpression(ExpressionNode varDeclaration, ExpressionNode? localDeclarationTail) : ExpressionNode
{
    public ExpressionNode VarDeclaration { get; } = varDeclaration;
    
    public ExpressionNode? LocalDeclarationTail { get; } = localDeclarationTail;

    public override SyntaxKind Kind => SyntaxKind.LocalDeclarationsExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return VarDeclaration;
        if (LocalDeclarationTail is not null)
            yield return LocalDeclarationTail;
    }
}

public sealed class LocalDeclarationsTailExpression(ExpressionNode varDeclaration, ExpressionNode? localDeclarationTail) : ExpressionNode
{
    public ExpressionNode VarDeclaration { get; } = varDeclaration;

    public ExpressionNode? LocalDeclarationTail { get; } = localDeclarationTail;

    public override SyntaxKind Kind => SyntaxKind.LocalDeclarationTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return VarDeclaration;
        if (LocalDeclarationTail is not null)
            yield return LocalDeclarationTail;
    }
}

public sealed class ReturnExpression(ExpressionNode? expression = null) : ExpressionNode
{
    public ExpressionNode? Expression { get; } = expression;

    public override SyntaxKind Kind => SyntaxKind.ReturnExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}

public sealed class SelectionStatementExpression(ExpressionNode logicExpr, ExpressionNode? firstStatement = null, ExpressionNode? secondStatement = null) : ExpressionNode
{
    public ExpressionNode LogicExpr { get; } = logicExpr;

    public ExpressionNode? FirstStatementsList { get; } = firstStatement;

    public ExpressionNode? SecondStatementsList { get; } = secondStatement;

    public override SyntaxKind Kind => SyntaxKind.SelectionStatementExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return LogicExpr;
        if (FirstStatementsList is not null)
            yield return FirstStatementsList;
        if (SecondStatementsList is not null)
            yield return SecondStatementsList;
    }
}

public sealed class StatementListExpression(ExpressionNode statementListTail) : ExpressionNode
{
    public ExpressionNode StatementListTail { get; } = statementListTail;

    public override SyntaxKind Kind => SyntaxKind.StatementListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return StatementListTail;
    }
}

public sealed class StatementListTailExpression(ExpressionNode statement, ExpressionNode? statementListTail = null) : ExpressionNode
{
    public ExpressionNode Statement { get; } = statement;

    public ExpressionNode? StatementListTail { get; } = statementListTail;

    public override SyntaxKind Kind => SyntaxKind.StatementListTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Statement;
        if (StatementListTail is not null)
            yield return StatementListTail;
    }
}

public sealed class IterationStatementExpression(ExpressionNode logicExpr, ExpressionNode? block) : ExpressionNode
{
    public ExpressionNode LogicExpr { get; } = logicExpr;
    
    public ExpressionNode? Block  { get; } = block ;

    public override SyntaxKind Kind => SyntaxKind.IterationStatementExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return LogicExpr;
        if (Block is not null)
            yield return Block;
    }
}

public sealed class AssignmentExpression(ExpressionNode variable, ExpressionNode expr) : ExpressionNode
{
    public ExpressionNode Variable { get; } = variable;
    
    public ExpressionNode Expr { get; } = expr;

    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Variable;
        yield return Expr;
    }
}

// wtf ExpressionStatementExpression
public sealed class ExpressionStatementExpression(ExpressionNode? expression = null) : ExpressionNode
{
    public ExpressionNode? Expression { get; } = expression;

    public override SyntaxKind Kind => SyntaxKind.ExpressionStatementExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}

public sealed class VariableExpression(SyntaxToken identifier, ExpressionNode? offsetExpr = null, bool isArrayRef = false) : ExpressionNode
{
    public SyntaxToken Identifier { get; } = identifier;

    public ExpressionNode? OffsetExpr { get; } = offsetExpr;

    public bool IsArrayRef { get; } = isArrayRef;

    public override SyntaxKind Kind => SyntaxKind.VariableExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Identifier;
        if (OffsetExpr is not null)
            yield return OffsetExpr;        
    }
}

public sealed class SimpleExpression(ExpressionNode additionExpr1, SyntaxToken? op = null, ExpressionNode? additionExpr2 = null) : ExpressionNode
{
    public ExpressionNode AdditionExpr1 { get; } = additionExpr1;

    public SyntaxToken? Op { get; } = op;

    public ExpressionNode? AdditionExpr2 { get; } = additionExpr2;

    public override SyntaxKind Kind => SyntaxKind.SimpleExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return AdditionExpr1;
        if (Op is not null)
            yield return Op;
        if (AdditionExpr2 is not null)
            yield return AdditionExpr2;
    }
}

public sealed class AdditiveExpression(ExpressionNode term, ExpressionNode? additiveExpressionTail = null) : ExpressionNode
{
    public ExpressionNode Term { get; } = term;
    public ExpressionNode? AdditiveExpressionTail { get; } = additiveExpressionTail;

    public override SyntaxKind Kind => SyntaxKind.AdditiveExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Term;
        if (AdditiveExpressionTail is not null)
            yield return AdditiveExpressionTail;
    }
}

public sealed class AdditiveTailExpression(SyntaxToken op, ExpressionNode term, ExpressionNode? additiveExpressionTail) : ExpressionNode
{
    public SyntaxToken Op { get; } = op;
    public ExpressionNode Term { get; } = term;
    public ExpressionNode? AdditiveExpressionTail { get; } = additiveExpressionTail;

    public override SyntaxKind Kind => SyntaxKind.AdditiveTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Op;
        yield return Term;
        if (AdditiveExpressionTail is not null)
            yield return AdditiveExpressionTail;
    }
}

public sealed class TermExpression(ExpressionNode factor, ExpressionNode? termTail) : ExpressionNode
{
    public ExpressionNode Factor { get; } = factor;
    
    public ExpressionNode? TermTail { get; } = termTail;

    public override SyntaxKind Kind => SyntaxKind.TermExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Factor;
        if (TermTail is not null)
            yield return TermTail;
    }
}

public sealed class TermTailExpression(SyntaxToken op, ExpressionNode factor, ExpressionNode? termTail) : ExpressionNode
{
    public SyntaxToken Op { get; } = op;

    public ExpressionNode Factor { get; } = factor;

    public ExpressionNode? TermTail { get; } = termTail;

    public override SyntaxKind Kind => SyntaxKind.TermTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Op;
        yield return Factor;
        if (TermTail is not null)
            yield return TermTail;
    }
}

public sealed class FactorExpression(ExpressionNode expr) : ExpressionNode
{
    public ExpressionNode Expr { get; } = expr;

    public override SyntaxKind Kind => SyntaxKind.FactorExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Expr;
    }
}

public sealed class CallFunctionExpression(SyntaxToken identifier, ExpressionNode args) : ExpressionNode
{
    public SyntaxToken Identifier { get; } = identifier;
    public ExpressionNode Args { get; } = args;

    public override SyntaxKind Kind => SyntaxKind.CallFunctionExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Identifier;
        yield return Args;
    }
}

public sealed class CallFuncArgsExpression(SyntaxToken identifier, ExpressionNode args) : ExpressionNode
{
    public SyntaxToken Identifier { get; } = identifier;

    public ExpressionNode Args { get; } = args;

    public override SyntaxKind Kind => SyntaxKind.CallFuncArgsExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Identifier;
        yield return Args;
    }
}

public sealed class ArgsExpression(ExpressionNode? argsList = null) : ExpressionNode
{
    public ExpressionNode? ArgsList { get; } = argsList;

    public override SyntaxKind Kind => SyntaxKind.ArgsExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (ArgsList is not null)
            yield return ArgsList;
    }
}

public sealed class ArgsListExpression(ExpressionNode expr, ExpressionNode? argsListTail) : ExpressionNode
{
    public ExpressionNode Expr { get; } = expr;

    public ExpressionNode? ArgsListTail { get; } = argsListTail;

    public override SyntaxKind Kind => SyntaxKind.ArgsListExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        yield return Expr;
        if (ArgsListTail is not null)
            yield return ArgsListTail;
    }
}

public sealed class ArgsListTailExpression(ExpressionNode? expr, ExpressionNode? argsListTail) : ExpressionNode
{
    public ExpressionNode? Expr { get; } = expr;

    public ExpressionNode? ArgsListTail { get; } = argsListTail;

    public override SyntaxKind Kind => SyntaxKind.ArgsListTailExpression;

    public override IEnumerable<ExpressionNode> GetChildren()
    {
        if (Expr is not null)
            yield return Expr;
        if (ArgsListTail is not null)
            yield return ArgsListTail;
    }
}

// public sealed class SimpleExpression(ExpressionNode additionExpr, SyntaxToken op, ExpressionNode termExpr) : ExpressionNode
// {
//     public ExpressionNode AdditionExpr { get; } = additionExpr;
//     public SyntaxToken Op { get; } = op;
//     public ExpressionNode TermExpr { get; } = termExpr;

//     public override SyntaxKind Kind => throw new System.NotImplementedException();

//     public override IEnumerable<ExpressionNode> GetChildren()
//     {
//         throw new System.NotImplementedException();
//     }
// }
