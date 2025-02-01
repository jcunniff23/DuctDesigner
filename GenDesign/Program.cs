namespace GenDesign;


// The purpose of this repository is to abstract my DuctRouter out into just the key algorithm, and implementing a layer on top of that,
// all while avoid the headache that is the Revit API.
public class Program
{
    // This class will be responsible for running and visualizing the pathfinding. starting with a 2D text output and stepping up to 3D via some package eventually.
    static void Main()
    {
        // choose problem space size
        // choose start & target points
        // create solver class
        // draw with solver results
        
        Point start = new Point(0, 0);
        Point end = new Point(10, 10);

        GeneticEvolution geneticEvolution = new GeneticEvolution(start, end, 10, 10, GeneticEvolution.RouteMode.OneToOne, 8);
        DuctPath path = geneticEvolution.InitializeRandomRoute(start, end);

        foreach (DuctSegment segment in path.DuctSegments)
        {
            Console.WriteLine($"({segment.Points[0].X}, {segment.Points[0].Y})");
            Console.WriteLine($"({segment.Points[1].X}, {segment.Points[1].Y})");
            Console.WriteLine("-----------------");
        }
        
    }

    private static void DrawPath(DuctPath path)
    {
        
    }
}