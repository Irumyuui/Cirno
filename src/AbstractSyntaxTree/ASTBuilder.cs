using System;
using System.Collections.Generic;
using Cirno.Expressions;
using Cirno.Lexer;

namespace Cirno.AbstractSyntaxTree;

internal sealed class ASTBuilder
{
    internal static void VisitProgram(in ExpressionNode exprNode, out ASTNode astNode)
    {
        // VisitDeclationList(exprNode, out astNode);
        if (exprNode is DeclarationListExpression decl)
        {
            VisitDeclationList(decl, out astNode);
        }
        else
        {
            throw new Exception($"Unexpected ExpressionNode type in {nameof(VisitProgram)}, expected ExpressionNode type is {nameof(DeclarationListExpression)}");
        }
    }

    private static void VisitDeclationList(in DeclarationListExpression expr, out ASTNode node)
    {
        List<DeclarationNode> declNodeList = [];

        // ExpressionNode? cur = exprNode.Declaration;
        switch (expr.Declaration)
        {
            case VarDeclarationExpression varDecl:
                {
                    VisitVarDeclation(varDecl, out var varCeclNode);
                    declNodeList.Add(varCeclNode);
                    break;
                }

            case FunctionExpression funcDecl:
                {
                    VisitFunctionDeclation(funcDecl, out var funcDeclNode);
                    declNodeList.Add(funcDeclNode);
                    break;
                }

            default:
                throw new Exception($"Unexpected ExpressionNode type in {nameof(VisitDeclationList)}, expected ExpressionNode type is {nameof(VarDeclarationExpression)} or {nameof(FunctionExpression)}");
        }

        var curExpr = expr.DeclarationListTail;
        while (curExpr is not null)
        {
            if (curExpr is not DeclarationListTailExpression tail)
                throw new Exception($"Unexpected ExpressionNode type in {nameof(VisitDeclationList)}, expected ExpressionNode type is {nameof(DeclarationListTailExpression)}");

            curExpr = tail.DeclarationListTail;

            switch (tail.Declaration)
            {
                case VarDeclarationExpression varDecl:
                    {
                        VisitVarDeclation(varDecl, out var varCeclNode);
                        declNodeList.Add(varCeclNode);
                        break;
                    }

                case FunctionExpression funcDecl:
                    {
                        VisitFunctionDeclation(funcDecl, out var funcDeclNode);
                        declNodeList.Add(funcDeclNode);
                        break;
                    }

                default:
                    throw new Exception($"Unexpected ExpressionNode type in {nameof(VisitDeclationList)}, expected ExpressionNode type is {nameof(VarDeclarationExpression)} or {nameof(FunctionExpression)}");
            }
        }

        node = new ProgramNode([.. declNodeList]);
        foreach (var item in declNodeList)
        {
            item.Parent = node;
        }
    }

    private static void VisitVarDeclation(in VarDeclarationExpression expr, out VariableDeclarationNode node)
    {
        if (expr.ArrayLength is SyntaxTokenWithValue<int> arrayLength)
        {
            node = new VariableDeclarationNode((expr.Identifier.Line, expr.Identifier.Position), null, expr.Identifier.Name, LiteralType.IntPtr, arrayLength.Value);
        }
        else if (expr.ArrayLength is not null)
        {
            node = new VariableDeclarationNode((expr.Identifier.Line, expr.Identifier.Position), null, expr.Identifier.Name, LiteralType.IntPtr, null);
        }
        else
        {
            node = new VariableDeclarationNode((expr.Identifier.Line, expr.Identifier.Position), null, expr.Identifier.Name, LiteralType.Int);
        }
    }

