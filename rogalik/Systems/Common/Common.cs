using rogalik.Framework;

namespace rogalik.Systems.Common;

public class Weight : IComponent
{
    public uint value;

    public Weight(uint value)
    {
        this.value = value;
    }
}

public class BasicAttributes : IComponent
{
    public uint strength;
    public uint agility;
    public BasicAttributes(uint strength = 1, uint agility = 1)
    {
        this.strength = strength;
        this.agility = agility;
    }
}