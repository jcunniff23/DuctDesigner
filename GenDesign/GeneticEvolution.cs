using System.Numerics;
using System.Security.Cryptography;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;

namespace GenDesign;

public class GeneticEvolution
{
    private int _generations = 0;
    private int _maximumGenerations;
    private readonly Point startPoint;
    private readonly Point endPoint;
    private int _waypoints;
    private int _populationSize;
    private Random _random;
    private List<PathIndividual> _population = new List<PathIndividual>();

    private double _mutationPerterbRate; // F
    private double _mutationRefreshRate;
    private double _crossoverRate; // CR
    

    // Genetic Algorithm for solving duct routing
    // starting with a 1-to-1 case
    public GeneticEvolution(Point start, Point objective, int populationSize,int waypoints, int maximumGenerations, double mutationPerterbRate, double crossoverRate)
    {
        _maximumGenerations = maximumGenerations;
        startPoint = start;
        endPoint = objective;
        _waypoints = waypoints; 
        _populationSize = populationSize;
        _mutationPerterbRate = mutationPerterbRate;
        _mutationRefreshRate = mutationPerterbRate*0.5;
        _crossoverRate = crossoverRate;
        _population = new List<PathIndividual>();
        _random = new Random();
    }

    private double FitnessSimple(PathIndividual individual)
    {
        // GOAL: MINIMIZE Pdrop
        double lengthCoeff = 0.15; // per unit length
        double deg45Coeff = 1.75; // per instance
        double def90Coeff = 2.25; // per instance
        
        
        double fitness = 0;
        fitness += individual.Segments.Sum(_ => _.Length) * lengthCoeff;

        for (int i = 1; i < individual.Segments.Count; i++)
        {
            double angle = CalculateTurnAngle(individual.Segments[i-1], individual.Segments[i]);
            fitness += angle switch
            {
                0 => 0,
                45 => deg45Coeff,
                90 => def90Coeff,
                _ => 50
            };
        }
        return fitness;
    }
    
