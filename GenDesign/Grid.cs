namespace GenDesign;

public class Grid
{
    // The class that will hold a set of nodes either in 2D or 3D space.
    // Stay generic but can be more implementation specific than Node.cs
    
    //2D text implementation:
    
    private Node[,] _grid;
    private int _gridSizeX, _gridSizeY;
    
    public Grid(int xSize, int ySize)
    {
        this._gridSizeX = xSize;
        this._gridSizeY = ySize;
        _grid = new Node[xSize, ySize];

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                bool walkable = true;
                _grid[i,j] = new Node(i, j, walkable);
            }
        }
        
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    
    
}