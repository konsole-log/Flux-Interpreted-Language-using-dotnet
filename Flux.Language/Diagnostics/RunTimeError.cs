using Flux.Language.Lexer;
namespace Flux.Language.Diagnostics;

public class RunTimeError : System.Exception{
  public readonly Token? token;

  public RunTimeError(Token? token,string message):base(message){
    this.token = token;
  }
}
