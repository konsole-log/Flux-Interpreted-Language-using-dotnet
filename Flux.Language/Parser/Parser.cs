using Flux.Language.AST;
using Flux.Language.Diagnostics;
using Flux.Language.Lexer;

namespace Flux.Language.Parser;

public class Parser
{
    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new List<Stmt>();
        while(!IsAtEnd()){
            statements.Add(Declaration());
        }
        return statements;
    }

    private Stmt Declaration(){
        try{
            if(Match(TokenType.LET)) return VarDeclaration();

            return Statement();
        }catch(ParseError error){
            Synchronize();
            return null;
        }
    }
    private Stmt Statement(){
        if(Match(TokenType.PRINT)){
            return PrintStatement();
        }
        return ExpressionStatement();
    }

    private Stmt.Print PrintStatement(){
        Expr value = Expression();
        Consume(TokenType.SEMICOLON,$"Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt VarDeclaration(){
        Token name = Consume(TokenType.IDENTIFIER,"Expect variable name.");

        Expr initializer = null;
        if(Match(TokenType.EQUAL)){
            initializer = Expression();
        }
        Consume(TokenType.SEMICOLON,$"Expect ';' after variable declaration.");
        return new Stmt.Let(name,initializer);
    }

    private Stmt.Expression ExpressionStatement(){
        Expr expr = Expression();
        Consume(TokenType.SEMICOLON,$"Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }

    private Expr Assignment(){
        Expr expr = Equality();

        if(Match(TokenType.EQUAL)){
            Token equals = Previous();
            Expr value = Assignment();

            if(expr is Expr.Variable){
                Token name = ((Expr.Variable)expr).getName();
                return new Expr.Assign(name,value);
            }
            ErrorReporter.Error(equals,"Invalid Assignment target.");
        }
        return expr;
    }
    private Expr Expression()
    {
        return Assignment();
    }

    private Expr Equality()
    {
        Expr expr = OprComparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token opr = Previous();
            Expr right = OprComparison();
            expr = new Expr.Binary(expr, opr, right);
        }
        return expr;
    }

    private Expr OprComparison()
    {
        Expr expr = Term();
        while (
            Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)
        )
        {
            Token opr = Previous();
            Expr right = Term();
            expr = new Expr.Binary(expr, opr, right);
        }
        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token opr = Previous();
            Expr right = Factor();
            expr = new Expr.Binary(expr, opr, right);
        }
        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();
        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token opr = Previous();
            Expr right = Unary();
            expr = new Expr.Binary(expr, opr, right);
        }
        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token opr = Previous();
            Expr right = Unary();
            return new Expr.Unary(opr, right);
        }
        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE))
            return new Expr.Literal(false);
        if (Match(TokenType.TRUE))
            return new Expr.Literal(true);
        if (Match(TokenType.NIL))
            return new Expr.Literal(null);
        if (Match(TokenType.IDENTIFIER)){
            return new Expr.Variable(Previous());
        }
        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(Previous().getLiteral());
        }
        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }
        throw Error(Peek(), "Expect Expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        throw Error(Peek(), message);
    }

    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd())
            return false;
        return Peek().getType() == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
            current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().getType() == TokenType.EOF;
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current - 1];
    }

    private ParseError Error(Token token, string message)
    {
        ErrorReporter.Error(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();
        while (!IsAtEnd())
        {
            if (Previous().getType() == TokenType.SEMICOLON)
            {
                return;
            }
            switch (Peek().getType())
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.LET:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }
            Advance();
        }
    }
}
