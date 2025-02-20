namespace GenDesign.DataStructures;

public class NodePath
{
    private int _length;
    public List<Node> Nodes { get; set; } =  new List<Node>();

    public double Length => CalculateLength();
    
    public int Turns => CalculateTurns();

    private int CalculateTurns()
    {
        int turns = 0;
        for (int i = 2; i < _length; i++)
        {
            double dx1 = Nodes[i - 1].X - Nodes[i - 2].X;
            double dy1 = Nodes[i - 1].Y - Nodes[i - 2].Y;
            double dx2 = Nodes[i].X - Nodes[i - 1].X;
            double dy2 = Nodes[i].Y - Nodes[i - 1].Y;

            if (dx1 * dy1 != dx2 * dy2) turns++;
        }

        return turns;
    }

    //private List<Node> Get45DegTurns()
    //{
    //    int turns = 0;
    //    for (int i = 2; i < _length; i++)
    //    {
    //        double dx1 = Nodes[i - 1].X - Nodes[i - 2].X;
    //        double dy1 = Nodes[i - 1].Y - Nodes[i - 2].Y;
    //        double dx2 = Nodes[i].X - Nodes[i - 1].X;
    //        double dy2 = Nodes[i].Y - Nodes[i - 1].Y;

    //        if (dx1 * dy1 != dx2 * dy2) turns++;
    //    }
    //}


    private double CalculateDistance(Node a, Node b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    private double CalculateLength()
    {
        double length = 0;
        for (int i = 1; i < Nodes.Count; i++)
        {
            length += CalculateDistance(Nodes[i], Nodes[i - 1]);
        }

        return length;
    }
    
    
}