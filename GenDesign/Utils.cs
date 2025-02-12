using System.Numerics;

namespace GenDesign;

public class Utils
{
    public Utils()
    {
    }

    internal Vector2 RotateVector(Vector2 vector, double angle)
    {
        float newX = (float)(vector.X * Math.Cos(angle) - vector.Y * Math.Sin(angle));
        float newY = (float)(vector.X * Math.Sin(angle) + vector.Y * Math.Cos(angle));
        
        return new Vector2(newX, newY);
    }

    internal double CalculateTurnAngle(DuctSegment prev, DuctSegment curr)
    {
        Vector2 prevDir = prev.Vector;
        Vector2 currDir = curr.Vector;
        return Math.Acos(Vector2.Dot(prevDir, currDir) / (prevDir.Length() * currDir.Length())) * (180 / Math.PI);
    }

    internal double SnapAngle(double angle)
    {
        return Math.Round(angle / 45) * 45;
    }

    internal bool IsValidTurn(DuctSegment prev, DuctSegment curr)
    {
        double angle = CalculateTurnAngle(prev, curr);
        return angle % 45 == 0 && angle <= 90;
    }

    internal int RandomAngle()
    {
        // Will randomly return an angle: -90, -45, 0, 45, 90

        Random rand = new Random();
        int angle;
        var sign = 1;
        int random1 = rand.Next(2);
        int random2 = rand.Next(1);

        if (random1 == 0)
            angle = 45;
        else if (random1 == 1)
            angle = 90;
        else
            return 0;

        if (random2 == 0)
            sign = 1;
        else
            sign = -1;

        return sign * angle;
    }

    internal double GetDistance(Point pointA, Point pointB)
    {
        return Math.Sqrt((pointA.X - pointB.X) * (pointA.X - pointB.X) + (pointA.Y - pointB.Y) * (pointA.Y - pointB.Y));
    }
}