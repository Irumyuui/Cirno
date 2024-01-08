using Cirno.CodeGen;
using LLVMSharp.Interop;

namespace Cirno.AbstractSyntaxTree;

public abstract class ExprNode : StatementNode
{
    public override LLVMValueRef? Accept(ICodeGenVisitor visitor)
        => visitor.Visit(this);
}