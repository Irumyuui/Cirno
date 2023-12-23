using System.Collections.Generic;
using Cirno.Expressions;
using Cirno.Lexer;

namespace Cirno.Parser;

public class Parser
{
    public Parser(string[] texts) {
        var lexer = new Lexer.Lexer(texts);

        SyntaxToken curToken;
        List<SyntaxToken> tokenList = [];

        do {
            curToken = lexer.NextToken();
            if (curToken.Kind is not SyntaxKind.Unknown and not SyntaxKind.CommentsEnd) {
                tokenList.Add(curToken);
            }
        } while (curToken.Kind is not SyntaxKind.EndOfFile);

        _tokens = [.. tokenList];
    }

    private readonly SyntaxToken[] _tokens;

    private int _position;

    private SyntaxToken Peek(int offset) {
        var index = _position + offset;
        if (index >= _tokens.Length)
            return _tokens[^1];
        return _tokens[index];
    }

    private SyntaxToken Current => Peek(0);

    private void MoveNextPosition() => _position ++;

    private SyntaxToken NextToken() {
        var prevToken = Current;
        MoveNextPosition();
        return prevToken;
    }

    private SyntaxToken Match(SyntaxKind kind) {
        if (Current.Kind == kind) {
            return NextToken();
        }

        if (kind is SyntaxKind.Type && Current.IsTypeToken()) {
            return NextToken();
        }

        // on expression
        if (kind is SyntaxKind.NumberOrIdentifier && Current.Kind is SyntaxKind.Number or SyntaxKind.Identifier) {
            return NextToken();
        }

        Cirno.Diagnostic.DiagnosticHelper.Raise($"Unexpected token {Current}, expected kind {kind}.");

        return kind is not SyntaxKind.Type ? new SyntaxToken(kind, Current.Name, Current.Line, Current.Position) : new SyntaxToken(SyntaxKind.Void, Current.Name, Current.Line, Current.Position);
    }

    public ExpressionTree Parse() {
        var root = ParseProgram();
        if (!Current.IsEndOfFileToken()) 
            Diagnostic.DiagnosticHelper.Raise("Code ends early.");
        return new ExpressionTree(root);
    }

    private ExpressionNode ParseProgram() {
        return ParseDeclarationList();
    }

    private ExpressionNode ParseDeclaration() {
        if (Current.Kind is SyntaxKind.Void || Peek(2).Kind is SyntaxKind.OpenRoundBrackets) {
            return ParseFunDeclaration();   
        }
        return ParseVarDeclaration();
    }

    private ExpressionNode ParseDeclarationList() {
        var declaration = ParseDeclaration();
        var declarationListTail = ParseDeclarationListTail();
        return new DeclarationListExpression(declaration, declarationListTail);
    }

    private ExpressionNode? ParseDeclarationListTail() {
        if (Current.Kind is not (SyntaxKind.Int or SyntaxKind.Void)) return null;
        var declaration = ParseDeclaration();
        var declarationListTail = ParseDeclarationListTail();
        return new DeclarationListTailExpression(declaration, declarationListTail);
    }

    private ExpressionNode ParseVarDeclaration() {
        var type = Match(SyntaxKind.Type);
        var identifier = Match(SyntaxKind.Identifier);

        VarDeclarationExpression expr;

        if (Current.Kind is SyntaxKind.OpenSquareBrackets) {
            _ = Match(SyntaxKind.OpenSquareBrackets);
            var arrayLength = Match(SyntaxKind.Number);
            _ = Match(SyntaxKind.CloseSquareBrackets);
            expr = new VarDeclarationExpression(type, identifier, arrayLength);
        } else {
            expr = new VarDeclarationExpression(type, identifier);
        }
        _ = Match(SyntaxKind.Semicolon);
        
        return expr;
    }

    private ExpressionNode ParseFunDeclaration() {
        var retType = Match(SyntaxKind.Type);
        var identifier = Match(SyntaxKind.Identifier);
        
        _ = Match(SyntaxKind.OpenRoundBrackets);
        var @params = ParseParams();
        _ = Match(SyntaxKind.CloseRoundBrackets);

        var compoundStatement = ParseCompoundStatement();

        return new FunctionExpression(retType, identifier, @params, compoundStatement);
    }

