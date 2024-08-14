using LSystemVisualizer.Core.Parser;

namespace ShutCo.UI.Core.Rules;

public class ParametricRule
{
    public ASTNode Predecessor { get; }
    public ASTNode? Condition { get; }
    public ASTNode Successor { get; }
    
    public string PredecessorText { get; }
    public string ConditionText { get; }
    public string SuccessorText { get; }

    private List<string> _parameters;
    private string ruleText;

    public ParametricRule(string rule)
    {
        ruleText = rule;
        var s = rule.Split(":");
        var pred = s[0];
        s = s[1].Split("->");
        var (condition, successor) = (s[0], s[1]);

        var predTokens = Tokenizer.Tokenize(pred);
        var conditionTokens = Tokenizer.Tokenize(condition);
        var successorTokens = Tokenizer.Tokenize(successor);
        PredecessorText = pred;
        ConditionText = condition;
        SuccessorText = successor;

        Predecessor = Parser.ParseModule(new Queue<Token>(predTokens))!;
        Condition = Parser.ParseModuleCondition(new Queue<Token>(conditionTokens))!;
        Successor = Parser.ParseModuleList(successorTokens)!;

        _parameters = GetParams();
    }

    public override string ToString()
    {
        return ruleText;
    }

    public string GetModuleLetter()
    {
        return Predecessor.Value;
    }

    public List<(string word, List<double> values)>? TryApply(string letter, List<double> currValues, Dictionary<string, double> defines)
    {
        if (Predecessor.Value != letter) return null;
        
        // Predecessor value mapping
        var currValuesMapping = new Dictionary<string, double>(defines);
        for (int i = 0; i < currValues.Count; i++)
        {
            currValuesMapping.Add(_parameters[i], currValues[i]);
        }

        if (Condition != null && Evaluator.EvaludateConditionModule(Condition, currValuesMapping) == false)
        {
            return null;
        }
        
        // Do calculations
        return Evaluator.Evaluate(Successor, currValuesMapping);
    }

    List<string> GetParams()
    {
        List<string> parameters = [];
        foreach (var expr in Predecessor.ChildNodes[0].ChildNodes)
        {
            var param = expr.ChildNodes[0].ChildNodes[0].ChildNodes[0] as ParameterNode;
            parameters.Add(param.Value);
        }

        return parameters;
    }
}