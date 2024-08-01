using LSystemVisualizer.Core.Parser;

namespace ShutCo.UI.Core.Rules;

public class ParametricRule
{
    private ASTNode _predecessor;
    private ASTNode _condition;
    private ASTNode _successor;

    private List<string> _parameters;

    public ParametricRule(string rule)
    {
        var s = rule.Split(":");
        var pred = s[0];
        s = s[1].Split("->");
        var (condition, successor) = (s[0], s[1]);

        var predTokens = Tokenizer.Tokenize(pred);
        var conditionTokens = Tokenizer.Tokenize(condition);
        var successorTokens = Tokenizer.Tokenize(successor);

        _predecessor = Parser.ParseModule(new Queue<Token>(predTokens))!;
        _condition = Parser.ParseCondition(new Queue<Token>(conditionTokens))!;
        _successor = Parser.ParseModuleList(successorTokens)!;

        _parameters = GetParams();
    }

    public string GetModuleLetter()
    {
        return _predecessor.Value;
    }

    public List<(string word, List<double> values)>? TryApply(string letter, List<double> currValues)
    {
        if (_predecessor.Value != letter) return null;
        
        // Predecessor value mapping
        var currValuesMapping = new Dictionary<string, double>();
        for (int i = 0; i < currValues.Count; i++)
        {
            currValuesMapping.Add(_parameters[i], currValues[i]);
        }

        if (Evaluator.EvaluateCondition(_condition, currValuesMapping) == false)
        {
            return null;
        }
        
        // Do calculations
        return Evaluator.Evaluate(_successor, currValuesMapping);
    }

    List<string> GetParams()
    {
        List<string> parameters = [];
        foreach (var expr in _predecessor.ChildNodes[0].ChildNodes)
        {
            var param = expr.ChildNodes[0].ChildNodes[0].ChildNodes[0] as ParameterNode;
            parameters.Add(param.Value);
        }

        return parameters;
    }
}