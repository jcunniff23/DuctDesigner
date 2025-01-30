using System.Security.Cryptography;

namespace GenDesign;

public class Solver
{
    private int _generations = 0;
    private int _maximumGenerations;
    private readonly Point startPoint;
    private readonly Point endPoint;
    private int _maximumTurns;
    public enum RouteMode 
    {
        OneToOne,
        OneToMany,
        ManyToMany
    }
    
    // Genetic Algorithm for solving duct routing
    // starting with a 1-to-1 case
    public Solver(Point start, Point objective, int initialPopulation, int maximumGenerations, RouteMode mode, int maximumTurns)
    {
        _maximumGenerations = maximumGenerations;
        startPoint = start;
        endPoint = objective;
        _maximumTurns = maximumTurns;
    }
    
    
    private void FitnessTest()
    {
        /*
            Fitness will be a function of:
                turns (pressure drop associated)
                distance
                area obstructed (obstacles collision)
                    -> need to give ducts an "area" until sizing can be implemented automatically

                

        */
    }
    private void SingleCrossover()
    {
        
    }

    private void DoubleCrossover()
    {
        
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

    private void RegeneratePath()
    {
        /*  To regenerate the path (after a broken mutation)
         *  the idea will be to take the remaining vector from
         *  the path's endpoint to the objective point
         *  then create a new vector that will follow the rules of
         *  the solver overall
         */
        
    }

    public DuctPath InitializeRandomRoute(Point start, Point end)
    {
        /*  TODO: random route needs to have decision making for how far each vector should travel ()
         *        initial random routes should be spread across the solution space (2d or 3d) so that the likelihood of being
         *        near the optimal solution is greater
         */
        Random random = new Random();
        List<DuctSegment> segments = new List<DuctSegment>();
        int randomTurnCount = random.Next(1, _maximumTurns+1); // random # of turns between 1 and max
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

    private void EncodePathToGenes()
    {
        /*
         *  Change data representation to be mutation/evolution friendly vectors?? 
         *  
         */
    }
    
    private void Evolution()
    {
            /*  
             *
             *
             *
             * 
             */
        

        while (_generations < _maximumGenerations)
        {
            List<DuctPath> currentPopulationSet = new List<DuctPath>(); //  newest evolution set, populate based on parents, or initial route on first gen
            
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
            
        return sign*angle;
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
        
        double newX = currentPosition.X + (distance * randDistance * Math.Cos(randAngle*Math.PI/180));
        double newY = currentPosition.Y + (distance * randDistance * Math.Sin(randAngle*Math.PI/180));
        Point nextPoint = new Point(newX, newY);
        
        return nextPoint;

    }

    private double GetDistance(Point pointA, Point pointB)
    {
        return Math.Sqrt((pointA.X - pointB.X)*(pointA.X - pointB.X) + (pointA.Y - pointB.Y)*(pointA.Y - pointB.Y));
    }
    
    
}