using System.Numerics;

namespace GenDesign.DataStructures;

public class DuctSegment
{
    // DuctSegments will represent the individual runs of duct that will sit between elbows and fittings. Each segment
    // can only have two points. The duct is assumed to be ran in a straight line between those ducts.
    // Duct properties: size, type, family, etc can all be set here.
    
    // public List<Point> Points { get; set; }
    
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public Point CenterPoint => new Point(StartPoint.X + EndPoint.X, StartPoint.Y + EndPoint.Y);
    public double? Diameter { get; set; } //TODO figure out a way to check that Diameter OR Width/Height are set exclusively
    public double? Width { get; set; }
    public double? Height { get; set; }
    public Vector2 Vector { get; init; }
    public double Length => Vector.Length();
    
    
    public DuctSegment(Point start, Point end)
    {
        // Points = new List<Point> { start, end };
        StartPoint = start;
        EndPoint = end;
        Vector = new Vector2((float)(end.X - start.X), (float)(end.Y - start.Y));
    } 
    
        
}