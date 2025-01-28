namespace GenDesign;

public class DuctPath
{
    // DuctPath will represent the top level route between the trunk and terminal. This can later be abstracted to being
    // a secondary trunk that can host its own DuctPaths as branches
    
    public List<Point> Points => DuctSegments.SelectMany(d => d.Points).ToList(); 
    public List<DuctSegment> DuctSegments { get; set; }

    public DuctPath(List<DuctSegment> ductSegments)
    {
        DuctSegments = ductSegments;
    }
}