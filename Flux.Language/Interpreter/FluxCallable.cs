namespace Flux.Language.Interpreter;

public interface FluxCallable{
  public Object? Call(Interpreter interpreter, List<Object> arguments);
  public int Arity();
}
