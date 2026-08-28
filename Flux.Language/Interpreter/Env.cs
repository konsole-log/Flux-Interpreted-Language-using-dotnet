using Flux.Language.Lexer;
using Flux.Language.Diagnostics;

namespace Flux.Language.Interpreter;

public class Env
{
    private readonly Dictionary<string, Object?> values = new Dictionary<string, Object?>();

    public Object? get(Token name){
      if(values.ContainsKey(name.getLexeme())){
        return values[name.getLexeme()];
      }
      throw new RunTimeError(name,$"Undefined Variable {name.getLexeme()}.");
    }
    public void Define(string name, Object? value){
      values[name]=value;
    }
    public void Assign(Token name,Object value){
      if(values.ContainsKey(name.getLexeme())){
        values[name.getLexeme()]=value;
        return;
      }
      throw new RunTimeError(name,$"Undefined variable '{name.getLexeme()}'.");
    }
}
