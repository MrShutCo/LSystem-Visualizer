namespace ShutCo.UI.Core.Rules;

public class SimpleProdRule : IProductionRule
{
    public string Predecessor { get; }
    public List<string> Successor { get; }
    
    public SimpleProdRule(string predecessor, string successor)
    {
        Predecessor = predecessor;
        Successor = LSystem.StringToWord(successor);
    }

    public bool CanApply(ILSystem system, List<string> word, int index)
    {
        return word[index] == Predecessor;
    }

    public List<string> Apply()
    {
        return Successor;
    }
}