    private static void VisitFunctionDeclation(in FunctionExpression expr, out FunctionDeclarationNode node)
    {
        var funcName = expr.FnName.Name;

        List<LiteralType> functionType = [
            expr.RetType.Kind switch {
                SyntaxKind.Int => LiteralType.Int,
                SyntaxKind.Void => LiteralType.Void,
                _ => throw new Exception($"Unexpected {nameof(FunctionExpression)} return type {expr.RetType} in {nameof(VisitFunctionDeclation)}, expected int or void")
            }
        ];
        List<(string Name, LiteralType Type)> paramters = [];

        switch (expr.ParamsList)
        {
            case SyntaxToken voidParamToken:
                if (voidParamToken.Kind is not SyntaxKind.Void)
                {
                    throw new Exception($"Unexpected function params {voidParamToken} in {nameof(VisitFunctionDeclation)} which function is {funcName}, expected void");
                }
                // functionType.Add(LiteralType.Void);
                break;
            case ParamListExpression paramList:
                {

                    if (paramList.Param is not ParamExpression firstParm)
                        throw new Exception($"Unexpected function params in {nameof(VisitFunctionDeclation)} which function is {funcName}, expected {nameof(ParamExpression)}");

                    paramters.Add((
                        firstParm.IdentifierToken.Name,
                        Type: firstParm.TypeToken.Kind switch
                        {
                            SyntaxKind.Int => LiteralType.Int,
                            SyntaxKind.IntPtrRef => LiteralType.IntPtr,
                            _ => throw new Exception($"Unexpected function params type {firstParm.TypeToken.Kind} in {nameof(VisitFunctionDeclation)} which function is {funcName}, expected {nameof(ParamExpression)}")
                        }
                    ));

                    var paramListTail = paramList.ParamListTail;
                    while (paramListTail is not null)
                    {
                        if (paramListTail is not ParamListTailExpression tail)
                            throw new Exception($"Unexpected exprssion type in {nameof(VisitFunctionDeclation)}, expected ExpressionNode type is {nameof(ParamListTailExpression)} or null");

                        paramListTail = tail.ParamListTail;
                        if (tail.Param is not ParamExpression cur)
                        {
                            throw new Exception($"Unexpected exprssion type in {nameof(VisitFunctionDeclation)}, expected ExpressionNode type is {nameof(ParamExpression)}");
                        }

                        paramters.Add((
                            cur.IdentifierToken.Name,
                            Type: cur.TypeToken.Kind switch
                            {
                                SyntaxKind.Int => LiteralType.Int,
                                SyntaxKind.IntPtrRef => LiteralType.IntPtr,
                                _ => throw new Exception($"Unexpected function params type {cur.TypeToken.Kind} in {nameof(VisitFunctionDeclation)} which function is {funcName}, expected {nameof(ParamExpression)}")
                            }
                        ));
                    }

                    break;
                }

            // case ParamExpression paramExpression:
            // {

            // }
            // break;

            default:
                throw new Exception($"Unexpected {nameof(FunctionExpression)} type in {nameof(VisitFunctionDeclation)}, expected ExpressionNode type is {nameof(ParamsExpression)} or {nameof(SyntaxToken)} with void kind");
        }

        functionType.Add(LiteralType.Void);
        foreach (var item in paramters)
        {
            functionType.Add(item.Type);
        }

        // VisitCompoundStatement()
        if (expr.CompoundStatement is not CompoundStatementExpression comp)
        {
            throw new Exception($"Unexpected type in {nameof(VisitFunctionDeclation)}, expected {nameof(CompoundStatementExpression)}");
        }
        VisitCompoundStatement(comp, out var compNode);

        node = new FunctionDeclarationNode(expr.FnName.GetTextPosition(), null, funcName, [.. functionType], [.. paramters], compNode);
        compNode.Parent = node;
    }

