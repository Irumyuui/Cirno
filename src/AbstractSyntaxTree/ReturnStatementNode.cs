using System.Collections.Generic;

namespace Cirno.AbstractSyntaxTree;

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