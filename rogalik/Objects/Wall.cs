using rogalik.Common;
using rogalik.Components;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Objects;

public interface IOpenable
{
    public bool Open();
}

public class Wall : Obj

{
    public Wall(Point point, Location location) : base(point, location)
    {
        AddComponent(new Appearance(R.Tiles.wallRock.Random()));
        AddComponent(new Impassible());
    }
}