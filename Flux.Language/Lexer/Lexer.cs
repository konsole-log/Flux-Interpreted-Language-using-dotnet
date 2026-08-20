using System.Collections.Generic;
namespace Flux.Language.Lexer;

public class Lexer
{
    private readonly string? source;//input file
    private readonly List<Token> tokens = new List<Token>();//output of the list of the token
    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Lexer(string source)
    {
        this.source = source;
    }
    public List<Token> ScanTokens(){
        while(!IsAtEnd()){
            start = current;
            ScanToken();
        }
        tokens.Add(new Token(TokenType.EOF,"",null,line));
        return tokens;
    }
    private bool IsAtEnd(){
        return current >= source.Length;
    }
    private void ScanToken(){
        char c = Advance();
        switch(c){
            //for single character
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE);break;
            case '}': AddToken(TokenType.RIGHT_BRACE);break;
            case ',': AddToken(TokenType.COMMA);break;
            case '.': AddToken(TokenType.DOT);break;
            case '-': AddToken(TokenType.MINUS);break;
            case '+': AddToken(TokenType.PLUS);break;
            case ';': AddToken(TokenType.SEMICOLON);break;
            case '*': AddToken(TokenType.STAR);break;
            default: ErrorReporter.Error(line,"Unexpected Character.");break;
        }
    }
    private char Advance(){
        return source[current++];
    }
    private void AddToken(TokenType type){
        AddToken(type,null);
    }
    private void AddToken(TokenType type, Object? literal){
        string text = source[start..current];
        tokens.Add(new Token(type,text,literal,line));
    }
}
