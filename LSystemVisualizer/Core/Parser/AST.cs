using System.Runtime.CompilerServices;
using ShutCo.UI.Core.Rules;

namespace LSystemVisualizer.Core.Parser;

public class ASTNode
{
    public string NodeType { get; set; }
    public string Value { get; set; }
    public List<ASTNode>? ChildNodes { get; set; }
    
    public ASTNode(string nodeType, string value, List<ASTNode> childNodes)
    {
        NodeType = nodeType;
        Value = value;
        ChildNodes = childNodes;
    }

    public ASTNode()
    {
        
    }
}


public class ExprNode(ASTNode node) : ASTNode("ExprNode", "", [node]);
public class TermNode(ASTNode node) : ASTNode("TermNode", "", [node]);
public class FactorNode(ASTNode node) : ASTNode("FactorNode", "", [node]);

public class UnaryOp : ASTNode
{
    public UnaryOp(string op, ASTNode node) : base("UnaryOp", op, [node]){}
}

public class ParameterNode : ASTNode
{
    public ParameterNode(string value) : base("Parameter", value, null) {}
}

public class ConstantNode : ASTNode
{
    public double ConstValue;

    public ConstantNode(string value) : base("Constant", value, null)
    {
        ConstValue = double.Parse(value);
    }
}

public class ExprListNode : ASTNode
{
    public ExprListNode(List<ASTNode> childNodes) : base("ExprList", "", childNodes){}
}

public class BinaryOp : ASTNode
{
    public BinaryOp(ASTNode left, ASTNode right, string type, string op) : base(type, op, [left, right]){}
}

public class Parser()
{
    private static List<List<string>> operatorPrec =
    [
        ["^"],
        ["+", "-"],
        ["<", ">", "<=", ">="],
        ["==", "!="],
        ["&"],
        ["|"],
        ["&&"],
        ["||"],
    ];
    
    public static ASTNode? ParseModuleList(List<Token> tokens)
    {
        List<ASTNode> modules = [];
        Queue<Token> tokenQueue = new Queue<Token>(tokens);
        
        while (tokenQueue.Count > 0)
        {
            var module = ParseModule(tokenQueue);
            if (module == null) return null;
            modules.Add(module);
        }

        return new ASTNode("ModuleList", "", modules);
    }

    public static ASTNode? ParseModule(Queue<Token> tokenQueue)
    {
        if (tokenQueue.Count == 0) return null;
        var tok = tokenQueue.Dequeue();
        if (tok.Type != TokenType.Symbol) return null;

        if (tokenQueue.Count == 0) return new ASTNode("Module", tok.Value, []); 
        
        var tokB = tokenQueue.Peek();
        // We have a standalone letter, continue on
        if (tokB.Type == TokenType.Symbol) return new ASTNode("Module", tok.Value, []);
        tokB = tokenQueue.Dequeue();
        if (tokB.Value != "(") return null;

        var node = ParseExprList(tokenQueue);
        if (node == null) return null;
            
        var tokRB = tokenQueue.Dequeue();
        if (tokRB.Value != ")") return null;
        
        return new ASTNode("Module", tok.Value, [node]);
    }

    static ASTNode? ParseConditionList(Queue<Token> tokenQueue)
    {
        var condition = ParseCondition(tokenQueue);
        if (condition == null) return null;

        if (tokenQueue.Count == 0) return condition;
        var tok = tokenQueue.Peek();
        if (tok.Value is not ("&" or "|")) return condition;

        var logicalOpToken = tokenQueue.Dequeue();
        var restOf = ParseConditionList(tokenQueue);

        return new ASTNode("LogicalOpNode", logicalOpToken.Value, [condition, restOf]);
    }

    public static ASTNode? ParseCondition(Queue<Token> tokenQueue)
    {
        var expr = ParseExpr(tokenQueue);
        if (expr == null) return null;

        var tok = tokenQueue.Dequeue();
        if (tok.Type != TokenType.Conditional) return null;

        var tokRhs = tokenQueue.Dequeue();
        if (tokRhs.Type != TokenType.Constant) return null;

        return new ASTNode("ConditionNode", tok.Value, [expr, new ConstantNode(tokRhs.Value)]);
    }

