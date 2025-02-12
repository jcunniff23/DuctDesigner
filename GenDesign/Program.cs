using System.Globalization;
using CsvHelper;
using GenDesign.DataStructures;
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
            crossoverRate: 0.5
        );

        var wholeFamily = geneticEvolution.Evolution();
        
        var basePath = "../../../assets/";
        var baseName = "GeneticEvolution-NoElitism-M012";
        var date = $"{DateTime.Today:yyyyMMdd}";
        var fileNum = 0;
        string filePath = $"{basePath}{baseName}_{date}-{fileNum}.csv";

        while (File.Exists(filePath))
        {
            fileNum++;
            filePath = $"{basePath}{baseName}_{date}-{fileNum}.csv";
        }

        using (var writer = new StreamWriter($"{filePath}"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<PathMap>();
            wholeFamily = wholeFamily.OrderBy(_ => _.GenerationNumber).ToList(); // enforce sorting hopefully :,(
            csv.WriteRecords(wholeFamily);
        }
        Console.WriteLine($"Mutation Perturb Count: {geneticEvolution.MutatePerturbCount}");
        Console.WriteLine($"Mutation Refresh Count: {geneticEvolution.MutateRefreshCount}");
        Console.WriteLine($"File Saved as '{filePath}'");

    }
}