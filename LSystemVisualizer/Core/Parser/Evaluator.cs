using System.Reflection.Metadata;
using LSystemVisualizer.Core.Parser;

namespace ShutCo.UI.Core.Rules;

public class Evaluator
{
    
    public static List<(string word, List<double> values)> Evaluate(ASTNode moduleList, Dictionary<string, double> currValues)
    {
        if (moduleList.NodeType != "ModuleList") return [];

        List<(string word, List<double> values)> values = [];

        int i = 0;
        foreach (var module in moduleList.ChildNodes)
        {
            // Just a standalone letter
            if (module.ChildNodes.Count == 0)
            {
                values.Add((module.Value, []));
                continue;
            }
            values.Add((module.Value, EvaluateModule(module, currValues)));
            i++;
        }

        return values;
    }
    

    public static List<double> EvaluateModule(ASTNode module, Dictionary<string, double> currValues)
    {
        if (module.NodeType == "SymbolNode")
        {
            return [0];
        }
        if (module.NodeType != "Module") return [];
        if (module.ChildNodes.Count != 1) return [];
        var exprList = module.ChildNodes[0] as ExprListNode;
            
        List<double> moduleValues = [];

        foreach (var expr in exprList.ChildNodes)
        {
            moduleValues.Add(EvaluateExpression(expr, currValues));
        }

        return moduleValues;
    }
    
    public static bool EvaluateCondition(ASTNode condition, Dictionary<string, double> currValues)
    {
        if (condition.ChildNodes?.Count != 2) throw new Exception("Condition improperly formatted!");
        if (condition.ChildNodes[1] is not ConstantNode) throw new Exception("RHS must be a constant!");
        
        var lhs = EvaluateExpression(condition.ChildNodes[0], currValues);
        var rhs = (condition.ChildNodes[1] as ConstantNode).ConstValue;
        switch (condition.Value)
        {
            case "<":
                return lhs < rhs;
            case "<=":
                return lhs <= rhs;
            case ">":
                return lhs > rhs;
            case ">=":
                return lhs >= rhs;
            case "=":
                return Math.Abs(lhs - rhs) < 0.00001;
        }

        throw new Exception($"Invalid condition: {condition.Value}");
    }


    public static double EvaluateExpression(ASTNode expr, Dictionary<string, double> currValues)
    {
        var term = EvaluateTerm(expr.ChildNodes[0], currValues);
        if (expr.ChildNodes.Count == 2)
        {
            var rhsValue = EvaluateExpression(expr.ChildNodes[1], currValues);
            if (expr.Value == "+") return term + rhsValue;
            if (expr.Value == "-") return term - rhsValue;
            throw new Exception($"Undefined binary operator {expr.Value} found!");
        }

        return term;
    }
    
    public static double EvaluateTerm(ASTNode term, Dictionary<string, double> currValues)
    {
        var factor = EvaluateFactor(term.ChildNodes[0], currValues);
        if (term.ChildNodes.Count == 2)
        {
            var rhsValue = EvaluateTerm(term.ChildNodes[1], currValues);
            if (term.Value == "*") return factor * rhsValue;
            if (term.Value == "/") return factor / rhsValue;
            if (term.Value == "^") return Math.Pow(factor, rhsValue);
            throw new Exception($"Undefined binary operator {term.Value} found!");
        }

        return factor;
    }
    
    public static double EvaluateFactor(ASTNode factor, Dictionary<string, double> currValues)
    {
        var singleChild = factor.ChildNodes[0];
        if (singleChild is ConstantNode constNode) return constNode.ConstValue;

        if (singleChild is ParameterNode paramNode)
        {
            if (!currValues.ContainsKey(paramNode.Value))
                throw new Exception(
                    $"Parameter {paramNode.Value} is not a valid parameter found in predeccessor module!");
            return currValues[paramNode.Value];
        }

        if (singleChild.NodeType == "ExprNode") return EvaluateExpression(singleChild, currValues);

        return 0;
    }
    
}