    static ExprListNode? ParseExprList(Queue<Token> tokenQueue)
    {
        List<ASTNode> nodes = [];
        while (tokenQueue.Count > 0)
        {
            var node = ParseExpr(tokenQueue);
            if (node == null) return null;
            nodes.Add(node);

            var tok = tokenQueue.Peek();
            if (tok.Type != TokenType.Comma) break;
            
            // There's more expressions to be evaluated
            tokenQueue.Dequeue();
        }

        return new ExprListNode(nodes);
    }

    static ASTNode? ParseExpr(Queue<Token> tokenQueue)
    {
        var term = ParseTerm(tokenQueue);
        if (term == null) return null;

        var tok = tokenQueue.Peek();
        if (tok.Value is not ("+" or "-")) return new ExprNode(term);

        tokenQueue.Dequeue();
        var rightOp = ParseExpr(tokenQueue);
        if (rightOp == null) return null;
        return new ASTNode("ExprNode", tok.Value, [term, rightOp]);
    }

    static ASTNode? ParseTerm(Queue<Token> tokenQueue)
    {
        var factor = ParseFactor(tokenQueue);
        if (factor == null) return null;

        var tok = tokenQueue.Peek();
        if (tok.Value is not ("*" or "/" or "^")) return new TermNode(factor);

        tokenQueue.Dequeue();
        var rightOp = ParseTerm(tokenQueue);
        if (rightOp == null) return null;
        return new ASTNode("TermNode", tok.Value, [factor, rightOp]);
    }


    public static ASTNode ParsePrecedence(List<Token> tokens)
    {
        return ParsePrecedence(new Queue<Token>(tokens), 0);
    }
    
    public static ASTNode ParsePrecedence(Queue<Token> tokenQueue, int level)
    {
        if (tokenQueue.Count == 0) return null;
        var left = level == operatorPrec.Count-1 ? ParsePrimary(tokenQueue) : ParsePrecedence(tokenQueue, level + 1);

        if (tokenQueue.Count == 0) return left;
        var tok = tokenQueue.Peek();
        if (operatorPrec[level].Contains(tok.Value)) return left;
        if (tok.Value == ")") return left;
        
        tokenQueue.Dequeue();
        var right = ParsePrecedence(tokenQueue, level);
        return new ASTNode($"PrecedenceOp {level}", tok.Value, [left, right]);
    }

    static ASTNode? ParsePrimary(Queue<Token> tokenQueue)
    {
        var tok = tokenQueue.Dequeue();

        if (tok.Value == "(")
        {
            var expr = ParsePrecedence(tokenQueue, operatorPrec.Count+1);
            tok = tokenQueue.Dequeue();
            if (tok.Value != ")") return null;
            return new ASTNode("Primary", "()", [expr]);
        }
        if (tok.Type == TokenType.Constant)
        {
            return new ConstantNode(tok.Value);
        }
        if (tok.Type == TokenType.Parameter)
        {
            return new ParameterNode(tok.Value);
        }

        // if (tok.Value == "-")
        // {
        //     var factor = ParseFactor(tokenQueue);
        //     return new UnaryOp(tok.Value, factor);
        // }

        return null;
    }
    
    static FactorNode? ParseFactor(Queue<Token> tokenQueue)
    {
        var tok = tokenQueue.Dequeue();

        if (tok.Value == "(")
        {
            var expr = ParseExpr(tokenQueue);
            tok = tokenQueue.Dequeue();
            if (tok.Value != ")") return null;
            return new FactorNode(expr);
        }
        if (tok.Type == TokenType.Constant)
        {
            return new FactorNode(new ConstantNode(tok.Value));
        }
        if (tok.Type == TokenType.Parameter)
        {
            return new FactorNode(new ParameterNode(tok.Value));
        }

        // if (tok.Value == "-")
        // {
        //     var factor = ParseFactor(tokenQueue);
        //     return new UnaryOp(tok.Value, factor);
        // }

        return null;
    }
}