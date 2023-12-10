using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using Cirno.Expressions;

namespace Cirno;

public class Parser
{
    public Parser(string[] texts) {
        var lexer = new Cirno.Lexer(texts);

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

    private SyntaxToken[] _tokens;

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

        Cirno.Diagostics.DiagosticHelper.Raise($"Unexpected token {Current}, expected kind {kind}.");

        if (kind is not SyntaxKind.Type)
            return new SyntaxToken(kind, Current.Name, Current.Line, Current.Position);
        else
            return new SyntaxToken(SyntaxKind.Void, Current.Name, Current.Line, Current.Position);
    }

    public ExpressionTree Parse() {
        var root = ParseProgram();
        if (!Current.IsEndOfFileToken()) 
            Diagostics.DiagosticHelper.Raise("Code ends early.");
        return new ExpressionTree(root);
    }

    private ExpressionNode ParseProgram() {
        return ParseDeclationList();
    }

    private ExpressionNode ParseDeclation() {
        if (Current.Kind is SyntaxKind.Void || Peek(2).Kind is SyntaxKind.OpenRoundBrackets) {
            return ParseFunDeclaration();   
        }
        return ParseVarDeclaration();
    }

    private ExpressionNode ParseDeclationList() {
        var declation = ParseDeclation();
        var declarationListTail = ParseDeclationListTail();
        return new DeclarationListExpression(declation, declarationListTail);
    }

    private ExpressionNode? ParseDeclationListTail() {
        if (Current.Kind is SyntaxKind.Int or SyntaxKind.Void) {
            var declaration = ParseDeclation();
            var declarationListTail = ParseDeclationListTail();
            return new DeclarationListTailExpression(declaration, declarationListTail);
        }
        return null;
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
        var localDecarationTail = ParseLocalDeclarationsTail();

        return new LocalDecarationsExpression(varDeclaration, localDecarationTail);
    }

    private ExpressionNode? ParseLocalDeclarationsTail() {
        if (!Current.Kind.IsType()) {
            return null;
        }

        var varDeclaration = ParseVarDeclaration();
        var localDecarationTail = ParseLocalDeclarationsTail(); 
        return new LocalDecarationsTailExpression(varDeclaration, localDecarationTail);
    }

    private ExpressionNode ParseStatement() {
        // while (Current.Kind is SyntaxKind.Semicolon) {
        //     _ = Match(SyntaxKind.Semicolon);
        // }

        if (Current.Kind is SyntaxKind.If) {
            return ParseSelectionStatement();
        }

        if (Current.Kind is SyntaxKind.While) {
            return ParseIterationStatement();
        }

        if (Current.Kind is SyntaxKind.Return) {
            return ParseReturnStatement();
        }

        return ParseExpressionStatement();  // assign or binary
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
        if (Current.Kind is not SyntaxKind.Semicolon) {
            expr = ParseCalculateExpression();
            _ = Match(SyntaxKind.Semicolon);
        }
        return new ReturnExpression(expr);
    }

    private ExpressionNode ParseSelectionStatement() {
        _ = Match(SyntaxKind.If);
        _ = Match(SyntaxKind.OpenRoundBrackets);
        var logicExpr = ParseCalculateExpression();
        _ = Match(SyntaxKind.CloseRoundBrackets);

        ExpressionNode? firstStatementsList;
        if (Current.Kind is SyntaxKind.OpenCurlyBrackets) {
            // _ = Match(SyntaxKind.OpenCurlyBrackets);
            firstStatementsList = ParseCompoundStatement();
            // _ = Match(SyntaxKind.CloseCurlyBrackets);
        } else {
            firstStatementsList = ParseStatement();
        }
        
        if (Current.Kind is SyntaxKind.Else) {
            _ = Match(SyntaxKind.Else);

            ExpressionNode? secondStatemnet;

            if (Current.Kind is SyntaxKind.OpenCurlyBrackets) {
                // _ = Match(SyntaxKind.OpenCurlyBrackets);
                secondStatemnet = ParseCompoundStatement();
                // _ = Match(SyntaxKind.CloseCurlyBrackets);
            } else {
                secondStatemnet = ParseStatement();
            }
            
            return new SelectionStatementExpression(
                logicExpr, firstStatementsList, secondStatemnet
            );
        }
        return new SelectionStatementExpression(logicExpr, firstStatementsList);
    }

    private ExpressionNode ParseIterationStatement() {
        _ = Match(SyntaxKind.While);
        _ = Match(SyntaxKind.OpenRoundBrackets);
        var logicExpr = ParseCalculateExpression();
        _ = Match(SyntaxKind.CloseRoundBrackets);

        ExpressionNode? compoundStatement;
        if (Current.Kind is SyntaxKind.OpenCurlyBrackets) {
            // Match(SyntaxKind.OpenCurlyBrackets);
            compoundStatement = ParseCompoundStatement();
            // Match(SyntaxKind.CloseCurlyBrackets);
        } else {
            compoundStatement = ParseStatement();
        }

        // var compoundStatement = ParseCompoundStatement();

        return new IterationStatementEpxression(logicExpr, compoundStatement);   
    }

    private ExpressionNode ParseCalculateExpression() {
        // if (Peek(1).Kind is SyntaxKind.Assign)
        //     return ParseAssignmentExpression();

        // return ParseSimpleExpression();
    
        if (Current.Kind is SyntaxKind.Identifier) {
            var prevPos = _position;
            var variable = ParseVariable();

            if (Current.Kind is SyntaxKind.Assign) {
                return ParseAssignmentExpression(variable);
            }
            _position = prevPos;
        }

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
            
            if (expr is null) {
                Cirno.Diagostics.DiagosticHelper.Raise($"Unexpected empty expression.");
            }

            return new VariableExpression(identifier, expr, true);
        } else{ 
            var identifier = Match(SyntaxKind.Identifier);
            return new VariableExpression(identifier);
        }
    }

    private ExpressionNode ParseSimpleExpression() {
        var additionExpr = ParseAdditiveExpression();
        if (Current.IsRelopToken()) {
            var op = NextToken();
            var additionExpr2 = ParseAdditiveExpression();
            return new SimpleExpression(additionExpr, op, additionExpr2);
        }
        return new SimpleExpression(additionExpr);
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
        if (Current.Kind is SyntaxKind.Plus or SyntaxKind.Minus) {
            var op = NextToken();
            var term = ParseTerm();
            var additiveExpressionTail = ParseAdditiveTailExpression();
            return new AdditiveTailExpression(op, term, additiveExpressionTail);
        }
        return null;
    }

    private ExpressionNode ParseTerm() {
        var factor = ParseFactor();
        var termTail = ParseTermTail();
        return new TermExpression(factor, termTail);
    }

    private ExpressionNode? ParseTermTail() {
        if (Current.Kind is SyntaxKind.Asterisk or SyntaxKind.Slash) {
            var op = NextToken();
            var factor = ParseFactor();
            var temrTail = ParseTermTail();
            return new TermTailExpression(op, factor, temrTail);
        }
        return null;
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
