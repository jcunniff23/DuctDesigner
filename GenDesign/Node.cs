namespace GenDesign;

public class Node
{
    // Class that will handle the logic of an individual spot on the grid, world, 3D environment, etc.
    // Needs to be abstract enough to work with any context but still specific to hold fcost, gcost. etc.

    public bool Walkable;
    public int gCost;
    public int hCost;
    private int _gridX;
    private int _gridY;
    public Node Parent;
    public char DisplayChar = '0';

    public Node(int gridX, int gridY, bool walkable)
    {
        this.Walkable = walkable;
        this._gridX = gridX;
        this._gridY = gridY;
    }
    
    public int GCost { get { return gCost; } }
    public int HCost { get { return hCost; } }
    public int FCost { get { return gCost + hCost; } }
    public int GridX { get { return _gridX; } }
    public int GridY { get { return _gridY; } }
    
}