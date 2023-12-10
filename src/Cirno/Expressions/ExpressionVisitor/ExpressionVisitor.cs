namespace Cirno.Expressions;

// public abstract class ExpressionVisitor {
//     protected ExpressionVisitor() {

//     }

//     public virtual ExpressionNode Visit(ExpressionNode node) {
//         return node.Accept(this);
//     }

//     protected internal virtual ExpressionNode VisitExtension(ExpressionNode node) {
//         return node.VisitChildren(this);
//     }

//     protected internal virtual ExpressionNode Visit(NumberExpression node) {
//         return node;
//     }

//     protected internal virtual ExpressionNode Visit(BinaryExpression node) {
//         Visit(node.Left);
//         Visit(node.Right);
//         return node;
//     }
// }
