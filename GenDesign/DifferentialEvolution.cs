using System.Numerics;

namespace GenDesign;

public class DifferentialEvolution 
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

    public DifferentialEvolution(Point start, Point objective, int initialPopulation, int maximumGenerations, RouteMode mode,
        int maximumTurns)
    {
        _maximumGenerations = maximumGenerations;
        startPoint = start;
        endPoint = objective;
        _maximumTurns = maximumTurns;
        _initialPopulation = initialPopulation;
    }
    
    public void Evolution(double crossoverProbability = 0.9, double differentialWeight = 0.8)
    {
        /*  Implementing based on:
         * https://optimization.cbe.cornell.edu/index.php?title=Differential_evolution#Initialization
         * Swagatam Das; Sankha Subhra Mullick; P.N. Suganthan (2016). Recent advances in differential evolution
         *
         *
         */

        int populationSize = _initialPopulation;

        //Initialize routes based on desired size from solver constructor
        List<DuctPath> paths = Enumerable.Range(0, populationSize)
            .Select(_ => InitializeRandomRoute(startPoint, endPoint)).ToList();

        int generationCount = 0;
        while (generationCount < _maximumGenerations)
        {
            foreach (DuctPath individual in paths)
            {
                foreach (DuctSegment segment in individual.DuctSegments)
                {
                    var mutantVector = MutantVector(differentialWeight, paths);
                    var trialVector = DifferentialCrossover(mutantVector, segment.Vector, crossoverProbability);
                    
                }
            }
        }
    }

    private Vector2 MutantVector(double differentialWeight, List<DuctPath> paths)
    {
        //choose 3 random segment vectors, 
        //combine segment vectors and return mutated segment

        List<DuctSegment> segments = new List<DuctSegment>();

        foreach (DuctPath path in paths)
        {
            segments.AddRange(path.DuctSegments);
        }

        Random random = new Random();
        var vectors = segments.OrderBy(x => random.Next()).Take(3).ToList();

        //v_i = x1 + F*(x2-x3)
        //multiplying v_i by 0.5 to scale down the vector 
        return (float)0.5 * (vectors[0].Vector + (float)differentialWeight * (vectors[1].Vector + vectors[2].Vector));
    }

    private Vector2 DifferentialCrossover(Vector2 mutantVector, Vector2 targetVector, double crossoverProbability)
    {
        Random random = new Random();
        double diceRoll = random.NextDouble();

        return diceRoll <= crossoverProbability ? mutantVector : targetVector;
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
    
    
    
}
    
    
    
    
