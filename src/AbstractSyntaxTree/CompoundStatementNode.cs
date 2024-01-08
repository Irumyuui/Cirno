using System.Collections.Generic;
using System.Collections.Immutable;
using Cirno.CodeGen;
using LLVMSharp.Interop;

namespace Cirno.AbstractSyntaxTree;

public sealed class CompoundStatementNode : ASTNode
{
    public CompoundStatementNode(ASTNode? parent, ImmutableArray<DeclarationNode> declarationStatement, ImmutableArray<StatementNode> expressionStatement)
    {
        Parent = parent;
        DeclarationStatement = declarationStatement;
        ExpressionStatement = expressionStatement;
    }

    public ImmutableArray<DeclarationNode> DeclarationStatement { get; }

    public ImmutableArray<StatementNode> ExpressionStatement { get; }

    public override string Name => "CompoundStatement";

    public override ASTNodeType NodeType => ASTNodeType.CompoundStatement;

    public override ASTNode? Parent { get; protected internal set; }

    public override IEnumerable<ASTNode> GetChildren()
    {
        foreach (var nex in DeclarationStatement)
            yield return nex;
        foreach (var nex in ExpressionStatement)
            yield return nex;
    }
    
    public override LLVMValueRef? Accept(ICodeGenVisitor visitor)
        => visitor.Visit(this);
}