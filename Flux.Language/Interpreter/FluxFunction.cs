using Flux.Language.Lexer;
using Flux.Language.Diagnostics;
using Flux.Language.AST;
namespace Flux.Language.Interpreter;

public class FluxFunction : FluxCallable{
  private readonly Stmt.Function declaration;
  public FluxFunction(Stmt.Function declaration){
    this.declaration = declaration;
  }

  public Object? Call(Interpreter interpreter, List<Object?> arguments){
    Env environment = new Env(interpreter.globals);
    for(int i = 0; i < declaration.getParams().Count;i++){
      environment.Define(declaration.getParams()[i].getLexeme(),arguments[i]);
    }
    interpreter.ExecuteBlock(declaration.getBody(),environment);
    return null;
  }

  public int Arity(){
    return declaration.getParams().Count;
  }

    public override string ToString() {
        return $"<fn {declaration.getName().getLexeme()} >";
    }
}
