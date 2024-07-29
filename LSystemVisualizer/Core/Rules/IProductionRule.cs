namespace ShutCo.UI.Core.Rules;

public interface IProductionRule
{
    public bool CanApply(ILSystem system, List<string> word, int index);
    public List<string> Apply();
}