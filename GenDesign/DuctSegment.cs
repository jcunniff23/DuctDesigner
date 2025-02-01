using System.Numerics;

namespace GenDesign;

public class DuctSegment
{
    // DuctSegments will represent the individual runs of duct that will sit between elbows and fittings. Each segment
    // can only have two points. The duct is assumed to be ran in a straight line between those ducts.
    // Duct properties: size, type, family, etc can all be set here.
    
    public List<Point> Points { get; set; } // TODO figure out a way to strictly check if Points count > 2 and prevent overfill
    
    public double? Diameter { get; set; } //TODO figure out a way to check that Diameter OR Width/Height are set exclusively
    public double? Width { get; set; }
    public double? Height { get; set; }
    public Vector2 Vector { get; init; }
    
    public DuctSegment(Point start, Point end)
    {
        Points = new List<Point> { start, end };
        Vector = new Vector2((float)(end.X - start.X), (float)(end.Y - start.Y));
    } 
    
        
}