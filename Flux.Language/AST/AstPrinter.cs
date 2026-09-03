using System.Text;
using Flux.Language.Lexer;

namespace Flux.Language.AST;

public class AstPrinter : Expr.Visitor<string>, Stmt.Visitor<string>
{
    private int indent = 0;

    // =========================
    // Public Print Methods
    // =========================

    public string Print(Expr expr)
    {
        indent = 0;
        return PrintExpressionTree(expr, "", true);
    }

    public string Print(Stmt stmt)
    {
        indent = 0;
        return PrintStatementTree(stmt, "", true);
    }

    public string Print(List<Stmt> statements)
    {
        indent = 0;

        StringBuilder builder = new();

        builder.AppendLine("Program");

        for (int i = 0; i < statements.Count; i++)
        {
            bool last = i == statements.Count - 1;

            builder.Append(PrintStatementTree(statements[i], "", last, true));
        }

        return builder.ToString();
    }

    // =========================
    // Expression Visitors
    // =========================

    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return "";
    }

    public string VisitCallExpr(Expr.Call expr)
    {
        return "";
    }

    public string VisitAssignExpr(Expr.Assign expr)
    {
        return "";
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return "";
    }

    public string VisitLiteralExpr(Expr.Literal expr)
    {
        return "";
    }

    public string VisitLogicalExpr(Expr.Logical expr)
    {
        return "";
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return "";
    }

    public string VisitVariableExpr(Expr.Variable expr)
    {
        return "";
    }

    // =========================
    // Statement Visitors
    // =========================

    public string VisitBlockStmt(Stmt.Block stmt)
    {
        return "";
    }

    public string VisitExpressionStmt(Stmt.Expression stmt)
    {
        return "";
    }

    public string VisitFunctionStmt(Stmt.Function stmt)
    {
        return "";
    }

    public string VisitIfStmt(Stmt.If stmt)
    {
        return "";
    }

    public string VisitPrintStmt(Stmt.Print stmt)
    {
        return "";
    }

    public string VisitReturnStmt(Stmt.Return stmt)
    {
        return "";
    }

    public string VisitLetStmt(Stmt.Let stmt)
    {
        return "";
    }

    public string VisitWhileStmt(Stmt.While stmt)
    {
        return "";
    }

    // =========================
    // Expression Tree Printer
    // =========================

    private string PrintExpressionTree(Expr? expr, string prefix, bool last)
    {
        StringBuilder builder = new();

        string branch = last ? "└── " : "├── ";

        switch (expr)
        {
            case Expr.Binary binary:
                builder.AppendLine(prefix + branch + "Binary: " + binary.getOpr().getLexeme());

                builder.Append(
                    PrintExpressionTree(binary.getLeft(), prefix + (last ? "    " : "│   "), false)
                );

                builder.Append(
                    PrintExpressionTree(binary.getRight(), prefix + (last ? "    " : "│   "), true)
                );

                break;

            case Expr.Unary unary:
                builder.AppendLine(prefix + branch + "Unary: " + unary.getOpr().getLexeme());

                builder.Append(
                    PrintExpressionTree(unary.getRight(), prefix + (last ? "    " : "│   "), true)
                );

                break;

            case Expr.Logical logical:
                builder.AppendLine(prefix + branch + "Logical: " + logical.getOpr().getLexeme());

                builder.Append(
                    PrintExpressionTree(logical.getLeft(), prefix + (last ? "    " : "│   "), false)
                );

                builder.Append(
                    PrintExpressionTree(logical.getRight(), prefix + (last ? "    " : "│   "), true)
                );

                break;

            case Expr.Grouping grouping:
                builder.AppendLine(prefix + branch + "Grouping");

                builder.Append(
                    PrintExpressionTree(
                        grouping.getExpression(),
                        prefix + (last ? "    " : "│   "),
                        true
                    )
                );

                break;

            case Expr.Literal literal:
                builder.AppendLine(prefix + branch + "Literal: " + (literal.getValue() ?? "nil"));

                break;

            case Expr.Variable variable:
                builder.AppendLine(prefix + branch + "Variable: " + variable.getName().getLexeme());

                break;

            case Expr.Assign assign:
                builder.AppendLine(prefix + branch + "Assign: " + assign.getName().getLexeme());

                builder.Append(
                    PrintExpressionTree(assign.getValue(), prefix + (last ? "    " : "│   "), true)
                );

                break;

            case Expr.Call call:
                builder.AppendLine(prefix + branch + "Call");

                builder.Append(
                    PrintExpressionTree(
                        call.getCallee(),
                        prefix + (last ? "    " : "│   "),
                        call.getArguments().Count == 0
                    )
                );

                for (int i = 0; i < call.getArguments().Count; i++)
                {
                    bool argumentLast = i == call.getArguments().Count - 1;

                    builder.Append(
                        PrintExpressionTree(
                            call.getArguments()[i],
                            prefix + (last ? "    " : "│   "),
                            argumentLast
                        )
                    );
                }

                break;
        }

        return builder.ToString();
    }

    // =========================
    // Statement Tree Printer
    // =========================

    private string PrintStatementTree(Stmt stmt, string prefix, bool last, bool root = false)
    {
        StringBuilder builder = new();

        string branch = root ? "" : (last ? "└── " : "├── ");

        switch (stmt)
        {
            case Stmt.Let let:
                builder.AppendLine(prefix + branch + "Let: " + let.getName().getLexeme());

                builder.Append(
                    PrintExpressionTree(
                        let.getInitializer(),
                        prefix + (last ? "    " : "│   "),
                        true
                    )
                );

                break;

            case Stmt.Print print:
                builder.AppendLine(prefix + branch + "Print");

                builder.Append(
                    PrintExpressionTree(
                        print.getExpression(),
                        prefix + (last ? "    " : "│   "),
                        true
                    )
                );

                break;

            case Stmt.Expression expression:
                builder.AppendLine(prefix + branch + "Expression");

                builder.Append(
                    PrintExpressionTree(
                        expression.getExpression(),
                        prefix + (last ? "    " : "│   "),
                        true
                    )
                );

                break;

            case Stmt.Block block:
                builder.AppendLine(prefix + branch + "Block");

                for (int i = 0; i < block.getStatements().Count; i++)
                {
                    bool childLast = i == block.getStatements().Count - 1;

                    builder.Append(
                        PrintStatementTree(
                            block.getStatements()[i],
                            prefix + (last ? "    " : "│   "),
                            childLast
                        )
                    );
                }

                break;

            case Stmt.If ifStmt:
                builder.AppendLine(prefix + branch + "If");

                string childPrefix = prefix + (last ? "    " : "│   ");

                builder.AppendLine(childPrefix + "├── Condition");

                builder.Append(
                    PrintExpressionTree(ifStmt.getCondition(), childPrefix + "│   ", true)
                );

                builder.AppendLine(childPrefix + "└── Then");

                builder.Append(
                    PrintStatementTree(
                        ifStmt.getThenbranch(),
                        childPrefix + "    ",
                        ifStmt.getElsebranch() == null,
                        true
                    )
                );

                if (ifStmt.getElsebranch() != null)
                {
                    builder.AppendLine(childPrefix + "└── Else");

                    builder.Append(
                        PrintStatementTree(
                            ifStmt.getElsebranch()!,
                            childPrefix + "    ",
                            true,
                            true
                        )
                    );
                }

                break;

            case Stmt.While whileStmt:
                builder.AppendLine(prefix + branch + "While");

                string whilePrefix = prefix + (last ? "    " : "│   ");

                builder.AppendLine(whilePrefix + "├── Condition");

                builder.Append(
                    PrintExpressionTree(whileStmt.getCondition(), whilePrefix + "│   ", true)
                );

                builder.AppendLine(whilePrefix + "└── Body");

                builder.Append(
                    PrintStatementTree(whileStmt.getBody(), whilePrefix + "    ", true, true)
                );

                break;

            case Stmt.Return returnStmt:
                builder.AppendLine(prefix + branch + "Return");

                builder.Append(
                    PrintExpressionTree(
                        returnStmt.getValue(),
                        prefix + (last ? "    " : "│   "),
                        true
                    )
                );

                break;

            case Stmt.Function function:
                builder.AppendLine(prefix + branch + "Function: " + function.getName().getLexeme());

                string functionPrefix = prefix + (last ? "    " : "│   ");

                builder.AppendLine(functionPrefix + "├── Parameters");

                string parameterPrefix = functionPrefix + "│   ";

                for (int i = 0; i < function.getParameters().Count; i++)
                {
                    bool parameterLast = i == function.getParameters().Count - 1;

                    builder.AppendLine(
                        parameterPrefix
                            + (parameterLast ? "└── " : "├── ")
                            + function.getParameters()[i].getLexeme()
                    );
                }

                builder.AppendLine(functionPrefix + "└── Body");

                string bodyPrefix = functionPrefix + "    ";

                for (int i = 0; i < function.getBody().Count; i++)
                {
                    bool bodyLast = i == function.getBody().Count - 1;

                    builder.Append(PrintStatementTree(function.getBody()[i], bodyPrefix, bodyLast));
                }

                break;
        }

        return builder.ToString();
    }
}
