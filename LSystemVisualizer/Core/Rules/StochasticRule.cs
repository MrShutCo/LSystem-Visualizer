namespace ShutCo.UI.Core.Rules;

public class StochasticRule : IProductionRule
{
    private Random _rngSource;
    public string Predecessor { get; }
    public List<List<string>> Successors { get; }
    private RandomWeightedPicker Picker { get; }
    public List<float> Weights { get; }
    
    public StochasticRule(Random rngSource, string predecessor, List<string> successors, List<float> weights)
    {
        _rngSource = rngSource;
        Predecessor = predecessor;
        Weights = weights;
        Successors = successors.Select(s => LSystem.StringToWord(s)).ToList();
        Picker = new RandomWeightedPicker(weights, rngSource);
    }

    public bool CanApply(ILSystem system, List<string> word, int index)
    {
        return word[index] == Predecessor;
    }

    public List<string> Apply()
    {
        return Successors[Picker.PickAnItem()];
    }
}