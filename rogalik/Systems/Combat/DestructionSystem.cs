using rogalik.Framework;
namespace rogalik.Systems.Combat;

public class DestructionSystem : GameSystem, IUpdateSystem
{
    public DestructionSystem(World world) : base(world)
    {
    }

    public void Update(uint ticks)
    {
        var filter = new Filter().With<Destroyed>().Apply(world.objects);

        foreach (var obj in filter)
        {
            world.objects.Remove(obj);
        }
    }
}