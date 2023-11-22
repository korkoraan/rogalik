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
        Console.WriteLine(o.ToString());
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