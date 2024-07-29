using System.Numerics;
using Blazor.Extensions.Canvas.Canvas2D;

namespace ShutCo.UI.Core;

public class LSystemDrawer
{
    private Canvas2DContext _context;
    
    public double X { get; set; }
    public double Y { get; set; }
    public double Angle { get; set; }

    public LSystemDrawer(Canvas2DContext context)
    {
        _context = context;
    }

    public async Task Forward(int amount, bool draw)
    {
        var deg = Angle * Math.PI / 180f;
        (X, Y) = (X + amount * Math.Cos(deg), Y + amount * Math.Sin(deg));
        if (draw)
        {
            await _context.LineToAsync(X, Y);
        }
        else
        {
            await _context.MoveToAsync(X,Y);
        }
        
        await _context.StrokeAsync();
    }
}