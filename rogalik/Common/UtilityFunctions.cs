using System;
using System.Collections.Generic;

namespace rogalik.Common;

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

    public static int NewInt(int min, int max)
    {
        return _random.Next(min, max);
    }
}