namespace Flux.Language.Interpreter;

internal class NativeFunction : FluxCallable
{
    private int arity;
    private readonly Func<object, object, double>? funcnum;
    private readonly Func<object, object, string?>? funcstr;
    private readonly Func<object, object, bool>? funcbool;

    public NativeFunction(int arity, Func<object, object, double> func)
    {
        this.arity = arity;
        this.funcnum = func;
    }

    public NativeFunction(int arity, Func<object, object, string?> func)
    {
        this.arity = arity;
        this.funcstr = func;
    }

    public NativeFunction(int arity, Func<object, object, bool> func)
    {
        this.arity = arity;
        this.funcbool = func;
    }

    public int Arity()
    {
        return arity;
    }

    public Object? Call(Interpreter interpreter, List<Object?> arguments)
    {
        return funcbool != null
            ? funcbool.Invoke(interpreter, arguments)
            : HandleNum(interpreter, arguments);
    }

    public Object? HandleNum(Interpreter interpreter, List<Object?> arguments)
    {
        return funcnum != null
            ? funcnum.Invoke(interpreter, arguments)
            : funcstr.Invoke(interpreter, arguments);
    }

    public override string ToString()
    {
        return "<native fn>";
    }
}
