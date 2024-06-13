using System;
using System.IO;

namespace rogalik.Framework.Map;

public static class Generator
{
    private static Tile.Data CreateTile(Random rnd)
    {
        var result = new Tile.Data
        {
            kind = Tile.Kinds.GetRandom(rnd)
        };
        result.subKind = result.kind switch
        {
            Tile.Kind.rockFloor => (byte)rnd.Next(3), // see Rendering
            Tile.Kind.wallRock => (byte)rnd.Next(1), // see Rendering
            _ => throw new ArgumentOutOfRangeException()
        };

        result.attrs = (short)rnd.Next();
        return result;
    }

    private static Tile.Data CreateTile(float n, Random rnd)
    {
        var result = new Tile.Data
        {
            kind = Tile.Kinds.GetFromNoise(n)
        };
        result.subKind = result.kind switch
        {
            Tile.Kind.rockFloor => (byte)rnd.Next(3), // see Rendering
            Tile.Kind.wallRock => (byte)rnd.Next(1), // see Rendering
            _ => throw new ArgumentOutOfRangeException()
        };

        result.attrs = (short)rnd.Next();
        return result;
    }
    
    // normalize to current conditions 
    private static Tile.Data UpdateTile(Tile.Data tile) => tile;
    
    public static MapBase CreateNewMap(uint width, uint height)
    {
        MapBase result = new MapBaseSimple(width, height);
        var rnd = new Random();
        var perlin = new Perlin2D(rnd);
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                var p = perlin.Noise(x + rnd.NextSingle(), y + rnd.NextSingle(), 8) + 0.5f;
                result.SetTile(new MapPoint(x, y), CreateTile(p, rnd));
            }
        }
        return result;
    }

    public static MapBase CreateMapFromFile(string filename)
    {
        if (!Path.Exists(filename))
            throw new FileNotFoundException($"load map: {filename} is absent");
        using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
        {
            var width = reader.ReadUInt32();
            var height = reader.ReadUInt32();
            MapBase result = new MapBaseSimple(width, height);
            var tmp = new Tile.Data();
            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    tmp.value = reader.ReadInt32();
                    result.SetTile(new MapPoint(x, y), UpdateTile(tmp));
                }
            }
            return result;
        }
    }
}