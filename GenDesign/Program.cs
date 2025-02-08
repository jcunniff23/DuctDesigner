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
            populationSize: 10,
            waypoints: 5,
            maximumGenerations: 5,
            GeneticEvolution.RouteMode.OneToOne,
            mutationPerterbRate: 0.1,
            crossoverRate: 0.1
        );

        var best = geneticEvolution.Evolution();

        VisualizePath(best);

    }

    static void VisualizePath(PathIndividual path)
        {
            // Create a bitmap to draw on
            int width = 800;
            int height = 800;
            var info = new SKImageInfo(width, height);
            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;

                // Clear the canvas
                canvas.Clear(SKColors.White);

                // Draw the path
                var paint = new SKPaint
                {
                    Color = SKColors.Blue,
                    StrokeWidth = 2,
                    IsAntialias = true
                };

                // Draw segments
                foreach (var segment in path.Segments)
                {
                    canvas.DrawLine(
                        ScalePoint(segment.StartPoint, width, height),
                        ScalePoint(segment.EndPoint, width, height),
                        paint
                    );
                }

                // Draw waypoints
                var waypointPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    IsAntialias = true
                };
                foreach (var waypoint in path.Waypoints)
                {
                    var point = ScalePoint(waypoint.Position, width, height);
                    canvas.DrawCircle(point, 5, waypointPaint);
                }

                // Draw start and end points
                var endpointPaint = new SKPaint
                {
                    Color = SKColors.Green,
                    IsAntialias = true
                };
                canvas.DrawCircle(ScalePoint(path.StartPoint, width, height), 5, endpointPaint);
                canvas.DrawCircle(ScalePoint(path.EndPoint, width, height), 5, endpointPaint);

                // Save the image to a file
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = System.IO.File.OpenWrite("../../../assets/path_visualization.png"))
                {
                    data.SaveTo(stream);
                }
            }

            Console.WriteLine("Visualization saved to 'path_visualization.png'");
        }

        // Scale points to fit the canvas
    static SKPoint ScalePoint(Point point, int width, int height)
        {
            float scaleX = width / 100f;
            float scaleY = height / 100f;
            return new SKPoint((float)point.X * scaleX, (float)point.Y * scaleY);
        }

    static void ExportPathCSV(PathIndividual path)
    {
        
    }
}