using System;
using System.Collections.Generic;

namespace rogalik.Framework;

/// <summary>
/// Console utility.
/// </summary>
public static class C
{
    public static void Print(string msg)
    {
        Console.WriteLine(msg);
    }
    public static void Print(object o)
    {
        var msg = o is null ? "null" : o.ToString();
        Print(msg);
    }
}

public static class ListExtension
{
    private static readonly Random _rng = new();

    /// <returns>random element of list T</returns>
    public static T Random<T>(this IList<T> list)
    {
        return list[_rng.Next(list.Count)];
    }
}

public static class Rnd
{
    private static Random _random;
    static Rnd()
    {
        _random = new Random();
    }

    /// <returns>returns value between 2 ints not including max</returns>
    public static int NewInt(int min, int max)
    {
        return _random.Next(min, max);
    }

    /// <returns>random element of the given list</returns>
    public static T ElementOf<T>(List<T> list)
    {
        return list[NewInt(0, list.Count)];
    }
    
    /// <returns>random index of the given list</returns>
    public static int Index<T>(List<T> list)
    {
        return NewInt(0, list.Count);
    }
}

public static class Geometry
{
    /// <returns>a list of points representing a line between 2 points. If p1 == p2 returns a list of 1 point</returns>
    public static List<Point> Line(Point p1, Point p2)
    {
        if (p1 == p2) return new List<Point> { p1 };
        
        var points = new List<Point>();
        int n = Distance(p1, p2);
        for (var step = 0f; step <= n; step++)
        {
            var pt = step / n;
            var t = n == 0 ? 0f : pt;
            points.Add(LerpPoint(p1, p2, t));
        }

        return points;
    }

    private static Point LerpPoint(Point p1, Point p2, float t)
    {
        var x = (int)Math.Round(Lerp(p1.x, p2.x, t));
        var y = (int)Math.Round(Lerp(p1.y, p2.y, t));
        return new Point(x, y);
    }

    private static float Lerp(float start, float end, float t)
    {
        return start * (1f - t) + end * t;
    }
    
    public static int Distance(Point p1, Point p2)
    {
        return Math.Max(Math.Abs(p1.x - p2.x), Math.Abs(p1.y - p2.y));
    }
}