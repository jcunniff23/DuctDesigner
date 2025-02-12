using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Newtonsoft.Json;

namespace GenDesign.DataStructures;

public class PathIndividual
{
    // DuctPath will represent the top level route between the trunk and terminal. This can later be abstracted to being
    // a secondary trunk that can host its own DuctPaths as branches
    
    public Point StartPoint { get; set; }
    public int GenerationNumber { get; set; }
    public Point EndPoint { get; set; }
    public List<WaypointGene> Waypoints { get; set; }
    public double Fitness { get; set; }
    public double FootprintFitness { get; set; }
    public double PressureDropFitness { get; set; }
    public double EndPointFitness { get; set; }
    public double FitnessCost { get; set; }
    public List<DuctSegment> Segments => CalculateSegmentsFromPoints();

    public PathIndividual(Point startPoint, Point endPoint, List<WaypointGene> waypoints)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Waypoints = waypoints;
        
    }
    
    // copy constructor 
    public PathIndividual(PathIndividual other)
    {
        GenerationNumber = other.GenerationNumber;
        Fitness = other.Fitness;
        FootprintFitness = other.FootprintFitness;
        EndPointFitness = other.EndPointFitness;
        StartPoint = other.StartPoint;
        EndPoint = other.EndPoint;
        Waypoints = other.Waypoints.Select(w => new WaypointGene(w)).ToList();
    }
    

    private List<DuctSegment> CalculateSegmentsFromPoints()
    {
        var segments = new List<DuctSegment>();
        Point current = StartPoint;
        foreach (var waypoint in Waypoints)
        {
            segments.Add(new DuctSegment(current, waypoint.Position));
            current = waypoint.Position;
        }
        segments.Add(new DuctSegment(current, EndPoint));
        return segments;    
    }
}


public sealed class PathMap : ClassMap<PathIndividual>
{
    public PathMap()
    {
        Map(m => m.GenerationNumber).Name("Generation");
        Map(m => m.Fitness).Name("Fitness"); //simple fitness function
        Map(m => m.FootprintFitness).Name("Footprint");
        // Map(m => m.PressureDropFitness).Name("Pressure_Drop");
        Map(m => m.EndPointFitness).Name("EndPointError");
        Map(m => m.FitnessCost).Name("Cost");
        Map(m => m.Waypoints).Name("Waypoints").TypeConverter<WaypointsJsonConverter>();
    }
}

public class WaypointsJsonConverter : DefaultTypeConverter
{
    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is List<WaypointGene> waypoints)
        {
            // Serialize the Waypoints list to a JSON string
            return JsonConvert.SerializeObject(waypoints);
        }
        return base.ConvertToString(value, row, memberMapData);
    }
}