namespace ShutCo.UI.Core.Rules;

public class ParametricRule : IProductionRule
{
    private double _t;
    public string Predecessor;

    public ParametricRule(double t, string predecessor, string condition, List<string> successor)
    {
        _t = t;
        Predecessor = predecessor;
        
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