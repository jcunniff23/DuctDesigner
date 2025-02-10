using SkiaSharp;

namespace GenDesign;


// The purpose of this repository is to abstract my DuctRouter out into just the key algorithm, and implementing a layer on top of that,
// all while avoid the headache that is the Revit API.
public class Program
{
    // This class will be responsible for running and visualizing the pathfinding. starting with a 2D text output and stepping up to 3D via some package eventually.
    static void Main(string[] args)
    {
        // choose problem space size
        // choose start & target points
        // create solver class
        // draw with solver results

        Point start = new Point(0, 0);
        Point end = new Point(100, 100);

        GeneticEvolution geneticEvolution = new GeneticEvolution(
            start: start,
            objective: end,
            populationSize: 20,
            waypoints: 5,
            maximumGenerations: 15,
            mutationPerterbRate: 0.12,
            crossoverRate: 0.1
        );

        var best = geneticEvolution.Evolution();

    }
}