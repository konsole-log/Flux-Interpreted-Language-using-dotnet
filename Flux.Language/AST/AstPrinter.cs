using System.Text;

namespace Flux.Language.AST;

public class AstPrinter : Expr.Visitor<string>, Stmt.Visitor<string>
{

	public string Print(Expr expr)
	{
		return expr.Accept(this);
	}

	public string Print(Stmt stmt)
	{
		return stmt.Accept(this);
	}

	public string VisitBinaryExpr(Expr.Binary expr)
	{
		return "";
	}

	public string VisitCallExpr(Expr.Call expr)
	{
		return "";
	}

	public string VisitAssignExpr(Expr.Assign expr)
	{
		return "";
	}

	public string VisitGroupingExpr(Expr.Grouping expr)
	{
		return "";
	}

	public string VisitLiteralExpr(Expr.Literal expr)
	{
		return "";
	}

	public string VisitLogicalExpr(Expr.Logical expr)
	{
		return "";
	}

	public string VisitUnaryExpr(Expr.Unary expr)
	{
		return "";
	}

	public string VisitVariableExpr(Expr.Variable expr)
	{
		return "";
	}

	public string VisitBlockStmt(Stmt.Block stmt)
	{
		return "";
	}

	public string VisitExpressionStmt(Stmt.Expression stmt)
	{
		return "";
	}

	public string VisitFunctionStmt(Stmt.Function stmt)
	{
		return "";
	}

	public string VisitIfStmt(Stmt.If stmt)
	{
		return "";
	}

	public string VisitPrintStmt(Stmt.Print stmt)
	{
		return "";
	}

	public string VisitReturnStmt(Stmt.Return stmt)
	{
		return "";
	}

	public string VisitLetStmt(Stmt.Let stmt)
	{
		return "";
	}

	public string VisitWhileStmt(Stmt.While stmt)
	{
		return "";
	}

}
