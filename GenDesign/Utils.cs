using System.Numerics;
using System.Runtime.CompilerServices;

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

    public static Vector2 GetAngleVector(Node a, Node b, int angle)
    {
        // returns the vector projected off of point b that will satisfy the angle provided
        // utilizes vector a->b to ensure that feasible directions are provided
        return new Vector2();
    }

    public static List<Vector2> GetAllPossibleAngles(Node a, Node b) 
    {
        // returns feasbile 0 deg, 45 deg, and 90 deg turns based on A->B
        return new List<Vector2>();
    }

    public static bool CheckCollision()
    {


        return false;
    }

    public static void DrawLine()
    {

    }

    
}