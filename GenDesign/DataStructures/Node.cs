namespace GenDesign;

public class Node(double x, double y, bool walkable)
{
    public double X { get; init; } = x;
    public double Y { get; init; } = y;
    // public double Z { get; init; } = z;

    public bool Walkable { get; init; } = walkable; // whether there is an obstacle or not
}