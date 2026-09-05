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
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        if (line == 1)
        {
            Console.Error.WriteLine($"[Line 1 Error {position} : {message}]");
        }
        else
        {
            Console.Error.WriteLine($"[Line {line-1} Error {position} : {message}]");
        }
        Console.ResetColor();
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
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Error.WriteLine($"\n[Line {error.token.getLine()} {error.Message}]");
        Console.ResetColor();
        hadRunTimeError = true;
    }
}
