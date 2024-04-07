using rogalik.Framework;

namespace rogalik.Combat;

public class DestructionSystem : GameSystem
{
    public DestructionSystem(World world) : base(world)
    {
    }

    public override void Update(uint ticks)
    {
        var filter = new Filter().With<Destroyed>().Apply(world.objects);

        foreach (var obj in filter)
        {
            world.objects.Remove(obj);
        }
    }
}