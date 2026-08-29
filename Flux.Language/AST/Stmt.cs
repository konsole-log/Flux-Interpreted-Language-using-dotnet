using Flux.Language.Diagnostics;
using Flux.Language.Lexer;

namespace Flux.Language.AST;

public abstract class Stmt
{
    public abstract T Accept<T>(Visitor<T> visitor);

    public interface Visitor<T>
    {
        public T VisitBlockStmt(Block stmt);
        public T VisitExpressionStmt(Expression stmt);
        public T VisitIfStmt(If stmt);
        public T VisitPrintStmt(Print stmt);
        public T VisitLetStmt(Let stmt);
    }

    public class Block : Stmt
    {
        readonly List<Stmt> statements;

        public List<Stmt> getStatements()
        {
            return statements;
        }

        public Block(List<Stmt> statements)
        {
            this.statements = statements;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
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

    public class If : Stmt
    {
        readonly Expr condition;
        readonly Stmt thenBranch;
        readonly Stmt? elseBranch;

        public Expr getCondition()
        {
            return condition;
        }

        public Stmt getThenbranch()
        {
            return thenBranch;
        }

        public Stmt? getElsebranch()
        {
            return elseBranch;
        }

        public If(Expr condition, Stmt thenBranch, Stmt? elseBranch)
        {
            this.condition = condition;
            this.thenBranch = thenBranch;
            this.elseBranch = elseBranch;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
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

    public class Let : Stmt
    {
        readonly Token name;
        readonly Expr? initializer;

        public Token getName()
        {
            return name;
        }

        public Expr? getInitializer()
        {
            return initializer;
        }

        public Let(Token name, Expr? initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitLetStmt(this);
        }
    }
}
