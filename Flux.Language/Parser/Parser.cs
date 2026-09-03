using System.Diagnostics;
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
        while (!IsAtEnd())
        {
            Stmt? statement = Declaration();
            if (statement != null)
            {
                statements.Add(statement);
            }
        }
        return statements;
    }

    private Stmt? Declaration()
    {
        try
        {
            if (Match(TokenType.FUN))
            {
                return Function("function");
            }
            if (Match(TokenType.LET))
                return VarDeclaration();

            return Statement();
        }
        catch (ParseError error)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt Statement()
    {
        if (Match(TokenType.FOR))
        {
            return ForStatement();
        }
        if (Match(TokenType.IF))
        {
            return IfStatement();
        }
        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }
        if (Match(TokenType.RETURN))
        {
            return ReturnStatement();
        }
        if (Match(TokenType.WHILE))
        {
            return WhileStatement();
        }
        if (Match(TokenType.LEFT_BRACE))
        {
            return new Stmt.Block(Block());
        }
        return ExpressionStatement();
    }

    private Stmt ForStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");
        Stmt? initializer;
        if (Match(TokenType.SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(TokenType.LET))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        Expr? condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after loop condition");

        Expr? increment = null;
        if (!Check(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

        Stmt body = Statement();

        if (increment != null)
        {
            body = new Stmt.Block([body, new Stmt.Expression(increment)]);
        }
        if (condition == null)
        {
            condition = new Expr.Literal(true);
        }
        body = new Stmt.While(condition, body);

        if (initializer != null)
        {
            body = new Stmt.Block([initializer, body]);
        }
        return body;
    }

    private Stmt.If IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, $"Expect '(' after 'if'. ");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, $"Expect ')' after if condition");
        Stmt thenBranch = Statement();
        Stmt? elseBranch = null;
        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }
        return new Stmt.If(condition, thenBranch, elseBranch);
    }

    private Stmt.Print PrintStatement()
    {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, $"Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt.Return ReturnStatement()
    {
        Token keyword = Previous();
        Expr? value = null;
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
        return new Stmt.Return(keyword, value);
    }

    private Stmt.Let VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr? initializer = null;
        if (Match(TokenType.EQUAL))
        {
            initializer = Expression();
        }
        Consume(TokenType.SEMICOLON, $"Expect ';' after variable declaration.");
        return new Stmt.Let(name, initializer);
    }

    private Stmt.While WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '{{' after 'while'.");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect '}}' after condition.");
        Stmt body = Statement();

        return new Stmt.While(condition, body);
    }

    private Stmt.Expression ExpressionStatement()
    {
        Expr expr = Expression();
        Consume(TokenType.SEMICOLON, $"Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }

    private Stmt.Function Function(string kind)
    {
        Token name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name");
        Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");
        List<Token> parameters = new List<Token>();
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters");
                }
                parameters.Add(Consume(TokenType.IDENTIFIER, "Expect paramter name."));
            } while (Match(TokenType.COMMA));
        }
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters");
        Consume(TokenType.LEFT_BRACE, $"Expect {{ before {kind} body");
        List<Stmt> body = Block();
        return new Stmt.Function(name, parameters, body);
    }

    private List<Stmt> Block()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            Stmt? statement = Declaration();
            if (statement != null)
            {
                statements.Add(statement);
            }
        }
        Consume(TokenType.RIGHT_BRACE, $"Expect '}}' after block.");
        return statements;
    }

    private Expr Assignment()
    {
        Expr expr = OrExp();

        if (Match(TokenType.EQUAL))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Expr.Variable)
            {
                Token name = ((Expr.Variable)expr).getName();
                return new Expr.Assign(name, value);
            }
            Error(equals, "Invalid Assignment target.");
        }
        return expr;
    }

    private Expr OrExp()
    {
        Expr expr = AndExp();
        while (Match(TokenType.OR))
        {
            Token opr = Previous();
            Expr right = AndExp();
            expr = new Expr.Logical(expr, opr, right);
        }
        return expr;
    }

    private Expr AndExp()
    {
        Expr expr = Equality();
        while (Match(TokenType.AND))
        {
            Token opr = Previous();
            Expr right = Equality();
            expr = new Expr.Logical(expr, opr, right);
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
        return Call();
    }

    private Expr Call()
    {
        Expr expr = Primary();
        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }
            else
            {
                break;
            }
        }
        return expr;
    }

    private Expr.Call FinishCall(Expr callee)
    {
        List<Expr> arguments = new List<Expr>();
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "No more arguments than 255");
                }
                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }
        Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");
        return new Expr.Call(callee, paren, arguments);
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE))
            return new Expr.Literal(false);
        if (Match(TokenType.TRUE))
            return new Expr.Literal(true);
        if (Match(TokenType.NIL))
            return new Expr.Literal(null);
        if (Match(TokenType.IDENTIFIER))
        {
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
        return new ParseError(token, message);
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
