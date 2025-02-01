using System.Numerics;
using System.Security.Cryptography;

namespace GenDesign;

public class GeneticEvolution
{
    private int _generations = 0;
    private int _maximumGenerations;
    private readonly Point startPoint;
    private readonly Point endPoint;
    private int _maximumTurns;
    private int _initialPopulation;

    public enum RouteMode
    {
        OneToOne,
        OneToMany,
        ManyToMany
    }

    // Genetic Algorithm for solving duct routing
    // starting with a 1-to-1 case
    public GeneticEvolution(Point start, Point objective, int initialPopulation, int maximumGenerations, RouteMode mode,
        int maximumTurns)
    {
        _maximumGenerations = maximumGenerations;
        startPoint = start;
        endPoint = objective;
        _maximumTurns = maximumTurns;
        _initialPopulation = initialPopulation;
    }

    private double FitnessPressureDrop(DuctPath path)
    {
        // GOAL: MINIMIZE Pdrop
        double C0_duct = 0.15; // per unit lenght
        double C0_45degFitting = 0.75; // per instance
        double C0_90degFitting = 1.25; // per instance
        double totalLength = 0.0;

        int fittings45deg = 0;
        int fittings90deg = 0;

        // find instance of each turn & find all lengths
        for (int i = 0; i < path.DuctSegments.Count; i++)
        {
            DuctSegment thisSegment = path.DuctSegments[i];

            // if there are no nextSegments, break for loop
            if (i == path.DuctSegments.Count - 1)
            {
                break;
            }

            DuctSegment nextSegment = path.DuctSegments[i + 1];
            totalLength += (double)thisSegment.Vector.Length();

            var theta = Math.Acos(Vector2.Dot(thisSegment.Vector, nextSegment.Vector) /
                                  (thisSegment.Vector.Length() * nextSegment.Vector.Length()));
            var anglesPossible = new List<double> { 0, Math.PI / 4, Math.PI / 2 };
            if (theta == Math.PI / 4)
            {
                fittings45deg++;
            }
            else if (theta == Math.PI / 2)
            {
                fittings90deg++;
            }
            else if (theta == 0)
            {
                fittings90deg++;
            }
            else
            {
                Console.Write($"ERROR ON FITNESS PRESSURE DROP: theta = {theta}");
            }
        }

        return (C0_duct * totalLength + C0_45degFitting * fittings45deg + C0_90degFitting * fittings90deg);
    }

    private double FitnessFootprint(DuctPath path)
    {
        // GOAL: MINIMIZE Footprint/Area
        return 0.0;
    }

    private double FitnessCost(DuctPath path)
    {
        // GOAL: MINIMIZE Cost (Length/fittings)
        return 0.0;
    }

    private List<DuctPath> EqualizeGenomes(List<DuctPath> paths)
    {
        DuctPath longestPath = paths[0];
        foreach (DuctPath path in paths)
        {
            if (path.DuctSegments.Count > longestPath.DuctSegments.Count)
            {
                longestPath = path;
            }
        }

        paths.Remove(longestPath);

        foreach (DuctPath path in paths)
        {
            // Randomly? segment path until path.DuctSegments.count == longest

            while (path.DuctSegments.Count < longestPath.DuctSegments.Count)
            {
                // split random gene until satisifed       
            }
        }

        return paths;
    }

    private List<DuctPath> SingleCrossover(DuctPath parent1, DuctPath parent2)
    {
        Random random = new Random();
        int parent1Index = random.Next(parent1.DuctSegments.Count);

        return new List<DuctPath>(); //temp for error 
    }

    private List<DuctPath> DoubleCrossover(DuctPath parent1, DuctPath parent2)
    {
        Random random = new Random();
        
        return new List<DuctPath>(); //temp for error 
    }

    private void Mutate()
    {
        /*  Mutations that are valid:
         *      deletion,
         *      insertion (how to choose new vector?)
         *      substitutions (swapping),
         *
         */
    }

    private bool ValidatePath(DuctPath path)
    {
        /*  Test whether the path:
         *      is continuous from start point to target
         *      has valid turns
         *      does not collide with obstacles?
         *
         *  validate path will not do scoring, only check for issues that would
         *  ruin future generations
         */

        bool checkPath = false;

        foreach (DuctSegment segment in path.DuctSegments)
        {
        }


        return checkPath;
    }

