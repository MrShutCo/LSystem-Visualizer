using ShutCo.UI.Core.Rules;
using TurtleGraphics.BlazorCanvas;

namespace ShutCo.UI.Core;

public class LSystem(
    string name,
    string startingWord,
    List<IProductionRule> productionRules,
    float distance,
    float angle,
    float scalingSize) : ILSystem
{
    public string Name { get; } = name;
    public string StartingWord { get; } = startingWord;
    public List<IProductionRule> ProductionRules { get; } = productionRules;
    public float Distance { get; set; } = distance;
    public float Angle { get; set; } = angle;
    public float ScalingSize { get; set; } = scalingSize;
    public List<string> Ignore { get; set; } = [];

    private Stack<(float, float, float)> stateStack = [];
    
    public virtual List<string> StepLSystem(List<string> word, int iterations)
    {
        var prevWord = word;
        for (var i = 0; i < iterations; i++)
        {
            List<string> newWord = [];
            for (var j = 0; j < prevWord.Count; j++)
            {
                var hasBeenApplied = false;
                foreach (var pr in ProductionRules)
                {
                    if (pr.CanApply(this, prevWord, j))
                    {
                        hasBeenApplied = true;
                        newWord.AddRange(pr.Apply());
                        break;
                    }
                }
                if (!hasBeenApplied)
                {
                    newWord.Add(prevWord[j]);
                }
            }
            prevWord = newWord;
        }

        return prevWord;
    }

    public static List<string> StringToWord(string word) => word.ToList().Select(ch => ch.ToString()).ToList();
    public static string WordToString(IEnumerable<string> word) => word.Aggregate((w, letter) => w += letter);

    public virtual async Task StepTurtle(string letter, Turtle turtle, int iterations)
    {
        var d = Distance / MathF.Pow(ScalingSize, iterations);
        switch (letter)
        {
            case "F" or "L" or "R":
                turtle.PenVisible = true;
                await turtle.Forward(d);
                break;
            case "f":
                turtle.PenVisible = false;
                await turtle.Forward(d);
                break;
            case "+":
                await turtle.Rotate(-angle);
                break;
            case "-":
                await turtle.Rotate(angle);
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