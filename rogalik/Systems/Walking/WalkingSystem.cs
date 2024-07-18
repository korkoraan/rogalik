using rogalik.Framework;
using rogalik.Systems.Common;
using rogalik.Systems.Time;

namespace rogalik.Systems.Walking;

public class Leg : IComponent
{
}

public class ActionWalk
{
    public Point vector;

    public ActionWalk(Point vector)
    {
        this.vector = vector;
    }
}

public class DebuffWalk(uint percentage)
{
}

public class WalkingSystem : GameSystem, IEarlyUpdateSystem, IUpdateSystem
{
    public WalkingSystem(World world) : base(world)
    {
    }
    
    public void EarlyUpdate(uint ticks)
    {
        var objs = new Filter().With(o => o.GetComponent<Attempting>()?.action is ActionWalk).With<BasicAttributes>().Apply(world.objects);
        foreach (var obj in objs)
        {
            var attrs = obj.GetComponent<BasicAttributes>();
            var walk = (ActionWalk)obj.GetComponent<Attempting>().action;
            uint timeCost = Consts.BIG_TICK ;
            obj.Perform(walk, timeCost);
        }
    }
    
    public void Update(uint ticks)
    {
        var objs = new Filter().With(o => o.LastFinishedActionIs<ActionWalk>()).Apply(world.objects);
        foreach (var obj in objs)
        {
            var walk = (ActionWalk)obj.GetComponent<Actions>().queue.Peek().info;
            var vector = walk.vector;
            obj.AddComponent(new Velocity(vector.x, vector.y, vector.z));
        }
    }
}