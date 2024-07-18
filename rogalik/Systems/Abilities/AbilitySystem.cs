using System.Collections;
using System.Collections.Generic;
using rogalik.Framework;

namespace rogalik.Systems.Abilities;

public class Ability : IComponent
{
    public readonly string name;
    public readonly string description;

    public Ability(string name, string description = "")
    {
        this.name = name;
        this.description = description;
    }
}

public class PossessedAbilities : IComponent, IEnumerable
{
    public readonly List<Obj> items = new();

    public void Add(Obj ability)
    {
        items.Add(ability);
    }
    
    public IEnumerator GetEnumerator()
    {
        return items.GetEnumerator();
    }
}

public class AbilitySystem : GameSystem, IUpdateSystem
{
    public AbilitySystem(World world) : base(world)
    {
    }

    public void Update(uint ticks)
    {
        throw new System.NotImplementedException();
    }
}