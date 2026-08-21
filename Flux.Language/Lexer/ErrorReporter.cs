namespace Flux.Language.Lexer;

public class ErrorReporter
{
    public static bool hadError { get; set; }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string position, string message)
    {
        Console.Error.WriteLine($"[Line {line} Error {position} :{message}]");
        hadError = true;
    }
}