    private double FitnessPressureDrop(PathIndividual pathIndividual)
    {
        // GOAL: MINIMIZE Pdrop
        double C0_duct = 0.15; // per unit length
        double C0_45degFitting = 0.75; // per instance
        double C0_90degFitting = 1.25; // per instance
        double totalLength = 0.0;

        int fittings45deg = 0;
        int fittings90deg = 0;

        // find instance of each turn & find all lengths
        for (int i = 0; i < pathIndividual.Segments.Count; i++)
        {
            DuctSegment thisSegment = pathIndividual.Segments[i];

            // if there are no nextSegments, break for loop
            if (i == pathIndividual.Segments.Count - 1)
            {
                break;
            }

            DuctSegment nextSegment = pathIndividual.Segments[i + 1];
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

    private double FitnessFootprint(PathIndividual individual)
    {
        // GOAL: MINIMIZE weighted sum of centroids distance from start
        double fitness = 0;
        double distancePenalty = 1; // per unit distance
        Point SolutionAreaCenter = new Point((startPoint.X + endPoint.X)*0.5, (startPoint.Y + endPoint.Y)*0.5);
        
        fitness += individual.Segments.Sum(_ => GetDistance(_.CenterPoint, SolutionAreaCenter)) * distancePenalty;

        return fitness;
    }

    private double FitnessCost(PathIndividual pathIndividual)
    {
        // GOAL: MINIMIZE Cost (Length/fittings)
        return 0.0;
    }
    
    private double FitnessError(PathIndividual individual)
    {
        
        double fitness = 0;
        double openSolutionPenalty = 10; // per unit length
        Point individualEnd = individual.Segments.Last().EndPoint;
        double endError = GetDistance(endPoint, individualEnd);
        fitness += endError * openSolutionPenalty;
        
        return fitness;
    }
    

    private List<PathIndividual> EqualizeGenomes(List<PathIndividual> paths)
    {
        PathIndividual longestPathIndividual = paths[0];
        foreach (PathIndividual path in paths)
        {
            if (path.Segments.Count > longestPathIndividual.Segments.Count)
            {
                longestPathIndividual = path;
            }
        }

        paths.Remove(longestPathIndividual);

        foreach (PathIndividual path in paths)
        {
            // Randomly? segment path until path.DuctSegments.count == longest

            while (path.Segments.Count < longestPathIndividual.Segments.Count)
            {
                // split random gene until satisifed       
            }
        }

        return paths;
    }
    
    private List<PathIndividual> SelectParents(List<double> fitnessScores)
    {
        var parents = new List<PathIndividual>();
        for (int i = 0; i < _populationSize; i++)
        {
            // sample 2 individuals and choose the better one
            int index1 = _random.Next(_populationSize);
            int index2 = _random.Next(_populationSize);
            var parent = fitnessScores[index1] < fitnessScores[index2]
                ? _population[index1]
                : _population[index2];
            parents.Add(parent);
        }
        return parents;
    }

    private PathIndividual Elitism(List<PathIndividual> paths, Func<PathIndividual, double> fitnessFunction)
    {
        // Preserve the best solution
        List<PathIndividual> sortedPaths = paths.OrderBy(fitnessFunction).ToList();
        PathIndividual bestPath = sortedPaths.First();
        return bestPath;
    }
    
    
    private PathIndividual SingleCrossover(PathIndividual parent1, PathIndividual parent2)
    {
        var childWaypoints = new List<WaypointGene>();
        int crossoverPoint = _random.Next(1, Math.Min(parent1.Waypoints.Count, parent2.Waypoints.Count));
        childWaypoints.AddRange(parent1.Waypoints.Take(crossoverPoint));
        childWaypoints.AddRange(parent2.Waypoints.Skip(crossoverPoint));
        var newPath = new PathIndividual(startPoint, endPoint, childWaypoints);
        newPath.Fitness = FitnessSimple(newPath);
        var mostFitParent = parent1.Fitness <= parent2.Fitness ? parent1 : parent2;

        if (newPath.Fitness < mostFitParent.Fitness)
        {
            return newPath;
        }
        else
        {
            return mostFitParent;
        }
    }

    private int mutatePerturbCount = 0;
    private int mutateRefreshCount = 0;
    private bool Mutate(PathIndividual individual)
    {

        var diceRoll = _random.NextDouble();
        if (diceRoll < (_mutationRefreshRate + _mutationPerterbRate) && diceRoll > _mutationPerterbRate)
        {
            var waypoints = GenerateRandomWaypoints();
            individual = new PathIndividual(startPoint, endPoint, waypoints);
            mutateRefreshCount++;
            return true;
        } 
        else if (diceRoll < _mutationPerterbRate)
        {
            //TODO adjust mutation step size
            var dist = GetDistance(startPoint, endPoint)*0.25;
            foreach (var waypoint in individual.Waypoints)
            {
                // Perturb position
                waypoint.Position = new Point(
                    waypoint.Position.X + _random.NextDouble() * (dist / individual.Waypoints.Count),
                    waypoint.Position.Y + _random.NextDouble() * (dist / individual.Waypoints.Count)
                );
            }
            
            RepairPathTurns(individual);
            mutatePerturbCount++;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ValidatePath(PathIndividual pathIndividual)
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

        foreach (DuctSegment segment in pathIndividual.Segments)
        {
        }


        return checkPath;
    }

    private void RepairPathTurns(PathIndividual individual)
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
         * OR!? ignore the incomplete paths for now and add a penalty function that will
         * act as a way to guide the solver to connect back the
         *
         */

        
        //Angle check
        for (int i = 1; i < individual.Segments.Count; i++)
        {
            var prev = individual.Segments[i - 1];
            var curr = individual.Segments[i];

            if (!IsValidTurn(prev, curr))
            {
                // Adjust current segment to nearest valid angle
                double angle = CalculateTurnAngle(prev, curr);
                double snappedAngle = SnapAngle(angle);
                Vector2 newDir = RotateVector(prev.Vector, snappedAngle);

                // Update waypoint position
                individual.Waypoints[i - 1].Position = new Point(
                    prev.StartPoint.X + newDir.X,
                    prev.StartPoint.Y + newDir.Y
                );
            }
        }
    }


    //Utility functions, move to separate class eventually
    //TODO refactor util functions?
    
    private Vector2 RotateVector(Vector2 vector, double angle)
    {
        float newX = (float)(vector.X * Math.Cos(angle) - vector.Y * Math.Sin(angle));
        float newY = (float)(vector.X * Math.Sin(angle) + vector.Y * Math.Cos(angle));
        
        return new Vector2(newX, newY);
    }
    
    private double CalculateTurnAngle(DuctSegment prev, DuctSegment curr)
    {
        Vector2 prevDir = prev.Vector;
        Vector2 currDir = curr.Vector;
        return Math.Acos(Vector2.Dot(prevDir, currDir) / (prevDir.Length() * currDir.Length())) * (180 / Math.PI);
    }

    private double SnapAngle(double angle)
    {
        return Math.Round(angle / 45) * 45;
    }

    private bool IsValidTurn(DuctSegment prev, DuctSegment curr)
    {
        double angle = CalculateTurnAngle(prev, curr);
        return angle % 45 == 0 && angle <= 90;
    }

    private void InitializePopulation()
    {
        for (int i = 0; i < _populationSize; i++)
        {
            var waypoints = GenerateRandomWaypoints();
            var ind = new PathIndividual(startPoint, endPoint, waypoints);
            ind.GenerationNumber = 0;
            _population.Add(ind);
        }
    }

    private List<WaypointGene> GenerateRandomWaypoints()
    {
        // int numWaypoints = _random.Next(1, _maxWaypoints + 1); // random # of turns between 1 and max
        int numWaypoints = _waypoints; // set number of turns by hyperparameter
        int counter = 0;
        List<WaypointGene> waypoints = new List<WaypointGene>();
        
        double dist = GetDistance(startPoint, endPoint);
        
        Point current = startPoint; // initialize search point with start point
        // Loop through and create new duct segments until condition completed
        while (counter < numWaypoints)
        {
            counter++;
            // Generate a new waypoint with 45° or 90° turns
            double angle = _random.Next(0, 4) * 45 * (Math.PI / 180); // snap to a 45 deg up to 135deg
            double length = _random.NextDouble() * (dist/ (_waypoints/2)); // search length with fudge factor for dynamic search distance?
            Point next = new Point(
                current.X + length * Math.Cos(angle),
                current.Y + length * Math.Sin(angle)
            );      //create waypoint based on current pos and ping with random value using distance

            // waypoints.Add(new WaypointGene { Position = next, IsActive = true });
            waypoints.Add(new WaypointGene { Position = next});
            current = next;
        }

        return waypoints;
    }
    

    public PathIndividual Evolution()
    {
        InitializePopulation();
        _population.ForEach(_ => _.Fitness = FitnessSimple(_));
        _population.ForEach(_ => _.FootprintFitness = FitnessFootprint(_));
        _population.ForEach(_ => _.EndPointFitness = FitnessError(_));
        
        List<PathIndividual> wholeFamily = new List<PathIndividual>();
        wholeFamily.AddRange(_population);
        
        
        while (_generations < _maximumGenerations)
        {
            /*Each evolution will:
             * 1. evaluate population
             * 2. retain the best path (elitism)
             * 3. select parents
             * 4. reproduce/crossover for children
             * 4a. repair child path to snap to feasible angles
             * 5. mutate (skip elite genome)
             * 5a. repair child mutation to snap to feasible angles
             */
            
            _generations++;
            int currentGen = _generations - 1;
            var elite = Elitism(_population, FitnessSimple);
            _population.Remove(elite);
            
            var parents = _population;
            var children = new List<PathIndividual>();
            while (children.Count < _populationSize)
            {
                // var parentsCross = SelectParents(fitness); //  more intelligent way to select parents?
                var parent1 = parents[_random.Next(parents.Count)];
                var parent2 = parents[_random.Next(parents.Count)];
                var child = SingleCrossover(parent1, parent2);
                Mutate(child);
                child.GenerationNumber = currentGen;
                child.Fitness = FitnessSimple(child);
                child.FootprintFitness = FitnessFootprint(child);
                child.EndPointFitness = FitnessError(child);
                
                children.Add(child);

            }
            
            elite.GenerationNumber = currentGen;
            children.Add(elite);
            _population = children;
            wholeFamily.AddRange(_population); // add newly created generation to whole family so it can be dumped to csv later

        }

        var basePath = "../../../assets/";
        var baseName = "GeneticEvolution";
        var date = $"{DateTime.Today:yyyyMMdd}";
        var fileNum = 0;
        string filePath = $"{basePath}{baseName}_{date}-{fileNum}";

        while (File.Exists(filePath))
        {
            fileNum++;
            filePath = $"{basePath}{baseName}_{date}-{fileNum}";
        }

        using (var writer = new StreamWriter($"{filePath}.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<PathMap>();
            wholeFamily = wholeFamily.OrderBy(_ => _.GenerationNumber).ToList(); // enforce sorting hopefully :,(
            csv.WriteRecords(wholeFamily);
        }
        Console.WriteLine($"Mutation Pertrub Count: {mutatePerturbCount}");
        Console.WriteLine($"Mutation Refresh Count: {mutateRefreshCount}");
        Console.WriteLine($"File Saved as '{filePath}'");
        
            
        return _population.OrderBy(FitnessSimple).First(); // return best 
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