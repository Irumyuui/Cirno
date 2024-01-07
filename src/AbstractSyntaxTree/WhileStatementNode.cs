using System.Collections.Generic;
using Cirno.CodeGen;
using LLVMSharp.Interop;

namespace Cirno.AbstractSyntaxTree;

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

    public override LLVMValueRef? Accept(ICodeGenVisitor visitor, LLVMBasicBlockRef? entryBasicBlock,
        LLVMBasicBlockRef? exitBasicBlock)
        => visitor.Visit(this, entryBasicBlock, entryBasicBlock);
}