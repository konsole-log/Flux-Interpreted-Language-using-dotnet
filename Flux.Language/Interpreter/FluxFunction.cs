using Flux.Language.AST;
using Flux.Language.Diagnostics;
using Flux.Language.Lexer;

namespace Flux.Language.Interpreter;

public class FluxFunction : FluxCallable
{
    private readonly Stmt.Function declaration;

    public FluxFunction(Stmt.Function declaration)
    {
        this.declaration = declaration;
    }

    public Object? Call(Interpreter interpreter, List<Object> arguments)
    {
        Env environment = new Env(interpreter.globals);
        for (int i = 0; i < declaration.getParameters().Count; i++)
        {
            environment.Define(declaration.getParameters()[i].getLexeme(), arguments[i]);
        }
        try{
            interpreter.ExecuteBlock(declaration.getBody(),environment);
        }catch(Return returnValue){
            return returnValue.getValue();
        }
        return null;
    }

    public int Arity()
    {
        return declaration.getParameters().Count;
    }

    public override string ToString()
    {
        return $"<fn {declaration.getName().getLexeme()} >";
    }
}
