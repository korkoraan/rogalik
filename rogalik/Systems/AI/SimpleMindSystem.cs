using System;
using rogalik.Framework;
using rogalik.Systems.Combat;
using rogalik.Systems.Common;
using rogalik.Systems.Walking;

namespace rogalik.Systems.AI;

public class SimpleMindSystem : GameSystem
{
    public override void Update(uint ticks)
    {
        var objs = new Filter().With<Position>().With<Mind>().Apply(world.objects);
        foreach (var obj in objs)
        {
            var rndStep = new Point(Rnd.NewInt(-1, 1), Rnd.NewInt(-1, 1));
            obj.AddComponent(new ActionWalk(rndStep));
        }
    }

    public SimpleMindSystem(World world) : base(world)
    {
    }
}

public class Mind : IComponent
{
}