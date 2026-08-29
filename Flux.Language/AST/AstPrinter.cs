using System.Text;
using Flux.Language.Lexer;

namespace Flux.Language.AST;

public class AstPrinter : Expr.Visitor<string> {
    public string Print(Expr expr) {
        return expr.Accept(this);
    }

    public string VisitLogicalExpr(Expr.Logical expr){
        return "";
    }
    public string VisitBinaryExpr(Expr.Binary expr) {
        return Parenthesize(expr.getOpr().getLexeme(), expr.getLeft(), expr.getRight());
    }
    public string VisitAssignExpr(Expr.Assign expr){
        return Parenthesize($"={expr.getName().getLexeme()}",expr.getValue());
    }

    public string VisitVariableExpr(Expr.Variable expr){
        return expr.getName().getLexeme();
    }
     
    public string VisitGroupingExpr(Expr.Grouping expr) {
        return Parenthesize("group", expr.getExpression());
    }

    public string VisitLiteralExpr(Expr.Literal expr) {
        if (expr.getValue() == null)
            return "nil";
        return expr.getValue().ToString();
    }

    public string VisitUnaryExpr(Expr.Unary expr) {
        return Parenthesize(expr.getOpr().getLexeme(), expr.getRight());
    }

    private string Parenthesize(string name, params Expr[] exprs) {
        StringBuilder builder = new StringBuilder();

        builder.Append("(").Append(name);
        foreach (Expr expr in exprs) {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }
}
