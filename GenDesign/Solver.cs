using GenDesign.DataStructures;

namespace GenDesign;

public class Solver
{
    private int _temperature;
    // public int 
    private NodePath _path;
    private World _world;
    private Random _random;
    private Utils _utils;
    
    public Node StartNode { get; private set; }
    public Node EndNode { get; private set; }

    public Solver(Node startNode, Node endNode, int xMax, int yMax)
    {
        StartNode = startNode;
        EndNode = endNode;
        _utils = new Utils();
    }
    
    
    private double Fitness()
    {
        //TODO - create a turn counting (with angle) method to feed into Fitness()
        //score each path based on distance, number of turns, smoothness
        return _path.Length + (double)_path.Turns*2.1;
    }

    private NodePath CreateNeighborSolution(double displaceFactor = 0.02)
    {
        NodePath path = new NodePath();
        var nodeIndex = _random.Next(1, _path.Nodes.Count);
        var solutionDist = Utils.GetDistance(StartNode, EndNode);
        var perturbDist = solutionDist * displaceFactor; // try between 1% and 5%
        var directionX = _random.Next(3)-1; // random -1 0 1
        var directionY = _random.Next(3)-1; // random -1 0 1
        var newX = directionX*perturbDist;
        var newY = directionY*perturbDist;
        
        var oldNode = path.Nodes[nodeIndex];
        var newNode = new Node(oldNode.X + newX, oldNode.Y + newY, true);

        path = _path;
        path.Nodes[nodeIndex] = newNode;
        return path;
    }

    private NodePath InitialSolution()
    {
        NodePath path = new NodePath();
    }
    
    
    private NodePath Anneal()
    {
        _path = InitialSolution();
    }
    
}
