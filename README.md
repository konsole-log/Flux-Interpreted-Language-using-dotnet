# Flux

A small interpreted language, written in C#/.NET.

```
func fib(n){
  if (n<=1) return n;
  return fib(n-2)+fib(n-1);
}

for(let i=0;i<20;i=i+1){
  print fib(i);
}
```

## Features

- Variables with `let`
- Functions (`func`) with closures and recursion
- `if` / `else` conditionals
- `for` and `while` loops
- Arithmetic (`+ - * /`), comparison (`< <= > >= == !=`), and logical (`and`, `or`, `!`) operators
- Strings, numbers, booleans, and `nil`
- `print` statement
- Built-in functions: `clock()`, `input()`, `inputNum()`
- A REPL for running code interactively

## Examples

**Variables and print**
```
let name = "Flux";
print "Hello, " + name;
```

**If / else**
```
let age = 20;
if (age >= 18) {
  print "adult";
} else {
  print "minor";
}
```

**While loop**
```
let i = 0;
while (i < 5) {
  print i;
  i = i + 1;
}
```

**Functions and closures**
```
func makeCounter() {
  let count = 0;
  func increment() {
    count = count + 1;
    return count;
  }
  return increment;
}

let counter = makeCounter();
print counter(); // 1
print counter(); // 2
```

**Reading input**
```
print "What's your name?";
let name = input();
print "Hi, " + name;
```

## Install

Grab the binary for your platform from the [Releases](../../releases) page — there's nothing else to install, it's a single file with the .NET runtime baked in.

**macOS / Linux**
```
tar -xzf flux-<platform>.tar.gz
chmod +x flux
sudo mv flux /usr/local/bin/
```

**Windows**
Unzip `flux-win-x64.zip` and put `flux.exe` somewhere on your PATH (e.g. `C:\tools`, then add that folder to PATH via System Properties → Environment Variables).

Either way, once it's on your PATH:

```
flux test.flux
```

### Building the binaries yourself

If you'd rather build from source than use a release, you only need the [.NET SDK](https://dotnet.microsoft.com/download) for this one-time step — people who just want to *run* Flux still don't need it.

```
git clone https://github.com/konsole-log/Flux-Interpreted-Language-using-dotnet.git
cd Flux-Interpreted-Language-using-dotnet
./build.sh
```

This spits out a ready-to-use archive per platform in `dist/`. Pick the one for your OS and follow the same steps as above.

### Running without installing anything

If you just want to try it out without putting anything on your PATH:

```
dotnet run --project Flux.CLI -- test.flux
```

## Usage

```
flux                 # starts a REPL
flux file.flux        # runs a file
flux --tokens file.flux   # prints the token stream
flux --ast file.flux      # prints the parsed AST
flux --all file.flux      # prints tokens, AST, and output together
```

Only `.flux` files are accepted — anything else gets rejected with an error.

## Project layout

- `Flux.Language` — the actual language: lexer, parser, AST, interpreter
- `Flux.CLI` — the command-line front end (`flux`)
- `tool/generate_ast.py` — script used to generate the AST node classes

## Development

For hacking on the interpreter itself:

```
dotnet build
```

or open `Flux.slnx` in your IDE of choice.
