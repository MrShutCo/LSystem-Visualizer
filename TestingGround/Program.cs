// See https://aka.ms/new-console-template for more information

using LSystemVisualizer.Core;
using LSystemVisualizer.Core.Parser;
using ShutCo.UI.Core;
using ShutCo.UI.Core.Rules;

var condition1 = Parser.ParseCondition(new Queue<Token>(Tokenizer.Tokenize("y < 5")));
var condition2 = Parser.ParseCondition(new Queue<Token>(Tokenizer.Tokenize("x + y >= 5")));

// Expected output: True, False, False
Console.WriteLine(Evaluator.EvaluateCondition(condition1, new Dictionary<string, double> { { "y", 4 } }));
Console.WriteLine(Evaluator.EvaluateCondition(condition1, new Dictionary<string, double> { { "y", 5 } }));
Console.WriteLine(Evaluator.EvaluateCondition(condition1, new Dictionary<string, double> { { "y", 6 } }));

// Expected output: True, True, False
Console.WriteLine(Evaluator.EvaluateCondition(condition2, new Dictionary<string, double> { { "y", 4 }, { "x", 1 } }));
Console.WriteLine(Evaluator.EvaluateCondition(condition2, new Dictionary<string, double> { { "y", 6 }, { "x", 9 } }));
Console.WriteLine(Evaluator.EvaluateCondition(condition2, new Dictionary<string, double> { { "y", 0 }, { "x", 1 } }));

// Calculate actual values based on starting word
var startingWord1 = "B(2)A(4,4)";

//var evals = Evaluator.Evaluate(startingWordAST, []);
//Console.WriteLine(evals);

List<string> testParam =
[
    "A(x,y) : y <= 3 -> A(x*2, x+y)",
    "A(x,y): y > 3 -> B(x)A(x/y,0)",
    "B(x) : x < 1 -> C",
    "B(x) : x >= 1 -> B(x-1)"
];

var rules = testParam.Select(t => new ParametricRule(t)).ToList();

var lsystem = new ParametricLSystem(rules);
lsystem.StepLSystem([startingWord1], 10);