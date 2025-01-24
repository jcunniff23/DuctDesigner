namespace GenDesign;

public class Pathfinder
{
    //  Individual class for holding a modified A* routing, this will likely only be accessed by the Solver
    private Grid _grid;
    public Pathfinder(Grid grid)
    {
        this._grid = grid;
    }

    public List<Node> AStar(Node start, Node goal)
    {
        List<Node> openSet  = new List<Node>();
        List<Node> closedSet = new List<Node>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost <= node.FCost && openSet[i].hCost < node.hCost)
                {
                    node = openSet[i];
                }
            }
        
            openSet.Remove(node);
            closedSet.Add(node);

            if (node == goal)
            {
                var path = RetracePath(start, goal);
                return path;
            }

            foreach (Node neighbor in _grid.GetNeighbours(node))
            {
                if (!neighbor.Walkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = node.gCost + GetDistance(node, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, goal);
                    neighbor.Parent = node;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
            
        }
        
        Console.WriteLine("ESCAPED WHILE LOOP SOMEHOW!?!");
        return new List<Node>();

    }

    private List<Node> RetracePath(Node start, Node goal)
    {
        List<Node> path = new List<Node>();
        Node currentNode = goal;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    private int GetDistance(Node current, Node goal)
    {
        return Math.Abs(current.GridX - goal.GridX) + Math.Abs(current.GridY - goal.GridY);
    }
    
    
}