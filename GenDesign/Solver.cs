using GenDesign.DataStructures;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace GenDesign;

public class Solver
{
    private double _temperature;
    private NodePath _path;
    //private World _world;
    private Random _random;
    private Utils _utils;
    private double _coolingRate;
    private double _startingTemp;
    private double _tempMin;
    
    public Node StartNode { get; private set; }
    public Node EndNode { get; private set; }

    public Solver(Node startNode, Node endNode, double coolingRate, double startingTemp, double tempMin)
    {
        StartNode = startNode;
        EndNode = endNode;
        _utils = new Utils();
        _coolingRate = coolingRate;
        _startingTemp = startingTemp;
        _tempMin = tempMin;

    }
    
    
    private double Fitness(NodePath path)
    {
        //TODO - create a turn counting (with angle) method to feed into Fitness()
        //score each path based on distance, number of turns, smoothness
        return path.Length + (double)path.Turns*2.1;
    }

    private NodePath CreateNeighborSolution(NodePath path, double displaceFactor = 0.02)
    {

        //ways to change the solution:
        /*
            1. move the waypoitns (along their angle/vector)
            2. add a new waypoint
            3. change the angle of waypoint (45deg -> 90deg)
         */
        
        
        var nodeIndex = _random.Next(1, path.Nodes.Count);
        
        var solutionDist = Utils.GetDistance(StartNode, EndNode);
        var perturbDist = solutionDist * displaceFactor; // try between 1% and 5%
        var directionX = _random.Next(3)-1; // random -1 0 1
        var directionY = _random.Next(3)-1; // random -1 0 1
        var newX = directionX*perturbDist;
        var newY = directionY*perturbDist;
        var newZ = 0; // TODO Change this later!
        
        var oldNode = path.Nodes[nodeIndex];
        var newNode = new Node(oldNode.X + newX, oldNode.Y + newY, oldNode.Z + newZ, true);


        //TODO - only allow for created solutions to be along 0deg, 45deg and 90deg vectors (0deg takeoffs as well)
        path.Nodes[nodeIndex] = newNode;
        return path;
    }

    private NodePath AddNodeToSolution()
    {

        // pick a random node and go one back so that a midpoint node can be created. once that node is created, move it around a little?
        var path = _path;
        var randomNodeIdx = _random.Next(_path.Nodes.Count);
        int nextNodeIdx = 0;
        int newNodeIdx = 1;
        if (randomNodeIdx == 0)
        {
            nextNodeIdx = 1;
            newNodeIdx = 1;
        }
        else
        {
            nextNodeIdx = randomNodeIdx - 1;
            newNodeIdx = randomNodeIdx;
        }

        var node1 = _path.Nodes[randomNodeIdx];
        var node2 = _path.Nodes[nextNodeIdx];
        XYZ midPoint = new XYZ((node1.X + node2.X) / 2, (node1.Y + node2.Y) / 2, (node1.Z + node2.Z) / 2);

        var newNode = NodeFactory.GetNode(midPoint);
        path.Nodes[newNodeIdx] = newNode;

        return path;
    }

    private NodePath InitialSolution()
    {
        //start with straight line solution and iterate with fitness function
        NodePath path = new NodePath();
        path.Nodes.Add(StartNode);
        // assume horizontal takeoff --> once placed the takeoff cannot move (tune this later.)
        path.Nodes.Add(new Node(StartNode.X * 1.1, StartNode.Y, StartNode.Z, true) { FixedPosition = true });

        //adding node after take off that is able to be moved
        path.Nodes.Add(new Node(StartNode.X * 1.25, StartNode.Y, StartNode.Z, true) { FixedPosition = false});
        path.Nodes.Add(EndNode);
        return path;
    }
    
    
    private NodePath Anneal()
    {
        _path = InitialSolution();
        _temperature = _startingTemp;
        double threshold = 0;

        int newNodeThreshold = 8; // after perturbing current path X times, create new node to escape local min.
        NodePath bestPathFound = _path;
        NodePath currentPath = _path;
        NodePath newPath = _path;
        int attemptsOnCurrentPath = 0;



        while (_temperature > _tempMin) 
        {
            // take path solution, determine fitness
            // iterate on solution, randomly
            newPath = CreateNeighborSolution(currentPath);
            attemptsOnCurrentPath++;
            var deltaE = Fitness(newPath) - Fitness(currentPath);


            if (deltaE < 0 || _random.Next() < Math.Exp(-deltaE / _temperature))
            {
                currentPath = newPath;
            }
            else
            {
                attemptsOnCurrentPath++;
            }

            _temperature *= _coolingRate;
        }

        return _path;
    }
    
}