    private static void VisitCompoundStatement(in CompoundStatementExpression expr, out CompoundStatementNode node)
    {
        List<DeclarationNode> declList = [];

        if (expr.LocalDeclarations is LocalDeclarationsExpression localDeclExpr)
        {

            if (localDeclExpr.VarDeclaration is VarDeclarationExpression varDeclExpr)
            {
                VisitVarDeclation(varDeclExpr, out var varNode);
                declList.Add(varNode);
            }
            else
            {
                throw new Exception($"Unexpected type in {nameof(VisitCompoundStatement)}, expected {nameof(VarDeclarationExpression)}");
            }

            var localDeclTail = localDeclExpr.LocalDeclarationTail;
            while (localDeclTail is not null)
            {
                if (localDeclTail is not LocalDeclarationsTailExpression tail)
                    throw new Exception($"Unexpected type in {nameof(VisitCompoundStatement)}, expected {nameof(LocalDeclarationsTailExpression)}");

                var localDecarationTail = tail.LocalDeclarationTail;
                localDeclTail = tail.LocalDeclarationTail;

                if (tail.VarDeclaration is VarDeclarationExpression tailVarDeclExpr)
                {
                    VisitVarDeclation(tailVarDeclExpr, out var varNode);
                    declList.Add(varNode);
                }
                else
                {
                    throw new Exception($"Unexpected type in {nameof(VisitCompoundStatement)}, expected {nameof(VarDeclarationExpression)}");
                }
            }

        }
        else if (expr.LocalDeclarations is not null)
        {
            throw new Exception($"Unexpected type in {nameof(VisitCompoundStatement)}, expected {nameof(LocalDeclarationsExpression)} or null");
        }

        List<StatementNode> stmtList = [];

        if (expr.StatementList is StatementListTailExpression stmtListExpr)
        {

            var curStmtListTail = stmtListExpr;
            while (curStmtListTail is not null)
            {
                // if (curStmtListTail.Statement is not )
                if (curStmtListTail.StatementListTail is not null && curStmtListTail.StatementListTail is not StatementListTailExpression tail)
                {
                    throw new Exception($"Unexpected type in {nameof(VisitCompoundStatement)}, expected {nameof(StatementListTailExpression)} or null");
                }

                var stmtExpr = curStmtListTail.Statement;
                curStmtListTail = curStmtListTail.StatementListTail as StatementListTailExpression;

                // curStmtListTail = curStmtListTail.StatementListTail;
                switch (stmtExpr)
                {
                    case ExpressionStatementExpression binaryExpr:
                        {
                            if (binaryExpr.Expression is null)
                            {
                                continue;
                            }
                            VisitBinaryExpression(binaryExpr.Expression, out var stmt);
                            stmtList.Add(stmt);
                            break;
                        }

                    case IterationStatementExpression iterExpr:
                        {
                            VisitBinaryExpression(iterExpr.LogicExpr, out var logicExprNode);
                            StatementNode stmt;
                            if (iterExpr.Block is CompoundStatementExpression compound)
                            {
                                VisitCompoundStatement(compound, out var block);
                                stmt = new WhileStatementNode(null, logicExprNode, block);
                                block.Parent = stmt;
                            }
                            else
                            {
                                VisitBinaryExpression(iterExpr.Block!, out var exprNode);
                                var tmp = new CompoundStatementNode(null, [], [exprNode]);
                                stmt = new WhileStatementNode(null, logicExprNode, tmp);
                                tmp.Parent = stmt;
                                exprNode.Parent = tmp;
                            }

                            stmtList.Add(stmt);
                            break;
                        }

                    case SelectionStatementExpression selectionEpxr:
                        {
                            VisitBinaryExpression(selectionEpxr.LogicExpr, out var logicExprNode);

                            CompoundStatementNode? block1 = null, block2 = null;
                            if (selectionEpxr.FirstStatementsList is CompoundStatementExpression compound1)
                            {
                                VisitCompoundStatement(compound1, out block1);
                            }
                            else if (selectionEpxr.FirstStatementsList is not null)
                            {
                                VisitBinaryExpression(selectionEpxr.FirstStatementsList, out var result);
                                block1 = new CompoundStatementNode(null, [], [result]);
                                result.Parent = block1;
                            }
                            if (block1 is not null && selectionEpxr.SecondStatementsList is CompoundStatementExpression compound2)
                            {
                                VisitCompoundStatement(compound2, out block2);
                            }
                            else if (selectionEpxr.SecondStatementsList is not null)
                            {
                                VisitBinaryExpression(selectionEpxr.SecondStatementsList, out var result);
                                block2 = new CompoundStatementNode(null, [], [result]);
                                result.Parent = block2;
                            }

                            // var stmt = new IfStatementNode(null, stmt);
                            StatementNode stmt;
                            if (block1 == null)
                            {
                                stmt = new IfStatementNode(null, logicExprNode);
                            }
                            else if (block2 == null)
                            {
                                stmt = new IfStatementNode(null, logicExprNode, block1);
                                block1.Parent = stmt;
                            }
                            else
                            {
                                stmt = new IfStatementNode(null, logicExprNode, block1, block2);
                                block2.Parent = stmt;
                            }
                            stmtList.Add(stmt);
                            break;
                        }

                    case ReturnExpression returnExpr:
                        {
                            // var returnExprNode = VisitBinaryExpression(returnExpr.Expression)
                            ExprNode? retExprNode = null;
                            if (returnExpr.Expression is not null)
                            {
                                VisitBinaryExpression(returnExpr.Expression, out retExprNode);
                            }
                            var stmt = new ReturnStatementNode(null, retExprNode);
                            if (retExprNode is not null)
                            {
                                retExprNode.Parent = stmt;
                            }

                            stmtList.Add(stmt);
                            break;
                        }
                }
            }


        }
        else if (expr.StatementList is not null)
        {
            throw new Exception($"Unexpected type in {nameof(VisitCompoundStatement)}, expected {nameof(StatementListTailExpression)} or null");
        }

        // throw new Exception();

        node = new CompoundStatementNode(null, [.. declList], [.. stmtList]);
        foreach (var item in declList)
        {
            item.Parent = node;
        }
        foreach (var item in stmtList)
        {
            item.Parent = node;
        }
    }