    private void RegeneratePath(DuctPath path)
    {
        /*  To regenerate the path (after a broken mutation)
         *  the idea will be to take the remaining vector from
         *  the path's endpoint to the objective point
         *  then create a new vector that will follow the rules of
         *  the solver overall
         *
         *  OR - take the difference vector and disperse it across other genes
         *  in the solution genome. This would cause issues with gene diversity and moving
         *  the solution space around though.
         */

        /*
         * OR, ignore the incomplete paths for now and add a penalty function that will
         * act as a way to guide the solver to connect back the
         *
         */

        DuctSegment newSegment = new DuctSegment(path.DuctSegments.Last().Points[1], endPoint);
        path.DuctSegments.Add(newSegment);
    }

    private double IncompletePathPenalty(DuctPath path)
    {
        int penaltyMultiplier = 10; // TODO tune this parameter for broken paths
        DuctSegment? finalSegment;
        if (path.DuctSegments.Count == 0)
            return penaltyMultiplier * GetDistance(startPoint, endPoint);
        else
            finalSegment = path.DuctSegments.Last();

        return penaltyMultiplier * GetDistance(finalSegment.Points[1], endPoint);
    }

    public DuctPath InitializeRandomRoute(Point start, Point end)
    {
        /*  TODO: random route needs to have decision making for how far each vector should travel ()
         *        initial random routes should be spread across the solution space (2d or 3d) so that the likelihood of being
         *        near the optimal solution is greater
         */
        Random random = new Random();
        List<DuctSegment> segments = new List<DuctSegment>();
        int randomTurnCount = random.Next(1, _maximumTurns + 1); // random # of turns between 1 and max
        int turnCounter = 0;
        double fullDist = GetDistance(start, end);

        // Loop through and create new duct segments until condition completed
        while (turnCounter < randomTurnCount)
        {
            turnCounter++;
            Point mostRecentPoint;
            if (segments.Count != 0)
                mostRecentPoint = segments.Last().Points[1];
            else
                mostRecentPoint = start;

            if (GetDistance(mostRecentPoint, end) < fullDist * 0.05)
            {
                // If the most recent segment is within 5% of the total distance to the target, close the path
                mostRecentPoint = new Point(end.X, end.Y);
                break;
            }

            if (turnCounter == randomTurnCount)
            {
                // On final segment, reach the objective to complete path
                DuctSegment finalSegment = new DuctSegment(mostRecentPoint, end);
                segments.Add(finalSegment);
                break;
            }


            DuctSegment nextSegment = new DuctSegment(mostRecentPoint, RandomNextPoint(mostRecentPoint, end));
            segments.Add(nextSegment);
        }

        DuctPath path = new DuctPath(segments);
        return path;
    }

    private void Evolution()
    {
        /*
         *
         *
         *
         *
         */

        // TODO Create better stop conditions
        while (_generations < _maximumGenerations)
        {
            List<DuctPath>
                currentPopulationSet =
                    new List<DuctPath>(); //  newest evolution set, populate based on parents, or initial route on first gen

            if (_generations == 0)
            {
                // populate using InitializeRandomRoute()
                InitializeRandomRoute(startPoint, endPoint);
            }


            _generations++;
        }
    }


    private int RandomAngle()
    {
        // Will randomly return an angle: -90, -45, 0, 45, 90

        Random rand = new Random();
        int angle;
        var sign = 1;
        int random1 = rand.Next(2);
        int random2 = rand.Next(1);

        if (random1 == 0)
            angle = 45;
        else if (random1 == 1)
            angle = 90;
        else
            return 0;

        if (random2 == 0)
            sign = 1;
        else
            sign = -1;

        return sign * angle;
    }

    private Point RandomNextPoint(Point currentPosition, Point targetPosition)
    {
        // Will return a new point to construct a new segment that will be somewhere between current location and target location
        Random rand = new Random();

        int randInt = rand.Next(101); // return random value between 0-100
        double randDistance = (double)randInt / 100;
        //ensures that this function will next overshoot the objective point but will never really reach it.

        int randAngle = RandomAngle();
        double distance = GetDistance(currentPosition, targetPosition);

        if (currentPosition == startPoint)
        {
            randAngle = 0;
        }

        if (currentPosition == targetPosition)
        {
            return currentPosition;
        }

        double newX = currentPosition.X + (distance * randDistance * Math.Cos(randAngle * Math.PI / 180));
        double newY = currentPosition.Y + (distance * randDistance * Math.Sin(randAngle * Math.PI / 180));
        Point nextPoint = new Point(newX, newY);

        return nextPoint;
    }

    private double GetDistance(Point pointA, Point pointB)
    {
        return Math.Sqrt((pointA.X - pointB.X) * (pointA.X - pointB.X) + (pointA.Y - pointB.Y) * (pointA.Y - pointB.Y));
    }
}