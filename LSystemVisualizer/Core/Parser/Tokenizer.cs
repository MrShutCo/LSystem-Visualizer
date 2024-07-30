using System.Text.RegularExpressions;

namespace ShutCo.UI.Core.Rules;

public enum TokenType
{
    Symbol,
    Constant,
    Parameter,
    Conditional,
    BinaryOp,
    Parenthesis,
    Comma
}

public class Token
{
    public TokenType Type { get; }
    public string Value { get; }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type}: {Value}";
    }
}

public static class Tokenizer
{
    public static (string Letter, List<string> Parameters) ParsePredecessor(string pred)
    {
        var match = Regex.Match( pred,@"([A-Z]*)\((.*)\)");
        var letter = match.Groups[1].Value;
        var parameters = match.Groups[1].Value.Split(",");
        return (letter, parameters.ToList());
    }

    public static List<Token> Tokenize(string word)
    {
        var tokens = new List<Token>();
        for (var i = 0; i < word.Length; i++)
        {
            if (char.IsUpper(word[i]))
            {
                tokens.Add(ParseLetter(word, ref i));
            } 
            else if (char.IsLower(word[i]))
            {
                tokens.Add(new Token(TokenType.Parameter, word[i].ToString()));
            }
            else if (char.IsNumber(word[i]))
            {
                tokens.Add(ParseConstant(word, ref i));
            }
            else switch (word[i])
            {
                case '<' or '>' or '=':
                    tokens.Add(ParseConditional(word, ref i));
                    break;
                case '+' or '-' or '*' or '/':
                    tokens.Add(new Token(TokenType.BinaryOp, word[i].ToString()));
                    break;
                case '(' or ')':
                    tokens.Add(new Token(TokenType.Parenthesis, word[i].ToString()));
                    break;
                case ',':
                    tokens.Add(new Token(TokenType.Comma, word[i].ToString()));
                    break;
            }
        }

        return tokens;
    }

    private static Token ParseConditional(string word, ref int i)
    {
        string tokvalue = word[i].ToString();
        if (i + 1 < word.Length && word[i + 1] == '=')
        {
            tokvalue += word[i + 1];
            i++;
        }
        return new Token(TokenType.Conditional, tokvalue);
    }

    private static Token ParseWhile(string word, ref int i, Func<char, bool> predicate, TokenType type)
    {
        var tokValue = "";
        while (i < word.Length && predicate(word[i]))
        {
            tokValue += word[i];
            i++;
        }
        i--;
        return new Token(type, tokValue);
    }
    
    private static Token ParseLetter(string word, ref int i)
        =>ParseWhile(word, ref i, char.IsUpper, TokenType.Symbol);
    
    private static Token ParseConstant(string word, ref int i)
        =>ParseWhile(word, ref i, char.IsNumber, TokenType.Constant);
    
}