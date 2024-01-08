using System.Collections.Generic;
using Cirno.CodeGen;
using LLVMSharp.Interop;

namespace Cirno.AbstractSyntaxTree;

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

    public override LLVMValueRef? Accept(ICodeGenVisitor visitor)
        => visitor.Visit(this);
}