    private ExpressionNode ParseParams() {
        if (Current.Kind is SyntaxKind.Void && Peek(1).Kind is SyntaxKind.CloseRoundBrackets)
            return Match(SyntaxKind.Void);
        
        var paramList = ParseParamList();
        return paramList;
    }

    private ExpressionNode ParseParamList() {
        var param = ParseParam();
        // _ = Match(SyntaxKind.Comma);
        var paramListTail = ParseParamListTail();
        return new ParamListExpression(param, paramListTail);
    }

    private ExpressionNode ParseParam() {
        var type = Match(SyntaxKind.Int);
        var identifier = Match(SyntaxKind.Identifier);
        if (Current.Kind is SyntaxKind.OpenSquareBrackets) {
            _ = Match(SyntaxKind.OpenSquareBrackets);
            _ = Match(SyntaxKind.CloseSquareBrackets);
            var arrayType = new SyntaxToken(SyntaxKind.IntPtrRef, type.Name, type.Line, type.Position);
            return new ParamExpression(arrayType, identifier);
        }
        return new ParamExpression(type, identifier);
    }

    private ExpressionNode? ParseParamListTail() {
        if (Current.Kind is not SyntaxKind.Comma) {
            return null;
        }
        _ = Match(SyntaxKind.Comma);
        var param = ParseParam();
        var paramListTail = ParseParamListTail();
        return new ParamListTailExpression(param, paramListTail);
    }

    private ExpressionNode ParseCompoundStatement() {
        _ = Match(SyntaxKind.OpenCurlyBrackets);

        var localDeclarations = ParseLocalDeclarations();
        var statementList = ParseStatementsList();

        _ = Match(SyntaxKind.CloseCurlyBrackets);
        
        return new CompoundStatementExpression(localDeclarations, statementList);
    }

    private ExpressionNode? ParseStatementsList() {
        // while (Current.Kind is SyntaxKind.Semicolon) {
        //     MoveNextPosition();
        // }
        return ParseStatementListTail();
    }

    private ExpressionNode? ParseStatementListTail() {
        if (Current.Kind is SyntaxKind.Identifier
                         or SyntaxKind.Number
                         or SyntaxKind.If
                         or SyntaxKind.While
                         or SyntaxKind.Return
                         or SyntaxKind.Semicolon
        ) {
            var statement = ParseStatement();
            var statementListTail = ParseStatementListTail();
            return new StatementListTailExpression(statement, statementListTail);
        }
        return null;
    }

    private ExpressionNode? ParseLocalDeclarations() {
        if (!Current.Kind.IsType()) {
            return null;
        }

        var varDeclaration = ParseVarDeclaration();
        var localDeclarationTail = ParseLocalDeclarationsTail();

        return new LocalDeclarationsExpression(varDeclaration, localDeclarationTail);
    }

    private ExpressionNode? ParseLocalDeclarationsTail() {
        if (!Current.Kind.IsType()) {
            return null;
        }

        var varDeclaration = ParseVarDeclaration();
        var localDeclarationTail = ParseLocalDeclarationsTail(); 
        return new LocalDeclarationsTailExpression(varDeclaration, localDeclarationTail);
    }

    private ExpressionNode ParseStatement() {
        // while (Current.Kind is SyntaxKind.Semicolon) {
        //     _ = Match(SyntaxKind.Semicolon);
        // }

        return Current.Kind switch
        {
            SyntaxKind.If => ParseSelectionStatement(),
            SyntaxKind.While => ParseIterationStatement(),
            SyntaxKind.Return => ParseReturnStatement(),
            _ => ParseExpressionStatement()
        };
    }

    private ExpressionNode ParseExpressionStatement() {
        if (Current.Kind is SyntaxKind.Semicolon) {
            _ = Match(SyntaxKind.Semicolon);
            return new ExpressionStatementExpression();
        }
        var expr = ParseCalculateExpression();
        _ = Match(SyntaxKind.Semicolon);
        return new ExpressionStatementExpression(expr);
    }

