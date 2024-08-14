// // See https://aka.ms/new-console-template for more information
//
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

/*List<string> testParam =
[
    "A(x,y) : y <= 3 -> A(x*2, x+y)",
    "A(x,y): y > 3 -> B(x)A(x/y,0)",
    "B(x) : x < 1 -> C",
    "B(x) : x >= 1 -> B(x-1)"
];

var rules = testParam.Select(t => new ParametricRule(t)).ToList();

var lsystem = new ParametricLSystem(rules);
lsystem.StepLSystem([startingWord1], 10);*/


var system2 = new ParametricLSystem("A(1)", [
    new ParametricRule("A(s) : -> F(s)[+A(s/r)][-A(s/r)]")
], new Dictionary<string, string>
{
    { "r", "1.456" }
});

system2.StepLSystem(["A(1)"], 1);

var system = new ParametricLSystem("F(1)", [
    new ParametricRule("F(x) : -> F(x*p)+F(x*h)--F(x*h)+F(x*q)")
], new Dictionary<string, string>
{
    { "c", "1" },
    { "p", "0.3" },
    { "q", "c-p" },
    { "h", "(p*q)^0.5" },
});

//system.StepLSystem(["F(1)"], 5);

var system3 = new ParametricLSystem("A(1,10)", [
    new ParametricRule("A(l,w) : -> !(w)F(l)[&(a)B(l*t,w*x)]/(d)A(l*r,w*x)"),
    new ParametricRule("B(l,w) : -> !(w)F(l)[-(b)$C(l*t,w*x)]C(l*r,w*x)"),
    new ParametricRule("C(l,w) : -> !(w)F(l)[+(b)$B(l*t,w*x)]B(l*r,w*x)")
    
], new Dictionary<string, string>
{
    { "r", "0.9" },
    { "t", "0.6" },
    { "a", "45" },
    { "b", "45" },
    { "d", "137.5" },
    { "x", "0.707" }
});


//system2.StepLSystem(["A(1,10)"], 1);

/*
var s = "F(1,0)";
List<string> rulesS =
[
    "F(x,t) : t = 0 -> F(x*p,2)+F(x*h),1--F(x*h,1)+F(x*q,0)",
    "F(x,t) : t > 0 -> F(x,t-1)"
];*/
//
// using TestingGround;
//
// SinglesSolver.Singles(new [,]
// {
//     {1,1,1,4,2},
//     {1,2,4,5,2},
//     {3,5,3,1,4},
//     {1,3,5,2,1},
//     {2,4,1,5,5}
// });
//
// SinglesSolver.Singles(new [,]
// {
//     {2,6,5,3,4,2},
//     {6,5,1,3,5,3},
//     {1,5,1,6,2,5},
//     {3,6,4,5,6,5},
//     {4,3,6,5,4,2},
//     {2,1,2,6,3,6}
// });
//
// SinglesSolver.Singles(new [,]
// {
//     {3,2,11,1,6,4,7,10,7,9,5,4},
//     {6,1,3,9,7,6,4,3,4,1,7,7},
//     {12,12,9,10,11,4,1,2,4,5,8,7},
//     {9,4,6,9,2,12,10,12,5,7,3,6},
//     {9,12,6,2,12,9,6,2,1,9,4,8},
//     {1,9,12,3,4,6,9,8,1,6,3,11},
//     {10,3,12,4,4,5,12,9,9,11,6,10},
//     {2,8,5,7,3,5,2,9,6,12,12,10},
//     {6,1,8,2,9,12,5,12,2,1,10,5},
//     {2,9,9,5,12,2,8,1,7,10,10,6},
//     {7,3,3,1,1,2,4,5,11,11,9,5},
//     {4,11,2,6,7,10,8,5,8,12,1,4}
// });