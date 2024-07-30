namespace ShutCo.UI.Core.Rules;

public class AST
{
    
}

public class ASTNode
{
    public string NodeType { get; set; }
    public string Value { get; set; }
    public List<ASTNode> ChildNodes { get; set; }
    
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
    public int ConstValue;

    public ConstantNode(string value) : base("Constant", value, null)
    {
        ConstValue = int.Parse(value);
    }
}

public class ExprListNode : ASTNode
{
    public ExprListNode(List<ASTNode> childNodes) : base("ExprList", "", childNodes){}
}

public class BinaryOp : ASTNode
{
    public BinaryOp(ASTNode left, ASTNode right, string op) : base("BinaryOp", op, [left, right]){}
}

public class Parser()
{
    
    public static ASTNode? ParseSuccessor(List<Token> tokens)
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
        var tok = tokenQueue.Dequeue();
        if (tok.Type != TokenType.Symbol) return null;

        var tokB = tokenQueue.Dequeue();
        if (tokB.Value != "(") return null;

        var node = ParseExprList(tokenQueue);
        if (node == null) return null;
            
        var tokRB = tokenQueue.Dequeue();
        if (tokRB.Value != ")") return null;
        
        return new ASTNode("Module", tok.Value, [node]);
    }

    public static ASTNode? ParseCondition(Queue<Token> tokenQueue)
    {
        var expr = ParseExpr(tokenQueue);
        if (expr == null) return null;

        var tok = tokenQueue.Dequeue();
        if (tok.Type != TokenType.Conditional) return null;

        var tokRhs = tokenQueue.Dequeue();
        if (tokRhs.Type != TokenType.Constant) return null;

        return new BinaryOp(expr, new ConstantNode(tokRhs.Value), tok.Value);
    }

    static ASTNode? ParseExprList(Queue<Token> tokenQueue)
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
        if (tok.Value is not ("+" or "-")) return term;

        tokenQueue.Dequeue();
        var rightOp = ParseExpr(tokenQueue);
        if (rightOp == null) return null;
        return new BinaryOp(term, rightOp, tok.Value);
    }

    static ASTNode? ParseTerm(Queue<Token> tokenQueue)
    {
        var factor = ParseFactor(tokenQueue);
        if (factor == null) return null;

        var tok = tokenQueue.Peek();
        if (tok.Value is not ("*" or "/")) return factor;

        tokenQueue.Dequeue();
        var rightOp = ParseTerm(tokenQueue);
        if (rightOp == null) return null;
        return new BinaryOp(factor, rightOp, tok.Value);
    }
    
    static ASTNode? ParseFactor(Queue<Token> tokenQueue)
    {
        var tok = tokenQueue.Dequeue();

        if (tok.Value == "(")
        {
            var expr = ParseExpr(tokenQueue);
            tok = tokenQueue.Dequeue();
            if (tok.Value != ")") return null;
            return expr;
        }
        if (tok.Type == TokenType.Constant)
        {
            return new ConstantNode(tok.Value);
        }
        if (tok.Type == TokenType.Parameter)
        {
            return new ParameterNode(tok.Value);
        }

        if (tok.Value == "-")
        {
            var factor = ParseFactor(tokenQueue);
            return new UnaryOp(tok.Value, factor);
        }

        return null;
    }
}