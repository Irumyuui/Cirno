namespace Cirno.AbstractSyntaxTree;

public sealed class AST
{
    public AST(Cirno.Expressions.ExpressionTree tree)
    {
        ASTBuilder.VisitProgram(tree.Root, out var root);
        Root = root;
    }

    public ASTNode Root { get; private set; }

    public void Dump() => ASTNode.PrettyPrint(Root);
}