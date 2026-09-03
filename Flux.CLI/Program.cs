using Flux.Language.AST;
using Flux.Language.Diagnostics;
using Flux.Language.Highlighting;
using Flux.Language.Interpreter;
using Flux.Language.Lexer;
using Flux.Language.Parser;

namespace Flux.CLI;

public class Flux
{
    private static readonly Interpreter interpreter = new Interpreter();

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            RunPrompt();
            return;
        }

        if (args.Length == 2)
        {
            string option = args[0];
            string path = args[1];

            if (Path.GetExtension(path) != ".flux")
            {
                Console.WriteLine("Flux: only '.flux' files are supported.");
                Environment.Exit(64);
            }

            switch (option)
            {
                case "--tokens":
                    RunTokens(path);
                    break;

                case "--ast":
                    RunAst(path);
                    break;

                case "--all":
                    RunAll(path);
                    break;

                default:
                    Console.WriteLine($"Flux: unknown option '{option}'.");
                    Console.WriteLine();
                    PrintUsage();
                    Environment.Exit(64);
                    break;
            }

            return;
        }

        if (args.Length == 1)
        {
            RunFile(args[0]);
            return;
        }

        PrintUsage();
        Environment.Exit(64);
    }

    private static void RunFile(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Flux: file '{path}' not found.");
            Environment.Exit(66);
        }

        if (Path.GetExtension(path) != ".flux")
        {
            Console.WriteLine("Flux: only '.flux' files are supported.");
            Environment.Exit(64);
        }

        string source = File.ReadAllText(path);

        Run(source);

        if (ErrorReporter.hadError)
            Environment.Exit(65);

        if (ErrorReporter.hadRunTimeError)
            Environment.Exit(70);
    }

    private static void RunTokens(string path)
    {
        if (!ValidateFile(path))
            return;

        string source = File.ReadAllText(path);

        Console.WriteLine("========== SOURCE ==========");
        Console.WriteLine(source);

        Console.WriteLine();
        Console.WriteLine("========== TOKENS ==========");

        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.ScanTokens();

        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }

        if (ErrorReporter.hadError)
            Environment.Exit(65);
    }

    private static void RunAst(string path)
    {
        if (!ValidateFile(path))
            return;

        string source = File.ReadAllText(path);

        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.ScanTokens();

        if (ErrorReporter.hadError)
            Environment.Exit(65);

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();

        if (ErrorReporter.hadError)
            Environment.Exit(65);

        Console.WriteLine("========== AST ==========");

        AstPrinter printer = new AstPrinter();

        Console.WriteLine(printer.Print(statements));
    }

    private static void RunAll(string path)
    {
        if (!ValidateFile(path))
            return;

        string source = File.ReadAllText(path);

        // SOURCE
        Console.WriteLine("========== SOURCE ==========");
        Console.WriteLine(source);

       //Lexer 
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.ScanTokens();

        // SYNTAX HIGHLIGHT
        Console.WriteLine();
        Console.WriteLine("========== SYNTAX HIGHLIGHT ==========");
        Console.WriteLine(SyntaxHighlighter.Highlight(source, tokens));

        // Tokens
        Console.WriteLine();
        Console.WriteLine("========== TOKENS ==========");


        foreach (Token token in tokens)
        {
            Console.WriteLine(SyntaxHighlighter.HighlightToken(token));
        }

        if (ErrorReporter.hadError)
            Environment.Exit(65);

        // PARSER / AST
        Console.WriteLine();
        Console.WriteLine("========== AST ==========");

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();

        if (ErrorReporter.hadError)
            Environment.Exit(65);

        AstPrinter printer = new AstPrinter();
        Console.WriteLine(printer.Print(statements));

        // INTERPRETER
        Console.WriteLine();
        Console.WriteLine("========== OUTPUT ==========");

        interpreter.Interpret(statements);

        if (ErrorReporter.hadRunTimeError)
            Environment.Exit(70);
    }

    private static bool ValidateFile(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Flux: file '{path}' not found.");
            return false;
        }

        if (Path.GetExtension(path) != ".flux")
        {
            Console.WriteLine("Flux: only '.flux' files are supported.");
            return false;
        }

        return true;
    }

    private static void RunPrompt()
    {
        Console.WriteLine("Flux Programming Language");
        Console.WriteLine("Type 'exit' to quit.");
        Console.WriteLine();

        while (true)
        {
            Console.Write("flux> ");

            string? input = Console.ReadLine();

            if (input == "exit" || input == null)
                break;

            Run(input);

            ErrorReporter.hadError = false;
            ErrorReporter.hadRunTimeError = false;
        }
    }

    private static void Run(string source)
    {
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.ScanTokens();

        if (ErrorReporter.hadError)
            return;

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();

        if (ErrorReporter.hadError)
            return;

        interpreter.Interpret(statements);
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  flux <file.flux>");
        Console.WriteLine("  flux --tokens <file.flux>");
        Console.WriteLine("  flux --ast <file.flux>");
        Console.WriteLine("  flux --all <file.flux>");
    }
}
