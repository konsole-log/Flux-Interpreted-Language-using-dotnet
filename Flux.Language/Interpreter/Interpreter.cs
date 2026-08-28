using Flux.Language.AST;
using Flux.Language.Diagnostics;
using Flux.Language.Lexer;
using Flux.Language.Parser;

namespace Flux.Language.Interpreter;

public class Interpreter : Expr.Visitor<Object?>,Stmt.Visitor<Object?>
{
    private Env environment = new Env();
    public void Interpret(List<Stmt> statements){
      try{
        foreach(Stmt statement in statements){
          Execute(statement);
        }
      }catch(RunTimeError error){
        ErrorReporter.RunTimeError(error);
      }
    }

    private void Execute(Stmt stmt){
      stmt.Accept(this);
    }

    public Object? VisitLiteralExpr(Expr.Literal expr) { 
      return expr.getValue();
    }

    public Object? VisitBinaryExpr(Expr.Binary expr){
      Object? left = Evaluate(expr.getLeft());
      Object? right = Evaluate(expr.getRight());

      switch(expr.getOpr().getType()){
        case TokenType.BANG_EQUAL:
          return !IsEqual(left,right);
        case TokenType.EQUAL_EQUAL:
          return IsEqual(left,right);
        case TokenType.GREATER:
          CheckNumberOperands(expr.getOpr(),left,right);
          return (double)left > (double)right;
        case TokenType.GREATER_EQUAL:
          CheckNumberOperands(expr.getOpr(),left,right);
          return (double)left >= (double)right;
        case TokenType.LESS:
          CheckNumberOperands(expr.getOpr(),left,right);
          return (double)left < (double)right;
        case TokenType.LESS_EQUAL:
          CheckNumberOperands(expr.getOpr(),left,right);
          return (double)left <= (double)right;
        case TokenType.MINUS:
          CheckNumberOperands(expr.getOpr(),left,right);
          return (double)left - (double)right;
        case TokenType.PLUS:
          if (left is double && right is double){
            return (double)left + (double)right;
          }
          if(left is string || right is string){
            return Stringify(left)+" "+Stringify(right);
          }
          throw new RunTimeError(expr.getOpr(),"Operands must be two numbers or two strings");
        case TokenType.SLASH:
          CheckNumberOperands(expr.getOpr(),left,right);
          return (double)left / (double)right;
        case TokenType.STAR:
          CheckNumberOperands(expr.getOpr(),left,right);
          return (double)left * (double)right;
      }

      return null;
    }

    public Object? VisitGroupingExpr(Expr.Grouping expr){
      return Evaluate(expr.getExpression());
    } 

    public Object? VisitUnaryExpr(Expr.Unary expr){
      Object? right = Evaluate(expr.getRight());
      switch(expr.getOpr().getType()){
        case TokenType.BANG:
          return !IsTruthy(right);
        case TokenType.MINUS:
          CheckNumberOperand(expr.getOpr(),right);
          return -(double)right;
      }
      return null;
    }

    public Object? VisitExpressionStmt(Stmt.Expression stmt){
      Evaluate(stmt.getExpression());
      return null;
    }

    public Object? VisitPrintStmt(Stmt.Print stmt){
      Object? value = Evaluate(stmt.getExpression());
      Console.WriteLine(Stringify(value));
      return null;
    }
    
    public Object? VisitLetStmt(Stmt.Let stmt){
      Object? value = null;
      if(stmt.getInitializer()!=null){
        value = Evaluate(stmt.getInitializer());
      }
      environment.Define(stmt.getName().getLexeme(),value);
      return null;
    }

    public Object? VisitVariableExpr(Expr.Variable expr){
      return environment.get(expr.getName());
    }

    public Object? VisitAssignExpr(Expr.Assign expr){
      Object? value = Evaluate(expr.getValue());
      environment.Assign(expr.getName(),value);
      return value;
    }
    public string Stringify(Object? obj){
      if(obj == null) return "nil";
      if(obj is double){
        string text = obj.ToString();
        if(text.EndsWith(".0")){
          text = text[0..(text.Length-2)];
        }
        return text;
      }
      return obj.ToString();
    }

    private void CheckNumberOperand(Token opr,Object? operand){
      if(operand is double) return;
      throw new RunTimeError(opr,"Operand must be a number");
    }

    private void CheckNumberOperands(Token opr, Object? left, Object? right){
      if(left is double && right is double){
        return;
      }
      throw new RunTimeError(opr,"Operands must be a number");
    }
    private bool IsTruthy(Object? obj){
      if(obj == null) return false;
      if(obj is bool) return (bool)obj;
      return true;
    }

    private bool IsEqual(Object? a, Object? b){
      if(a == null && b==null) return true;
      if(a==null)return false;
      return a.Equals(b);
    }
    private Object? Evaluate(Expr expr){
      return expr.Accept(this);
    }
}
