using System.Collections.Generic;
using Cirno.CodeGen;
using LLVMSharp.Interop;

namespace Cirno.AbstractSyntaxTree;

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
    
    public override LLVMValueRef? Accept(ICodeGenVisitor visitor, LLVMBasicBlockRef? entryBasicBlock,
        LLVMBasicBlockRef? exitBasicBlock)
        => visitor.Visit(this, entryBasicBlock, entryBasicBlock);
}