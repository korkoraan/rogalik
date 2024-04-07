using System;

namespace rogalik.Framework;

/// <summary>
/// A three dimensional point in space.
/// <param name="x"> width </param>
/// <param name="y"> height </param>
/// <param name="z"> depth </param> 
/// </summary>
public struct Point
{
    public bool Equals(Point other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }

    public int x;
    public int y;
    public int z;

    public Point(int x, int y, int z = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator Point((int x, int y) t) => new Point(t.x, t.y, 0);
    public static implicit operator Point((int x, int y, int z) t) => new Point(t.x, t.y, t.z);
    public override string ToString()
    {
        return $"(x:{x} y:{y} z:{z})";
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }
    
    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
    }
    
    public static Point operator +(Point p1, int i)
    {
        return new Point(p1.x + i, p1.y + i, p1.z + i);
    }
    
    public static Point operator -(Point p1, int i)
    {
        return new Point(p1.x - i, p1.y - i, p1.z - i);
    }

    public static Point operator *(Point p, int i)
    {
        return new Point(p.x * i, p.y * i, p.z * i);
    }
    
    public static Point operator /(Point p, int i)
    {
        return new Point(p.x / i, p.y / i, p.z / i);
    }
    
    public static bool operator ==(Point p1, Point p2)
    {
        return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1 == p2);
    }

    public bool InRange(Point start, Point end)
    {
        return end.x >= x && x >= start.x && end.y >= y && y >= start.y && end.z >= z && z >= start.z;
    }
    
}