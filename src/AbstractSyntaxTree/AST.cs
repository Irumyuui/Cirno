namespace Cirno.AbstractSyntaxTree;

public sealed class AST
{
    public AST(Cirno.Expressions.ExpressionTree tree)
    {
        ASTBuildVisitor.VisitProgram(tree.Root, out var root);
        Root = root;
    }

    public ASTNode Root { get; private set; }
}