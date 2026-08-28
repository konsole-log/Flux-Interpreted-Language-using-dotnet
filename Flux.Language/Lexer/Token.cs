namespace Flux.Language.Lexer;

public class Token
{
    readonly TokenType type;
    readonly string lexeme;
    readonly Object? literal;
    readonly int line;

    public Token(TokenType type, string lexeme, Object? literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }
    public TokenType getType(){return this.type;}
    public Object? getLiteral(){return this.literal;}
    public string getLexeme(){return this.lexeme;}
    public int getLine(){return this.line;}
    public override string ToString()
    {
        return $"Type:{this.type,-15} Lexeme: {this.lexeme,-10} Literal: {this.literal,-10}";
    }
}