    private ExpressionNode ParseReturnStatement() {
        _ = Match(SyntaxKind.Return);
        ExpressionNode? expr = null;
        if (Current.Kind is SyntaxKind.Semicolon) return new ReturnExpression(expr);
        expr = ParseCalculateExpression();
        _ = Match(SyntaxKind.Semicolon);
        return new ReturnExpression(expr);
    }

    private ExpressionNode ParseSelectionStatement() {
        _ = Match(SyntaxKind.If);
        _ = Match(SyntaxKind.OpenRoundBrackets);
        var logicExpr = ParseCalculateExpression();
        _ = Match(SyntaxKind.CloseRoundBrackets);

        var firstStatementsList =
            // _ = Match(SyntaxKind.OpenCurlyBrackets);
            Current.Kind is SyntaxKind.OpenCurlyBrackets ? ParseCompoundStatement() :
            // _ = Match(SyntaxKind.CloseCurlyBrackets);
            ParseStatement();

        if (Current.Kind is not SyntaxKind.Else)
            return new SelectionStatementExpression(logicExpr, firstStatementsList);
        _ = Match(SyntaxKind.Else);

        var secondStatement =
            // _ = Match(SyntaxKind.OpenCurlyBrackets);
            Current.Kind is SyntaxKind.OpenCurlyBrackets ? ParseCompoundStatement() :
            // _ = Match(SyntaxKind.CloseCurlyBrackets);
            ParseStatement();
            
        return new SelectionStatementExpression(
            logicExpr, firstStatementsList, secondStatement
        );
    }

    private ExpressionNode ParseIterationStatement() {
        _ = Match(SyntaxKind.While);
        _ = Match(SyntaxKind.OpenRoundBrackets);
        var logicExpr = ParseCalculateExpression();
        _ = Match(SyntaxKind.CloseRoundBrackets);

        var compoundStatement =
            // Match(SyntaxKind.OpenCurlyBrackets);
            Current.Kind is SyntaxKind.OpenCurlyBrackets ? ParseCompoundStatement() :
            // Match(SyntaxKind.CloseCurlyBrackets);
            ParseStatement();

        // var compoundStatement = ParseCompoundStatement();
        return new IterationStatementExpression(logicExpr, compoundStatement);   
    }

    private ExpressionNode ParseCalculateExpression() {
        // if (Peek(1).Kind is SyntaxKind.Assign)
        //     return ParseAssignmentExpression();

        // return ParseSimpleExpression();

        if (Current.Kind is not SyntaxKind.Identifier) return ParseSimpleExpression();
        var prevPos = _position;
        var variable = ParseVariable();

        if (Current.Kind is SyntaxKind.Assign) {
            return ParseAssignmentExpression(variable);
        }
        _position = prevPos;

        return ParseSimpleExpression();
    }

    private ExpressionNode ParseAssignmentExpression(ExpressionNode variable) {
        // var identifier = Match(SyntaxKind.Identifier);
        // var variable = ParseVariable();
        _ = Match(SyntaxKind.Assign);

        var expr = ParseCalculateExpression();

        return new AssignmentExpression(variable, expr);
    }

    private ExpressionNode ParseVariable() {
        if (Peek(1).Kind is SyntaxKind.OpenSquareBrackets) {
            var identifier = Match(SyntaxKind.Identifier);

            _ = Match(SyntaxKind.OpenSquareBrackets);
            var expr = ParseCalculateExpression();
            _ = Match(SyntaxKind.CloseSquareBrackets);

            return new VariableExpression(identifier, expr, true);
        } else{ 
            var identifier = Match(SyntaxKind.Identifier);
            return new VariableExpression(identifier);
        }
    }

    private ExpressionNode ParseSimpleExpression() {
        var additionExpr = ParseAdditiveExpression();
        if (!Current.IsRelopToken()) return new SimpleExpression(additionExpr);
        var op = NextToken();
        var additionExpr2 = ParseAdditiveExpression();
        return new SimpleExpression(additionExpr, op, additionExpr2);
        // if (Peek(1).IsRelopToken()) {
        //     var additiveExpr = ParseAdditiveExpression();
        //     var op = NextToken();
        //     var term = ParseTerm();
        //     return new SimpleExpression(additiveExpr, op, term);
        // } else {
        //     var term = ParseTerm();
        //     return new SimpleExpression(null, null, term);
        // }
    }

