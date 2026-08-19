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
        else if (args.Length == 1) { 
            RunFile(args[0]);
        }else{
            RunPrompt();
        }
    }
    private static void RunFile(string path){
        if(!File.Exists(path)){
            Console.Error.WriteLine($"File not found : {path}");
            return;
        }
        Run(File.ReadAllText(path));
    }
    private static void RunPrompt(){

    }
    private static void Run(string path){

    }
}
