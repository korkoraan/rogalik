using System;

namespace rogalik.Framework.Map.Tile;

public enum Kind: byte
{
    rockFloor
    , wallRock
}

public static class Kinds
{
    private static readonly Array _values = Enum.GetValues(typeof(Kind));
    public static Kind GetRandom(Random rnd) => (Kind)_values.GetValue(rnd.Next(_values.Length));
}
