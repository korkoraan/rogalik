using System;

namespace rogalik.Framework.Map;

public readonly record struct MapPoint(int X, int Y)
{
    public override string ToString() => $"({X}, {Y})";
};

public readonly record struct Area(int X, int Y, uint Width, uint Height)
{
    public int EndX => (int)(X + Width - 1);
    public int EndY => (int)(Y + Height - 1);
    public override string ToString() => $"Area(xy: ({X}, {Y}), width: {Width}, height: {Height})";
};
public record struct MapPiece(Area area, Tile.Data[,] Tiles);

public abstract class MapBase(uint width, uint height)
{
    public readonly uint width = width;
    public readonly uint height = height;

    public abstract Tile.Data GetTile(MapPoint coord);
    public abstract void SetTile(MapPoint coord, Tile.Data data);
    public abstract MapPiece GetArea(Area area);
    public abstract void SetArea(MapPiece data);
}

public class MapBaseSimple(uint width, uint height) : MapBase(width, height)
{
    private readonly Tile.Data[,] _data = new Tile.Data[width, height];

    public override Tile.Data GetTile(MapPoint coord) => _data[coord.X, coord.Y];
    public override void SetTile(MapPoint coord, Tile.Data data) => _data[coord.X, coord.Y] = data;

    public override MapPiece GetArea(Area area)
    {
        if (area.X < 0 || area.Y < 0 || area.X + area.Width > width || area.Y + area.Height > height) 
            throw new ArgumentOutOfRangeException($"{area} is out of map");
        var result = new Tile.Data[area.Width, area.Height];
        var stop = new MapPoint(area.EndX, area.EndY);
        for (int x = area.X, i = 0; x <= stop.X; ++x, ++i)
        {
            for (int y = area.Y, j = 0; y <= stop.Y; ++y, ++j)
            {
                result[i, j].value = _data[x, y].value;
            }
        }
        return new MapPiece(area, result);
    }

    public override void SetArea(MapPiece data)
    {
        var area = data.area;
        if (area.X < 0 || area.Y < 0 || area.X + area.Width > width || area.Y + area.Height > height) 
            throw new ArgumentOutOfRangeException($"{area} is out of map");
        var stop = new MapPoint(area.EndX, area.EndY);
        for (int x = area.X, i = 0; x <= stop.X; ++x, ++i)
        {
            for (int y = area.Y, j = 0; y <= stop.Y; ++y, ++j)
            {
                _data[x, y].value = data.Tiles[i, j].value;
            }
        }
    }
}
