using Flux.Language.Lexer;
namespace Flux.Language.Diagnostics;

public class Return : System.Exception{
  private readonly Object? value;
  public Object? getValue(){
    return value;
  }
  public Return(Object? value):base(null,null){
    this.value = value;
  }
}

