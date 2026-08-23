namespace Flux.Language.AST;

public abstract class Stmt
{
    public abstract T Accept<T>(Visitor<T> visitor);

    public interface Visitor<T>
    {
        public T VisitExpressionStmt(Expression stmt);
        public T VisitPrintStmt(Print stmt);
    }

    public class Expression : Stmt
    {
        readonly Expr expression;

        public Expr getExpression()
        {
            return expression;
        }

        public Expression(Expr expression)
        {
            this.expression = expression;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public class Print : Stmt
    {
        readonly Expr expression;

        public Expr getExpression()
        {
            return expression;
        }

        public Print(Expr expression)
        {
            this.expression = expression;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }
}