    private static void VisitBinaryExpression(in ExpressionNode expr, out ExprNode node)
    {
        switch (expr)
        {
            case AssignmentExpression assignmentExpr:
                {

                    if (assignmentExpr.Variable is not VariableExpression varExpr)
                    {
                        throw new Exception($"Unexpected type in {nameof(VisitBinaryExpression)}, expected {nameof(VariableExpression)}");
                    }
                    VisitVariable(varExpr, out var left);
                    VisitBinaryExpression(assignmentExpr.Expr, out var right);

                    node = new BinaryOperatorNode(varExpr.Identifier.GetTextPosition(), null, "=", left, BinaryOperatorKind.Assignment, right);
                    break;
                }

            case SimpleExpression simExpr:
                VisitSimpleExpression(simExpr, out node);
                break;

            default:
                throw new Exception($"Unexpected position in {nameof(VisitBinaryExpression)}");
        }
    }

    private static void VisitSimpleExpression(in SimpleExpression expr, out ExprNode node)
    {
        if (expr.Op is null)
        {
            VisitAdditiveExpression((AdditiveExpression)expr.AdditionExpr1, out node);
        }
        else
        {
            if (!BinaryOperatorKindHelper.TryParse(expr.Op.Kind, out var opKind))
            {
                throw new Exception($"Unknown operator {expr.Op}");
            }
            if (!BinaryOperatorKindHelper.TryParse(opKind, out var opStr))
            {
                throw new Exception($"Unknown operator {opKind}");
            }

            VisitAdditiveExpression((AdditiveExpression)expr.AdditionExpr1, out var left);
            VisitAdditiveExpression((AdditiveExpression)expr.AdditionExpr2!, out var right);

            node = new BinaryOperatorNode(expr.Op.GetTextPosition(), null, opStr, left, opKind, right);
            left.Parent = node;
            right.Parent = node;
        }
    }

