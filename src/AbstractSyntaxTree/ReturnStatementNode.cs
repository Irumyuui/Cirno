using System.Collections.Generic;
using Cirno.CodeGen;
using LLVMSharp.Interop;

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
    
    public override LLVMValueRef? Accept(ICodeGenVisitor visitor, LLVMBasicBlockRef? entryBasicBlock,
        LLVMBasicBlockRef? exitBasicBlock)
        => visitor.Visit(this, entryBasicBlock, entryBasicBlock);
}