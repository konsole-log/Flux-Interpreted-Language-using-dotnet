using System.Drawing;
using System.Text;
using Flux.Language.Lexer;

namespace Flux.Language.Highlighting;

public static class SyntaxHighlighter
{
    // ANSI 24-bit colors.
    // These are based loosely on the Rose Pine palette.

    private const string Reset = "\u001b[0m";

    private const string Keyword = "\u001b[38;2;235;188;186m";
    private const string Function = "\u001b[38;2;196;167;231m";
    private const string String = "\u001b[38;2;246;193;119m";
    private const string Number = "\u001b[38;2;234;157;172m";
    private const string Identifier = "\u001b[38;2;224;222;244m";
    private const string Operator = "\u001b[38;2;156;207;216m";
    private const string Punctuation = "\u001b[38;2;144;140;170m";
    private const string Comment = "\u001b[38;2;110;106;134m";

    public static string Highlight(string source, List<Token> tokens)
    {
        StringBuilder result = new StringBuilder();

        int position = 0;

        foreach (Token token in tokens)
        {
            // Don't print EOF.
            if (token.getType() == TokenType.EOF)
                break;

            string lexeme = token.getLexeme();

            if (string.IsNullOrEmpty(lexeme))
                continue;

            /*
             * Find this token in the original source starting from
             * the current position.
             *
             * This preserves all whitespace and indentation from
             * the original Flux source.
             */
            int tokenPosition = source.IndexOf(lexeme, position);

            if (tokenPosition == -1)
            {
                // Fallback in case the lexeme cannot be found.
                result.Append(ColorForToken(token));
                result.Append(lexeme);
                result.Append(Reset);
                continue;
            }

            // Print everything between the previous token and this token.
            string gap = source[position..tokenPosition];

            AppendGap(result, gap);

            // Print the actual token with its color.
            result.Append(ColorForToken(token));
            result.Append(lexeme);
            result.Append(Reset);

            position = tokenPosition + lexeme.Length;
        }

        // Print anything remaining after the final token.
        if (position < source.Length)
        {
            AppendGap(result, source[position..]);
        }

        return result.ToString();
    }

    private static string ColorForToken(Token token)
    {
        TokenType type = token.getType();

        string typeName = type.ToString().ToUpperInvariant();

        // Keywords
        if (IsKeyword(typeName))
            return Keyword;

        // Strings
        if (type == TokenType.STRING)
            return String;

        // Numbers
        if (type == TokenType.NUMBER)
            return Number;

        // Identifiers
        if (type == TokenType.IDENTIFIER)
            return Identifier;

        // Operators
        if (IsOperator(typeName))
            return Operator;

        // Parentheses, braces, commas, semicolons, etc.
        if (IsPunctuation(typeName))
            return Punctuation;

        return Identifier;
    }

    private static bool IsKeyword(string type)
    {
        return type switch
        {
            "AND" => true,
            "OR" => true,

            "IF" => true,
            "ELSE" => true,

            "FOR" => true,
            "WHILE" => true,

            "FUN" => true,
            "RETURN" => true,

            "LET" => true,
            "CLASS" => true,

            "PRINT" => true,

            "TRUE" => true,
            "FALSE" => true,
            "NIL" => true,

            _ => false
        };
    }

    private static bool IsOperator(string type)
    {
        return type switch
        {
            "PLUS" => true,
            "MINUS" => true,
            "STAR" => true,
            "SLASH" => true,

            "EQUAL" => true,
            "EQUAL_EQUAL" => true,

            "BANG" => true,
            "BANG_EQUAL" => true,

            "GREATER" => true,
            "GREATER_EQUAL" => true,

            "LESS" => true,
            "LESS_EQUAL" => true,

            _ => false
        };
    }

    private static bool IsPunctuation(string type)
    {
        return type switch
        {
            "LEFT_PAREN" => true,
            "RIGHT_PAREN" => true,

            "LEFT_BRACE" => true,
            "RIGHT_BRACE" => true,

            "COMMA" => true,
            "DOT" => true,
            "SEMICOLON" => true,

            _ => false
        };
    }

    private static void AppendGap(StringBuilder result, string gap)
    {
        int position = 0;

        while (position < gap.Length)
        {
            // Detect // comments.
            if (
                position + 1 < gap.Length &&
                gap[position] == '/' &&
                gap[position + 1] == '/'
            )
            {
                int end = gap.IndexOf('\n', position);

                if (end == -1)
                    end = gap.Length;

                result.Append(Comment);
                result.Append(gap[position..end]);
                result.Append(Reset);

                position = end;
            }
            else
            {
                result.Append(gap[position]);
                position++;
            }
        }
    }
     public static string HighlightToken(Token token){
         if(token.getType()==TokenType.EOF){
             return "";
         }
         string color = ColorForToken(token);
         return $"{color}{token}{Reset}";

     }
}
