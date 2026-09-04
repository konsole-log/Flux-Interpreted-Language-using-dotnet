import sys


def eprint(*args, **kwargs):
    print(*args, file=sys.stderr, **kwargs)


# Handle CLI args
if len(sys.argv) != 2:
    eprint("Usage: python generate_ast.py <output directory>")
    exit(64)

output_dir = sys.argv[1]

# AST metaprogramming
ast_types = [
    "Binary    : Expr left, Token opr, Expr right",
    "Call: Expr callee, Token paren, List<Expr> arguments",
    "Assign    : Token name, Expr value",
    "Grouping  : Expr expression",
    "Literal   : Object? value",
    "Logical   : Expr left, Token opr, Expr right",
    "Unary     : Token opr, Expr right",
    "Variable  : Token name",
]

stmt_types = [
    "Block      : List<Stmt> statements",
    "Expression : Expr expression",
    "Function   : Token name, List<Token> parameters, List<Stmt> body",
    "If         : Expr condition, Stmt thenBranch, Stmt? elseBranch",
    "Print      : Expr expression",
    "Return     : Token keyword, Expr value",
    "Let        : Token name, Expr initializer",
    "While      : Expr condition, Stmt body",
]


def define_type(f, class_name, base_name, fields_str):
    f.write("\tpublic class %s : %s {\n\n" % (class_name, base_name))

    fields = fields_str.split(", ")

    # Write fields
    for field in fields:
        f.write("\t\treadonly %s;\n" % field)
    f.write("\n")

    # Write getters
    for field in fields:
        field_type = field.split()[0]
        field_val = field.split()[1]
        f.write("\t\tpublic %s get%s() {\n" % (field_type, field_val.capitalize()))
        f.write("\t\t\treturn %s;\n" % field_val)
        f.write("\t\t}\n\n")

    # Write constructor
    f.write("\t\tpublic %s(%s) {\n" % (class_name, fields_str))
    for field in fields:
        name = field.split(" ")[1]
        f.write("\t\t\tthis.%s = %s;\n" % (name, name))
    f.write("\t\t}\n\n")

    # Write visitor pattern
    f.write("\t\tpublic override T Accept<T>(Visitor<T> visitor) {\n")
    f.write("\t\t\treturn visitor.Visit%s%s(this);\n" % (class_name, base_name))
    f.write("\t\t}\n")

    f.write("\t}\n\n")


def define_visitor(f, base_name, types):
    f.write("\tpublic interface Visitor<T> {\n")

    # Make generic visitor method for each type
    for type in types:
        type_name = type.split(":")[0].strip()
        f.write(
            "\t\tpublic T Visit%s%s(%s %s);\n"
            % (type_name, base_name, type_name, base_name.lower())
        )

    f.write("\t}\n\n")


def define_ast(output_dir, base_name, types=[], *args):
    path = f"{output_dir}/{base_name}.cs"

    # Write abstract class and subclasses
    f = open(path, "w")
    f.write(
        "using Flux.Language.Lexer;\nusing Flux.Language.Diagnostics;\nnamespace Flux.Language.AST;\n\n"
    )
    f.write("public abstract class %s {\n\n" % base_name)

    # Write abstract accept method
    f.write("\tpublic abstract T Accept<T>(Visitor<T> visitor);\n\n")

    define_visitor(f, base_name, types)

    for type in types:
        class_name = type.split(":")[0].strip()
        fields = type.split(":")[1].strip()

        define_type(f, class_name, base_name, fields)

    f.write("}")
    f.close()


def define_ast_printer(output_dir, expr_types, stmt_types):
    path = f"{output_dir}/AstPrinter.cs"

    f = open(path, "w")

    f.write("using System.Text;\n\nnamespace Flux.Language.AST;\n\n")

    f.write(
        "public class AstPrinter : Expr.Visitor<string>, Stmt.Visitor<string>\n{\n\n"
    )

    # Print Expr
    f.write(
        "\tpublic string Print(Expr expr)\n\t{\n\t\treturn expr.Accept(this);\n\t}\n\n"
    )

    # Print Stmt
    f.write(
        "\tpublic string Print(Stmt stmt)\n\t{\n\t\treturn stmt.Accept(this);\n\t}\n\n"
    )

    # Generate Expr visitor methods
    for type in expr_types:
        type_name = type.split(":")[0].strip()

        f.write(
            f"\tpublic string Visit{type_name}Expr(Expr.{type_name} expr)\n"
            "\t{\n"
            '\t\treturn "";\n'
            "\t}\n\n"
        )

    # Generate Stmt visitor methods
    for type in stmt_types:
        type_name = type.split(":")[0].strip()

        f.write(
            f"\tpublic string Visit{type_name}Stmt(Stmt.{type_name} stmt)\n"
            "\t{\n"
            '\t\treturn "";\n'
            "\t}\n\n"
        )

    f.write("}\n")

    f.close()


define_ast(output_dir, "Expr", ast_types)
define_ast(output_dir, "Stmt", stmt_types)
define_ast_printer(output_dir, ast_types, stmt_types)
