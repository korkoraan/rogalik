using System.Collections.Generic;
using System.Linq;
using rogalik.Framework;
using rogalik.Systems.Combat;
using rogalik.Systems.Time;
using rogalik.Systems.Walking;

namespace rogalik.Systems.AI;

public class SimpleMindSystem : GameSystem, IEarlyUpdateSystem
{
    public void EarlyUpdate(uint ticks)
    {
        var objs = new Filter()
            .With<Position>()
            .With<Mind>()
            .Without<Attempting>()
            .With(o => !o.IsDoingSomething()).Apply(world.objects);
        foreach (var obj in objs)
        {
            var victims = world.
                GetObjectsInRadius(obj.GetComponent<Position>().point, 1)
                .Except([obj]).Where(o => o.HasComponent<Health>()).ToList();
                        
            if (victims.Count > 0)
            {
                var victim = Rnd.ElementOf(victims);
                obj.Attempt(new ActionHit(new Obj {new Weapon(10, 1, reach: 2)},victim));
                return;
            }
            // var rndStep = new Point(Rnd.NewInt(-1, 2), Rnd.NewInt(-1, 2));
            // obj.Do(new ActionWalk(rndStep));
            // obj.Attempt(new ActionWalk((-1, 0)));
        }
    }

    public SimpleMindSystem(World world) : base(world)
    {
    }
}

public class Mind : IComponent
{
    public enum Trait
    {
        
    }

    public List<Trait> traits = new ();
}