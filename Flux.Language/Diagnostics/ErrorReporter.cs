namespace Flux.Language.Diagnostics;

using Flux.Language.Lexer;

public class ErrorReporter
{
    public static bool hadError { get; set; }
    public static bool hadRunTimeError { get; set; }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string position, string message)
    {
        Console.Error.WriteLine($"[Line {line} Error {position} :{message}]");
        hadError = true;
    }

    public static void Error(Token token, String message)
    {
        if (token.getType() == TokenType.EOF)
        {
            Report(token.getLine(), "at end", message);
        }
        else
        {
            Report(token.getLine(), $"at '{token.getLexeme()}' ", message);
        }
    }

    public static void RunTimeError(RunTimeError error)
    {
        Console.Error.WriteLine($"{error.Message}\n[Line {error.token.getLine()} ]");
        hadRunTimeError = true;
    }
}
