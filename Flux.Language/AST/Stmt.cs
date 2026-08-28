using Flux.Language.Lexer;
using Flux.Language.Diagnostics;
namespace Flux.Language.AST;

public abstract class Stmt {

	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		public T VisitExpressionStmt(Expression stmt);
		public T VisitPrintStmt(Print stmt);
		public T VisitLetStmt(Let stmt);
	}

	public class Expression : Stmt {

		readonly Expr expression;

		public Expr getExpression() {
			return expression;
		}

		public Expression(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitExpressionStmt(this);
		}
	}

	public class Print : Stmt {

		readonly Expr expression;

		public Expr getExpression() {
			return expression;
		}

		public Print(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitPrintStmt(this);
		}
	}

	public class Let : Stmt {

		readonly Token name;
		readonly Expr initializer;

		public Token getName() {
			return name;
		}

		public Expr getInitializer() {
			return initializer;
		}

		public Let(Token name, Expr initializer) {
			this.name = name;
			this.initializer = initializer;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitLetStmt(this);
		}
	}

}