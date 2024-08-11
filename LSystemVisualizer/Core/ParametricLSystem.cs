using LSystemVisualizer.Core.Parser;
using ShutCo.UI.Core;
using ShutCo.UI.Core.Rules;
using TurtleGraphics.BlazorCanvas;

namespace LSystemVisualizer.Core;

public class ParametricLSystem : ILSystem
{
    private List<ParametricRule> _rules = [];
    private List<string> _lettersWithRules;

    public ParametricLSystem(List<ParametricRule> rules)
    {
        _rules = rules;
        _lettersWithRules = _rules.Select(r => r.GetModuleLetter()).ToList();
    }

    public List<string> StepLSystem(List<string> word, int iterations)
    {
        var newWord = word;
        for (int i = 0; i < iterations; i++)
        {
            newWord = StepLSystem(newWord);
            Console.WriteLine(newWord[0]);
        }

        return newWord;
    }

    List<string> StepLSystem(List<string> word)
    {
        // Build the AST Tree and evaluate. All Modules should only have constants, but this keeps things simple
        var tokens = Tokenizer.Tokenize(word[0]);
        // TODO: need to figure out how to keep track of where these constant letters were removed
        // and how to insert them back into the data
        //var constantLetters = RemoveNonParametricTokens(tokens);
        var astTree = Parser.Parser.ParseModuleList(tokens);

        string newWord = "";
        
        var emptyVals = new Dictionary<string, double>();
        
        // List of each module, and all of the current values
        var evaluations = Evaluator.Evaluate(astTree, emptyVals);

        foreach (var module in evaluations)
        {
            // Just a letter, replicate it
            if (module.values.Count == 0)
            {
                newWord += module.word;
                continue;
            }
            
            List<(string word, List<double> values)>? newValues = null;
            foreach (var rule in _rules)
            {
                // This may need to generate a list of stuff because more than one module may return
                newValues = rule.TryApply(module.word, module.values);
                if (newValues != null)
                {
                    break;
                }
            }
            
            if (newValues == null)
            {
                newWord += module.word;
            }
            else
            {
                foreach (var newModule in newValues)
                {
                    newWord += ModuleToString(newModule.word, newModule.values);
                }
            }
        }

        return [newWord];
    }

    /// <summary>
    /// Removes all tokens from the list that are not in some parametric rule. These tokens will just get replicated
    /// will not be attempted to be parsed
    /// </summary>
    List<(string, int)> RemoveNonParametricTokens(List<Token> tokens)
    {
        List<(string, int)> removedTokens = [];
        for (int i = tokens.Count-1; i >= 0; i--)
        {
            if (tokens[i].Type == TokenType.Symbol && _lettersWithRules.Contains(tokens[i].Value))
            {
                tokens.RemoveAt(i);
                removedTokens.Add((tokens[i].Value, i));
            }
        }

        return removedTokens;
    }

    string ModuleToString(string word, List<double> values)
    {
        string s = word;

        if (values.Count == 0) return s;

        s += "(";
        
        for (int i = 0; i < values.Count; i++)
        {
            s += values[i].ToString();
            if (i < values.Count - 1) s += ",";
        }

        s += ")";
        return s;
    }

    public virtual async Task StepTurtle(string letter, Turtle turtle, int iterations)
    {
        await Task.Delay(100);
    }

    public List<string> Ignore { get; set; }
}