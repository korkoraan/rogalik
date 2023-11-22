namespace rogalik.Common;

/// <summary>
/// A three dimensional point in space.
/// <param name="x"> width </param>
/// <param name="y"> height </param>
/// <param name="z"> depth </param> 
/// </summary>
public struct Point
{
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
}