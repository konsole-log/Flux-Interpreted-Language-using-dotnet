using Flux.Language.Lexer;
using Flux.Language.Diagnostics;

namespace Flux.Language.Interpreter;

public class Env
{
  //variables are stored as key value pair of the string name and object as value;
    private readonly Dictionary<string, Object?> values = new Dictionary<string, Object?>();
    //scope which is null for the global but has something for the local scope
    internal readonly Env? enclosing;
    
    //this constructor is for the global scope variable
    public Env(){
      this.enclosing = null;
    }

    //this constructor is for the local scoped variable
    public Env(Env enclosing){
      this.enclosing = enclosing;
    }

    public Env? getEnclosing(){
      return this.enclosing;
    }

    //retrieve variable value from the name
    public Object? get(Token name){
      if(values.ContainsKey(name.getLexeme())){
        return values[name.getLexeme()];
      }
      //if variable is not found look up
      if(enclosing !=null){
        return enclosing.get(name);
      }
      throw new RunTimeError(name,$"Undefined Variable {name.getLexeme()}.");
    }

    //define a variable
    public void Define(string name, Object? value){
      values[name]=value;
    }

    //Assign a value to the variable
    public void Assign(Token name, Object value)
    {
        if (values.ContainsKey(name.getLexeme()))
        {
            values[name.getLexeme()] = value;
            return;
        }
        // If variable not declared in scope, look up
        if (enclosing != null)
        {
            enclosing.Assign(name, value);
            return;
        }
        throw new RunTimeError(name, $"Undefined variable '{name.getLexeme()}'.");
    }
}