    private static void VisitAdditiveExpression(in AdditiveExpression expr, out ExprNode node)
    {
        VisitTermExpression((TermExpression)expr.Term, out var left);
        // if (expr.AdditiveExpressionTail is null) {
        //     node = left;
        //     return;
        // }

        var curTail = expr.AdditiveExpressionTail as AdditiveTailExpression;
        while (curTail is not null)
        {
            BinaryOperatorKindHelper.TryParse(curTail.Op.Kind, out var opKind);
            BinaryOperatorKindHelper.TryParse(opKind, out var opStr);
            VisitTermExpression((TermExpression)curTail.Term, out var right);

            var newNode = new BinaryOperatorNode(curTail.Op.GetTextPosition(), null, opStr, left, opKind, right);
            left.Parent = right.Parent = newNode;
            left = newNode;

            curTail = curTail.AdditiveExpressionTail as AdditiveTailExpression;
        }

        node = left;
    }

    private static void VisitTermExpression(in TermExpression expr, out ExprNode node)
    {
        VisitFactor((FactorExpression)expr.Factor, out var left);

        var curTail = expr.TermTail as TermTailExpression;
        while (curTail is not null)
        {
            BinaryOperatorKindHelper.TryParse(curTail.Op.Kind, out var opKind);
            BinaryOperatorKindHelper.TryParse(opKind, out var opStr);
            VisitFactor((FactorExpression)curTail.Factor, out var right);

            var newNode = new BinaryOperatorNode(curTail.Op.GetTextPosition(), null, opStr, left, opKind, right);
            left.Parent = right.Parent = newNode;
            left = newNode;

            curTail = curTail.TermTail as TermTailExpression;
        }

        node = left;
    }

    private static void VisitFactor(in FactorExpression expr, out ExprNode node)
    {
        if (expr.Expr is VariableExpression varExpr)
        {
            VisitVariable(varExpr, out node);
        }
        else if (expr.Expr is CallFuncArgsExpression callFuncExpr)
        {
            var funcName = callFuncExpr.Identifier.Name;

            List<ExprNode> argsList = [];
            if (((ArgsExpression)callFuncExpr.Args).ArgsList is ArgsListExpression argsListExpr)
            {
                VisitBinaryExpression(argsListExpr.Expr, out var result);
                argsList.Add(result);

                var curTail = argsListExpr.ArgsListTail as ArgsListTailExpression;
                while (curTail is not null)
                {
                    if (curTail.Expr is null)
                        break;

                    VisitBinaryExpression(curTail.Expr, out var binaryExprNode);
                    argsList.Add(binaryExprNode);

                    curTail = curTail.ArgsListTail as ArgsListTailExpression;
                }
            }

            node = new CallFunctionNode(callFuncExpr.Identifier.GetTextPosition(), null, funcName, [.. argsList]);
            foreach (var item in argsList)
            {
                item.Parent = node;
            }

            // while (curTail is not null) {
            //     VisitBinaryExpression(curTail.Expr, out var binaryExprNode);
            //     argsList.Add(binaryExprNode);

            // }
        }
        else if (expr.Expr is SyntaxTokenWithValue<int> numberToken)
        {
            node = new IntegerLiteral<int>(numberToken.GetTextPosition(), null, numberToken.Name, LiteralType.Int, numberToken.Value);
        }
        else
        {
            throw new Exception($"Unexpected expr in {nameof(VisitFactor)}");
        }
    }

    // private void VisitNumber(in NumberExpression expr, out ASTNode node) {
    //     node = new IntegerLiteral<int>(null, expr.Name)
    // }

    private static void VisitVariable(in VariableExpression expr, out ExprNode node)
    {
        if (expr is { IsArrayRef: true, OffsetExpr: not null })
        {
            VisitBinaryExpression(expr.OffsetExpr, out var exprNode);
            node = new ArraySubscriptExprNode(expr.Identifier.GetTextPosition(), null, expr.Identifier.Name, exprNode);
        }
        else
        {
            node = new LiteralNode(expr.Identifier.GetTextPosition(), null, expr.Identifier.Name);
        }
    }
}
