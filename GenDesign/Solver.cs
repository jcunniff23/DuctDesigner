using System.Security.Cryptography;

namespace GenDesign;

public class Solver
{
    private int _generations = 0;
    private int _maximumGenerations;
    private readonly Point startPoint;
    private readonly Point endPoint;
    public enum RouteMode 
    {
        OneToOne,
        OneToMany,
        ManyToMany
    }
    
    // Genetic Algorithm for solving duct routing
    // starting with a 1-to-1 case
    public Solver(Point start, Point objective, int initialPopulation, int maximumGenerations, RouteMode mode)
    {
        _maximumGenerations = maximumGenerations;
        startPoint = start;
        endPoint = objective;
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
    private void Crossover()
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

    private void InitializeRandomRoute(Point start, Point end)
    {
        /*  TODO: random route needs to have decision making for how far each vector should travel ()
         *        initial random routes should be spread across the solution space (2d or 3d) so that the liklihood of being
         *        near the optimal solution is greater
         */
        
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
        Random rand = new Random();
        int angle;
        var sign = 1;
        int random1 = rand.Next(1);
        int random2 = rand.Next(1);
        
        if (random1 == 0)
            angle = 45;
        else
            angle = 90;

        if (random2 == 0)
            sign = 1;
        else
            sign = -1;
            
        return sign*angle;
    }
    
    
}