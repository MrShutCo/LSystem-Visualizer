using LSystemVisualizer.Core.Parser;
using ShutCo.UI.Core;
using ShutCo.UI.Core.Rules;
using TurtleGraphics.BlazorCanvas;

namespace LSystemVisualizer.Core;

public class ParametricLSystem : ILSystem
{
    public List<ParametricRule> Rules = [];
    private List<string> _lettersWithRules;
    public string StartingWord;
    public float Angle;
    public string Name;
    public float ScalingSize;
    public float Distance;
    
    public Dictionary<string, double> Defines;
    
    public ParametricLSystem(string startingWord, List<ParametricRule> rules, Dictionary<string, string> defines)
    {
        Rules = rules;
        StartingWord = startingWord;
        Defines = [];
        foreach (var definition in defines)
        {
            var definitionParseTree =Parser.Parser.ParseExpr(new Queue<Token>(Tokenizer.Tokenize(definition.Value)));
            var defValue = Evaluator.EvaluateExpression(definitionParseTree, Defines);
            Defines[definition.Key] = defValue;
        }
        
        _lettersWithRules = Rules.Select(r => r.GetModuleLetter()).ToList();
    }

    public List<string> StepLSystem(List<string> word, int iterations)
    {
        var newWord = word;
        for (var i = 0; i < iterations; i++)
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
        var newWord = "";
        var emptyVals = new Dictionary<string, double>(Defines);
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
            foreach (var rule in Rules)
            {
                // This may need to generate a list of stuff because more than one module may return
                newValues = rule.TryApply(module.word, module.values, Defines);
                if (newValues != null)
                {
                    break;
                }
            }
            
            if (newValues == null)
            {
                if (module.values.Count == 0) newWord += module.word;
                else newWord += ModuleToString(module.word, module.values);
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

    public virtual async Task StepTurtle(string word, Turtle turtle, int iterations)
    {
        var modules = Parser.Parser.ParseModuleList(Tokenizer.Tokenize(word));
        Stack<(float, float, float)> stateStack = [];
        foreach (var module in modules.ChildNodes)
        {
            var values = Evaluator.EvaluateModule(module, []);

            switch (module.Value)
            {
                case "F":
                    turtle.PenVisible = true;
                    await turtle.Forward(Distance * (float)values[0]);
                    break;
                case "f":
                    turtle.PenVisible = false;
                    await turtle.Forward(Distance * (float)values[0]);
                    break;
                case "+":
                    await turtle.Rotate(-Angle);
                    break;
                case "-":
                    await turtle.Rotate(Angle);
                    break;
                case "[":
                    stateStack.Push((turtle.X, turtle.Y, turtle.Angle));
                    break;
                case "]":
                    var currState = stateStack.Pop();
                    turtle.PenVisible = false;
                    await turtle.MoveTo(currState.Item1, currState.Item2);
                    await turtle.RotateTo(currState.Item3);
                    break;
            }
        }
    }

    public List<string> Ignore { get; set; }
}