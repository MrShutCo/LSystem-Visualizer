// See https://aka.ms/new-console-template for more information

using ShutCo.UI.Core.Rules;

var predecessor = "A(t)";
var condition = "t > 5";
var successor = "B((t+1)/(s))CD(t*5,t-2)";

var parseTest1 = "B(t)";

List<string> testParam =
[
    "A(x,y) : y <= 3 -> A(x*2, x+y)",
    "A(x,y): y > 3 -> B(x)A(x/y,0)",
    "B(x) : x < 1 -> C",
    "B(x) : x >= 1 -> B(x-1)"
];

var tokens = Tokenizer.Tokenize(successor);
foreach (var token in tokens)
{
    Console.WriteLine(token);
}

var moduleList = Parser.ParseSuccessor(tokens);
Console.WriteLine(moduleList);

var a = new ParametricRule(testParam[0]);