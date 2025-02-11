using System.Numerics;

namespace TerminalDesign.DataStructures;

public class Room
{
    List<Point> BoundaryPoints { get; set; }
    List<Vector2> BoundaryLines {get; set;}
    Point Centroid {get; set;}
    Point Center {get; set;}

    public Room(List<Point> boundaryPoints)
    {
        BoundaryPoints = boundaryPoints;
        
    }
}