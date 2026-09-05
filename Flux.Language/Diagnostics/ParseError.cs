using Flux.Language.Diagnostics;
using Flux.Language.Lexer;
namespace Flux.Language.Diagnostics;

public class ParseError : System.Exception
{
  internal ParseError(Token token, string message):base(message){

  }
}
