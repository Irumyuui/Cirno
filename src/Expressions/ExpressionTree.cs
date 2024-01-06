namespace Cirno.Expressions;

public sealed class ExpressionTree {
    public ExpressionTree(ExpressionNode root) => Root = root;

    public ExpressionTree(string[] text) {
        var parser = new Parser.Parser(text);
        var tree = parser.Parse();
        Root = tree.Root;
    }

    public ExpressionNode Root { get; private set; }

    public void Dump() => ExpressionNode.PrettyPrint(Root);
}
