using System.Security.Cryptography;

namespace GenDesign;

public class Solver
{
    private int _generations = 0;
    private int _maximumGenerations;
    
    // Genetic Algorithm for solving duct routing
    private enum ValidDegreeAngles
    {
        _45 = 45, 
        _90 = 90
    }

    // starting with a 1-to-1 case
    public Solver(Point start, Point objective, int initialPopulation, int maximumGenerations)
    {
        _maximumGenerations = maximumGenerations;
    }
    
    
    private void FitnessTest()
    {
        
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

    private bool ValidatePath()
    {
        /*  Test whether the path:
         *      is continuous from start point to target
         *      has valid turns
         *      does not collide with obstacles?
         *
         *  validate path will not do scoring, only check for issues that would
         *  ruin future generations
         */
        return false;
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

    private void InitializeRandomRoute()
    {
        
    }

    private void EncodePathToGenes()
    {
        /*
         *  Change data representation to be mutation/evolution
         *  friendly vectors?? 
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
            _generations++;
            List<DuctPath> currentPopulationSet = new List<DuctPath>();
            


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