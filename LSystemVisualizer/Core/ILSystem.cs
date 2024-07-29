using TurtleGraphics.BlazorCanvas;

namespace ShutCo.UI.Core;

public interface ILSystem
{
    public List<string> StepLSystem(List<string> word, int iterations);
    public Task StepTurtle(string letter, Turtle turtle, int iterations);
    
    public List<string> Ignore { get; set; }
}