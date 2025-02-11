using TerminalDesign.DataStructures;

namespace TerminalDesign;

public class TerminalSolver
{

    private double _mutationRate;
    private double _crossoverRate;
    private Random _random;
    private int _maxGenerations;
    private int _currentGeneration = 0;
    private List<TerminalGene> _population;
    
    public TerminalSolver()
    {
        
    }


    private bool Crossover()
    {
        return false;
    }

    private bool TournamentSelection()
    {
        //
        return false;
    }

    private bool Mutate()
    {
        // perturb terminal XY location (+/- small amount)
        // have small chance to create new terminal location (~2%)
        
        return false;
    }

    private double Fitness()
    {
        //  Maximize: Coverage
        //  or - minimize penalties
        double fitness = 0;

        foreach (TerminalGene gene in _population)
        {
            //check if terminal is too close to walls -> penalty
            //check if terminal is too close to any other terminal -> penalty
            //check coverage of terminal - reward
        }
        
        return fitness
    }

    private List<TerminalGene> InitialSolution()
    {
        return new List<TerminalGene>();
    }

    private List<TerminalGene> Solve()
    {
        _population = InitialSolution();
        
        
        while (_currentGeneration < _maxGenerations)
        {
            
        }
        
        
        
        
        return new List<TerminalGene>();
    }
    
    
    
    
    
}