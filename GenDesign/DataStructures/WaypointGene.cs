namespace GenDesign.DataStructures;

public class WaypointGene
{
    public Point Position { get; set; }


    public WaypointGene(Point position)
    {
        Position = position;
    }
    public WaypointGene(WaypointGene waypointGene)
    {
        Position = waypointGene.Position;
    }
    
}