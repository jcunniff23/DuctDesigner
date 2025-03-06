using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GenDesign.DuctCreation;


namespace GenDesign.Optimize;

public class Anneal
{
    private double _temperature;
    private List<TakeoffDuct> _branches;
    private MainDuct _mainDuct;
    private DuctSegment _ductSegments;

    private Random _random;
    private double _coolingRate;
    private double _startingTemp;
    private double _tempMin;
    private int _maxMidPoints;

    public Anneal(double coolingRate, double startingTemp, double tempMin, int maxMidPoints)
    {
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

    public TakeoffDuct InitialSolution(DuctSegment segment, XYZ objectivePosition)
    {
        //1. determine takeoff point on segment
        //2. determine midpoints to terminal
        //2a. stop at distance X before terminal (flexduct)
        //3. construct object and return it

        XYZ takeoffPoint = segment.MainAxis.Evaluate(_random.NextDouble(), true);
        XYZ normal = segment.MainAxis.Direction.Normalize();
        XYZ zAxis = XYZ.BasisZ;

        XYZ orthogonalTakeoffDir = normal.CrossProduct(zAxis);
        if ((objectivePosition.DistanceTo(takeoffPoint + orthogonalTakeoffDir)) > (objectivePosition.DistanceTo(takeoffPoint - orthogonalTakeoffDir)))
            orthogonalTakeoffDir = orthogonalTakeoffDir * -1;

        XYZ takeoffEndPnt = takeoffPoint + orthogonalTakeoffDir * 1.5; // 1.5ft for duct starting takeoff, after that allow for turns
        List<XYZ> midPoints = new List<XYZ>();
        midPoints.Add(takeoffPoint);



        //random walk from start to endpoint (simple two step version)

        int stepCount = 4;
        int dir = _random.Next(1); // 0 = straight, 1 = turn towards terminal
        double angle = GetAngleFromPointsAndSegment(segment, takeoffEndPnt, objectivePosition);
        XYZ targetVectorNormal = (takeoffEndPnt - objectivePosition).Normalize();
        double targetVectDistance = (takeoffEndPnt - objectivePosition).GetLength();
        double flexDuctOffset = 2; // flex duct length of two feet from terminal


        XYZ currentPoint = takeoffEndPnt;
        midPoints.Add(takeoffEndPnt);

        //if (dir == 0)
        //{
        //    XYZ nextPoint = currentPoint + orthogonalTakeoffDir * targetVectDistance;
        //    midPoints.Add(nextPoint);
        //}
        //else
        //{
        //    XYZ nextPoint = currentPoint + normal * targetVectDistance;
        //    midPoints.Add(nextPoint);
        //}

        while (midPoints.Count < stepCount)
        {
            
            double currentTargetDist = (midPoints[^1] - objectivePosition).GetLength();
            
            if (currentTargetDist < flexDuctOffset)
            {
                break;
            }

            int dirToWalk = _random.Next(1); // 0 = cos, 1 = sin
            double distToWalk = (_random.Next(25,90)/100)*(midPoints[^1] - objectivePosition).GetLength();

            XYZ nextPoint;
            if (dirToWalk == 0)
            {
                nextPoint = midPoints[^1] + normal * distToWalk;
            }
            else
            {
                nextPoint = midPoints[^1] + orthogonalTakeoffDir * distToWalk;
            }

            midPoints.Add(nextPoint);
        }


        TakeoffDuct branch = new TakeoffDuct(takeoffPoint, objectivePosition, midPoints);
        return branch;
        
    }

    private TakeoffDuct CreateNeighborSolution(TakeoffDuct branch)
    {
        return branch;

    }


    private TakeoffDuct OptimizeTakeoff(DuctSegment segment, XYZ objectivePosition)
    {
        _temperature = _startingTemp;

        int newNodeThreshold = 50; // after perturbing current path X times, create new node to escape local min.
        int maxNodes = 4;
        int attemptsOnCurrentCount = 0;

        TakeoffDuct bestBranchFound;
        TakeoffDuct currentBranch = InitialSolution(segment, objectivePosition);
        TakeoffDuct newBranch;

        int loopCount = 1;

        while (_temperature > _tempMin)
        {

            newBranch = CreateNeighborSolution(currentBranch);
            var deltaE = 0;


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
        //optimize takeoffs globally with possible changes in pairing



    }



}
