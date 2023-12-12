using rogalik.Common;
using rogalik.Components;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Objects;

/// <summary>
/// A cell surface. Floor basically.
/// </summary>
public class Surface : Obj
{
    public Surface(Point point, Location location) : base(point, location)
    {
        AddComponent(new Appearance(R.Tiles.surfaceRock.Random()));
    }

}