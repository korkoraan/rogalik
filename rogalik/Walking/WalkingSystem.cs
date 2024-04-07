using System.Linq;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Walking;

public class Leg : IComponent
{
}

public class ActionWalk : IComponent, IAction
{
    public Point vector;

    public ActionWalk(Point vector)
    {
        this.vector = vector;
    }
}

public class WalkingSystem : GameSystem
{
    public override void Update(uint ticks)
    {
        var filter = new Filter().With<ActionWalk>().Apply(world.objects);
        foreach (var obj in filter)
        {
            var actionWalk = obj.GetComponent<ActionWalk>();
            var vector = actionWalk.vector;
            obj.AddComponent(new Velocity(vector.x, vector.y, vector.z));
            obj.RemoveComponent(actionWalk);
        }
    }

    public WalkingSystem(World world) : base(world)
    {
    }
}