using Flux.Language.Lexer;
using Flux.Language.Diagnostics;
namespace Flux.Language.AST;

public abstract class Expr {

	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		public T VisitBinaryExpr(Binary expr);
		public T VisitAssignExpr(Assign expr);
		public T VisitGroupingExpr(Grouping expr);
		public T VisitLiteralExpr(Literal expr);
		public T VisitLogicalExpr(Logical expr);
		public T VisitUnaryExpr(Unary expr);
		public T VisitVariableExpr(Variable expr);
	}

	public class Binary : Expr {

		readonly Expr left;
		readonly Token opr;
		readonly Expr right;

		public Expr getLeft() {
			return left;
		}

		public Token getOpr() {
			return opr;
		}

		public Expr getRight() {
			return right;
		}

		public Binary(Expr left, Token opr, Expr right) {
			this.left = left;
			this.opr = opr;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitBinaryExpr(this);
		}
	}

	public class Assign : Expr {

		readonly Token name;
		readonly Expr value;

		public Token getName() {
			return name;
		}

		public Expr getValue() {
			return value;
		}

		public Assign(Token name, Expr value) {
			this.name = name;
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitAssignExpr(this);
		}
	}

	public class Grouping : Expr {

		readonly Expr expression;

		public Expr getExpression() {
			return expression;
		}

		public Grouping(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitGroupingExpr(this);
		}
	}

	public class Literal : Expr {

		readonly Object? value;

		public Object? getValue() {
			return value;
		}

		public Literal(Object? value) {
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitLiteralExpr(this);
		}
	}

	public class Logical : Expr {

		readonly Expr left;
		readonly Token opr;
		readonly Expr right;

		public Expr getLeft() {
			return left;
		}

		public Token getOpr() {
			return opr;
		}

		public Expr getRight() {
			return right;
		}

		public Logical(Expr left, Token opr, Expr right) {
			this.left = left;
			this.opr = opr;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitLogicalExpr(this);
		}
	}

	public class Unary : Expr {

		readonly Token opr;
		readonly Expr right;

		public Token getOpr() {
			return opr;
		}

		public Expr getRight() {
			return right;
		}

		public Unary(Token opr, Expr right) {
			this.opr = opr;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitUnaryExpr(this);
		}
	}

	public class Variable : Expr {

		readonly Token name;

		public Token getName() {
			return name;
		}

		public Variable(Token name) {
			this.name = name;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitVariableExpr(this);
		}
	}

}