using System;

namespace rogalik.Framework.Map.Tile;

public enum Kind: byte
{
    rockFloor
    , wallRock
}

public static class Kinds
{
    private static readonly Array Values = Enum.GetValues(typeof(Kind));
    public static Kind GetRandom(Random rnd) => (Kind)Values.GetValue(rnd.Next(Values.Length));
    public static Kind GetFromNoise(float n)
    {
        return n switch
        {
            < 0.55f => Kind.rockFloor,
            _ => Kind.wallRock
        };
    }
}
