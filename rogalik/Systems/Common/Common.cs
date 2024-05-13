using rogalik.Framework;

namespace rogalik.Systems.Common;

public class Density : IComponent
{
    public uint value;

    public Density(uint value)
    {
        this.value = value;
    }
}

public class Volume : IComponent
{
    public uint value;

    public Volume(uint value)
    {
        this.value = value;
    }
}