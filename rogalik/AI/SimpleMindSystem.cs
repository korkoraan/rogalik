using rogalik.Common;
using rogalik.Framework;
using rogalik.Walking;

namespace rogalik.AI;

public class SimpleMindSystem : GameSystem
{
    public override void Update(uint ticks)
    {
        var filter = new Filter().With<Mind>().Apply(world.objects);
        foreach (var obj in filter)
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