namespace ShutCo.UI.Core.Rules;

public class ParametricRule : IProductionRule
{
    
    private double[] _parameters;
    public string Predecessor;

    public ParametricRule(string rule)
    {
        var s = rule.Split(":");
        var pred = s[0];
        s = s[1].Split("->");
        var (condition, successor) = (s[0], s[1]);

        var predTokens = Tokenizer.Tokenize(pred);
        var conditionTokens = Tokenizer.Tokenize(condition);
        var successorTokens = Tokenizer.Tokenize(successor);

        var predTree = Parser.ParseModule(new Queue<Token>(predTokens));
        var conditionTree = Parser.ParseCondition(new Queue<Token>(conditionTokens));
        var successorTree = Parser.ParseSuccessor(successorTokens);
        Console.WriteLine();
    }
    
    public bool CanApply(ILSystem system, List<string> word, int index)
    {
        return false;
    }

    public List<string> Apply()
    {
        return [];
    }
}