    private ExpressionNode ParseAdditiveExpression() {
        var term = ParseTerm();
        var additiveExpressionTail = ParseAdditiveTailExpression();
        return new AdditiveExpression(term, additiveExpressionTail);
    }

    private ExpressionNode? ParseAdditiveTailExpression() {
        if (Current.Kind is not (SyntaxKind.Plus or SyntaxKind.Minus)) return null;
        var op = NextToken();
        var term = ParseTerm();
        var additiveExpressionTail = ParseAdditiveTailExpression();
        return new AdditiveTailExpression(op, term, additiveExpressionTail);
    }

    private ExpressionNode ParseTerm() {
        var factor = ParseFactor();
        var termTail = ParseTermTail();
        return new TermExpression(factor, termTail);
    }

    private ExpressionNode? ParseTermTail() {
        if (Current.Kind is not (SyntaxKind.Asterisk or SyntaxKind.Slash)) return null;
        var op = NextToken();
        var factor = ParseFactor();
        var termTail = ParseTermTail();
        return new TermTailExpression(op, factor, termTail);
    }

    private ExpressionNode ParseFactor() {
        if (Current.Kind is SyntaxKind.OpenRoundBrackets) {
            _ = Match(SyntaxKind.OpenRoundBrackets);
            var expr = ParseCalculateExpression();
            _ = Match(SyntaxKind.CloseRoundBrackets);
            return new FactorExpression(expr);
        }

        if (Current.Kind is SyntaxKind.Identifier) {
            if (Peek(1).Kind is not SyntaxKind.OpenRoundBrackets)
                return new FactorExpression(ParseVariable());
            return new FactorExpression(ParseCallFunction());
        }

        var num = Match(SyntaxKind.Number);
        return new FactorExpression(num);
    }

    private ExpressionNode ParseCallFunction() {
        var identifier = Match(SyntaxKind.Identifier);
        _ = Match(SyntaxKind.OpenRoundBrackets);
        var args = ParseArgsExpression();
        _ = Match(SyntaxKind.CloseRoundBrackets);
        return new CallFuncArgsExpression(identifier, args);
    }

    private ExpressionNode ParseArgsExpression() {
        return new ArgsExpression(ParseArgsListExpression());
    }

    private ExpressionNode? ParseArgsListExpression() {
        if (Current.Kind is SyntaxKind.CloseRoundBrackets) {
            return null;
        }
        var expr = ParseCalculateExpression();
        var argsListTail = ParseArgsListTailExpression();
        return new ArgsListExpression(expr, argsListTail);
    }

    private ExpressionNode? ParseArgsListTailExpression() {
        if (Current.Kind is not SyntaxKind.Comma) {
            return null;
        }

        _ = Match(SyntaxKind.Comma);
        var expr = ParseCalculateExpression();
        var argsListTail = ParseArgsListTailExpression();
        return new ArgsListTailExpression(expr, argsListTail);
    }

    // private Stack<ExpressionNode> _stkVal = [];  // 单调栈
    // private Stack<SyntaxToken> _stkOp = [];

    // private void ConnetNode() {
    //     var right = _stkVal.Pop();
    //     var op = _stkOp.Pop();
    //     var left = _stkVal.Pop();
    //     _stkVal.Push(new BinaryExpression(left, op, right));
    // }

    // private ExpressionNode ParseBinaryExpression() {
    //     while (Current.Kind is (not SyntaxKind.Identifier or SyntaxKind.Number) and not SyntaxKind.EndOfFile) {
    //         Match(SyntaxKind.NumberOrIdentifier);
    //         MoveNextPosition();
    //     }

    //     if (Current.Kind is SyntaxKind.EndOfFile) {
    //         return Current;
    //     }
        
    //     _stkVal.Clear();
    //     _stkOp.Clear();

    //     _stkVal.Push(Current);
    //     while (Current.Kind is SyntaxKind.Number or SyntaxKind.Identifier or SyntaxKind.OpenRoundBrackets or open || Current.IsOperatorToken()) {
    //         if (Current.Kind is SyntaxKind.Number or SyntaxKind.Identifier) {
    //             _stkVal.Push(NextToken());
    //         } else if (Current.IsOperatorToken()) 
    //     }
    // }
}
