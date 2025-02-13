namespace GenDesign;

public class Utils
{

    public void CreateObstacle()
    {
        
    }

    public static double GetDistance(Node a, Node b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
    
    
}