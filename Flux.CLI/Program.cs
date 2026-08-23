using Flux.Language.Lexer;
using Flux.Language.Diagnostics;
namespace Flux.CLI;

public class Flux
{

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: flux [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        Run(File.ReadAllText(path));
        if (ErrorReporter.hadError)
            Environment.Exit(65);
    }
    private static void RunPrompt()
    {
        // Writing a REPL which also stands for Read a line of input, Evaluate it, Print the result, then Loop and do it all over again.
        Console.WriteLine("REPL for Flux:");
        Console.WriteLine($"Type 'exit' to quit");
        while (true)
        {
            Console.Write("> ");

            string? input = Console.ReadLine();
            if (input == "exit" || input == null)
            {
                break;
            }
            Run(input);
            ErrorReporter.hadError = false;
    }
    }

    private static void Run(string source)
    {
        //lexer code goes here
        Lexer lexer = new(source);
        List<Token> tokens = lexer.ScanTokens();
        foreach(Token token in tokens){
            Console.WriteLine(token);
        }
    }
}
