using LSystemVisualizer.Core;
using LSystemVisualizer.Core.Parser;

namespace ShutCo.UI.Core.Rules;

public class ParametricRule
{
    public ASTNode Predecessor { get; private set; }
    public ASTNode? Condition { get; private set; }
    public ASTNode Successor { get; private set; }
    
    public string PredecessorText { get; private set; }
    public string ConditionText { get; private set; }
    public string SuccessorText { get; private set; }
    
    public string LeftContextText { get; }
    public string RightContextText { get; }

    private ASTNode? LeftContext;
    private ASTNode? RightContext;

    private List<string> _parameters;
    private List<string> _leftParams;
    private List<string> _rightParams;
    private string ruleText;

    public ParametricRule(string rule, string leftContext, string rightContext)
    {
        PrepareRule(rule);
        LeftContextText = leftContext;
        RightContextText = rightContext;
        LeftContext = Parser.ParseModule(new Queue<Token>(Tokenizer.Tokenize(LeftContextText)));
        RightContext = Parser.ParseModule(new Queue<Token>(Tokenizer.Tokenize(RightContextText)));
        _leftParams = GetParams(LeftContext);
        _rightParams = GetParams(RightContext);
    }
    
    public ParametricRule(string rule)
    {
        PrepareRule(rule);
        LeftContextText = "";
        RightContextText = "";
    }

    void PrepareRule(string rule)
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

        _parameters = GetParams(Predecessor);
    }

    public override string ToString()
    {
        var s = "";
        if (LeftContextText != "") s += LeftContext + " < ";
        s += PredecessorText;
        if (RightContextText != "") s += " > " + RightContext;
        return $"{s} : {ConditionText} -> {SuccessorText}";
    }

    public string GetModuleLetter()
    {
        return Predecessor.Value;
    }

    public List<Module>? TryApply(Module module, Dictionary<string, double> defines)
    {
        if (Predecessor.Value != module.Word) return null;
        
        // Predecessor value mapping
        var currValuesMapping = new Dictionary<string, double>(defines);
        for (int i = 0; i < module.Values.Count; i++)
        {
            currValuesMapping.Add(_parameters[i], module.Values[i]);
        }

        if (Condition != null && Evaluator.EvaludateConditionModule(Condition, currValuesMapping) == false)
        {
            return null;
        }
        
        // Do calculations
        return Evaluator.Evaluate(Successor, currValuesMapping);
    }
    
    public List<Module>? TryApplyContext(Module? left, Module main, Module? right, Dictionary<string, double> defines)
    {
        if (Predecessor.Value != main.Word) return null;
        
        // Predecessor value mapping
        var currValuesMapping = new Dictionary<string, double>(defines);
        for (int i = 0; i < main.Values.Count; i++)
        {
            currValuesMapping.Add(_parameters[i], main.Values[i]);
        }
        for (int i = 0; i < left?.Values.Count; i++)
        {
            currValuesMapping.Add(_leftParams[i], left.Values[i]);
        }
        for (int i = 0; i < right?.Values.Count; i++)
        {
            currValuesMapping.Add(_rightParams[i], right.Values[i]);
        }

        if (Condition != null && Evaluator.EvaludateConditionModule(Condition, currValuesMapping) == false)
        {
            return null;
        }

        // Left context doesnt match
        if (left is not null && LeftContext is not null && left.Word != LeftContext.Value)
        {
            return null;
        }
        
        // Right context doesnt match 
        if (right is not null && RightContext is not null && right.Word != RightContext.Value)
        {
            return null;
        }
        
        // Do calculations
        return Evaluator.Evaluate(Successor, currValuesMapping);
    }

    List<string> GetParams(ASTNode? module)
    {
        List<string> parameters = [];
        if (module is null || module.ChildNodes.Count == 0) return [];
        foreach (var expr in module.ChildNodes[0].ChildNodes)
        {
            var param = expr.ChildNodes[0].ChildNodes[0].ChildNodes[0] as ParameterNode;
            parameters.Add(param.Value);
        }

        return parameters;
    }
}