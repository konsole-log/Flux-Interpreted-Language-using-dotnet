using System.Collections.Generic;

namespace Flux.Language.Lexer;

public class Lexer
{
    private readonly string? source; //input file
    private readonly List<Token> tokens = new List<Token>(); //output of the list of the token
    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Lexer(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }
        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            //for single character
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    //A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                line++;
                break;
            case '"':
                IsString();
                break;
            default:
                ErrorReporter.Error(line, "Unexpected Character.");
                break;
        }
    }
    private void IsString(){
        while(Peek()!='"'&&!IsAtEnd()){
            if(Peek()=='\n'){
                line++;
            }
            Advance();
        }
        if(IsAtEnd()){
            ErrorReporter.Error(line,"Unterminated String.");
            return;
        }
        //the closing ".
        Advance();
        //Trim the surrounding quotes.
        string value = source[(start+1)..(current-1)];
        AddToken(TokenType.STRING,value);

    }
    private char Peek()
    {
        if (IsAtEnd())
        {
            return '\0';
        }
        return source[current];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }
        if (source[current] != expected)
        {
            return false;
        }
        current++;
        return true;
    }

    private char Advance()
    {
        return source[current++];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, Object? literal)
    {
        string text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }
}
