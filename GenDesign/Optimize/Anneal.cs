using GenDesign.DataStructures;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GenDesign.DuctCreation;


namespace GenDesign.Solver;

public class Anneal
{
    private double _temperature;
    private List<TakeoffDuct> _branches;
    private MainDuct _mainDuct;
    private DuctSegment _ductSegments;

    private Random _random;
    private Utils _utils;
    private double _coolingRate;
    private double _startingTemp;
    private double _tempMin;
    private int _maxMidPoints;

    public Node StartNode { get; private set; }
    public Node EndNode { get; private set; }

    public Anneal(Node startNode, Node endNode, double coolingRate, double startingTemp, double tempMin, int maxMidPoints)
    {
        StartNode = startNode;
        EndNode = endNode;
        _utils = new Utils();
        _coolingRate = coolingRate;
        _startingTemp = startingTemp;
        _tempMin = tempMin;
        _random = new Random();
        _maxMidPoints = maxMidPoints;

    }


    private double Fitness(List<TakeoffDuct> ducts)
    {
        //TODO - create a turn counting (with angle) method to feed into Fitness()
        //score each path based on distance, number of turns, smoothness
        return 0.0;
    }

    private NodePath CreateNeighborSolution(NodePath path, double displaceFactor = 0.01)
    { 

        var nodeIndex = _random.Next(1, path.Nodes.Count - 1);

        while (path.Nodes[nodeIndex].FixedPosition == true)
            nodeIndex = _random.Next(1, path.Nodes.Count - 1);

        var solutionDist = Utils.GetDistance(StartNode, EndNode);
        var perturbDist = solutionDist * displaceFactor; // try between 1% and 5%
        var directionX = _random.Next(3) - 1; // random -1 0 1
        var directionY = _random.Next(3) - 1; // random -1 0 1
        var newX = directionX * perturbDist;
        var newY = directionY * perturbDist;
        var newZ = 0; // TODO Change this later!

        var oldNode = path.Nodes[nodeIndex];
        var newNode = new Node(oldNode.X + newX, oldNode.Y + newY, oldNode.Z + newZ, true);


        //TODO - only allow for created solutions to be along 0deg, 45deg and 90deg vectors (0deg takeoffs as well)
        path.Nodes[nodeIndex] = newNode;
        return path;
    }

    private double GetAngleFromPointsAndSegment(DuctSegment segment, XYZ pointA,  XYZ pointB)
    {
        XYZ targetVector = (pointB - pointA).Normalize();
        XYZ originalVector = segment.MainAxis.Direction.Normalize();
        double dotProduct = originalVector.DotProduct(targetVector);
        dotProduct = Math.Max(-1, Math.Min(1, dotProduct));

        double angleRads = Math.Acos(dotProduct);
        double angleDegs = angleRads * (180/ Math.PI);


        return angleDegs;
    }

    private TakeoffDuct InitialSolution(DuctSegment segment, XYZ objectivePosition)
    {
        //1. determine takeoff point on segment
        //2. determine midpoints to terminal
        //3. construct object and return it

        //double xPos = _random.NextDouble()*(segment.End.X - segment.Start.X) + segment.Start.X;
        //double yPos = _random.NextDouble() * (segment.End.Y - segment.Start.Y) + segment.Start.Y;
        //double zPos = segment.Start.Z;
        //XYZ takeoffPoint = new XYZ(xPos, yPos, zPos);

        XYZ takeoffPoint = segment.MainAxis.Evaluate(_random.NextDouble(), true);
        XYZ normal = segment.MainAxis.Direction.Normalize();
        XYZ zAxis = XYZ.BasisZ;

        XYZ orthogonalTakeoffDir = normal.CrossProduct(zAxis);
        if ((objectivePosition.DistanceTo(takeoffPoint + orthogonalTakeoffDir)) > (objectivePosition.DistanceTo(takeoffPoint - orthogonalTakeoffDir)))
            orthogonalTakeoffDir = orthogonalTakeoffDir * -1;

        XYZ takeoffEndPnt = takeoffPoint + orthogonalTakeoffDir * 1.5; // 1.5ft for duct starting takeoff, after that allow for turns
        List<XYZ> midPoints = new List<XYZ>();
        midPoints.Add(takeoffPoint);


        XYZ currentPoint = takeoffEndPnt;


        //random walk from start to endpoint (simple two step version)

        int dir = _random.Next(1); // 0 = straight, 1 = turn towards terminal
        double angle = GetAngleFromPointsAndSegment(segment, takeoffEndPnt, objectivePosition);
        XYZ targetVectorNormal = (takeoffEndPnt - objectivePosition).Normalize();
        double targetVectDistance = (takeoffEndPnt - objectivePosition).GetLength();


        if (dir == 0)
        {
            XYZ nextPoint = currentPoint + orthogonalTakeoffDir * targetVectDistance;
            midPoints.Add(nextPoint);
        }
        else
        {
            XYZ nextPoint = currentPoint + normal * targetVectDistance;
            midPoints.Add(nextPoint);
        }


        TakeoffDuct branch = new TakeoffDuct(takeoffPoint, objectivePosition, midPoints);
        return branch;
        
    }


    private TakeoffDuct OptimizeTakeoff(DuctSegment segment, XYZ objectivePosition)
    {
        _temperature = _startingTemp;

        int newNodeThreshold = 50; // after perturbing current path X times, create new node to escape local min.
        int maxNodes = 4;
        int attemptsOnCurrentCount = 0;

        NodePath bestBranchFound;
        NodePath currentBranch;
        NodePath newBranch;

        int loopCount = 1;

        while (_temperature > _tempMin)
        {

            Console.WriteLine($"Attempt: {loopCount}, Fitness: {Fitness(_path)}");

            loopCount++;
            // take path solution, determine fitness
            // iterate on solution, randomly


            newBranch = CreateNeighborSolution(currentBranch);
            var deltaE = Fitness(newBranch) - Fitness(currentBranch);


            if (deltaE < 0 || _random.Next() < Math.Exp(-deltaE / _temperature))
            {
                currentBranch = newBranch;
            }
            else
            {
                attemptsOnCurrentCount++;
            }




            _temperature *= _coolingRate;
        }

        return currentBranch;
    }

    private List<XYZ> GenerateTakeoff(DuctSegment segment, XYZ terminalPosition)
    {
        XYZ direction;

        //if terminal is within short axis bounds, create a straight run and set StraightRun to true

        return new List<XYZ> { terminalPosition };
    }




    public void Run()
    {
        //for each segment to terminal pairing -> OptimizeTakeoff();
    }



}
