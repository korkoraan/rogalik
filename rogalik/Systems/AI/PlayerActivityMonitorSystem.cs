using rogalik.Framework;
namespace rogalik.Systems.AI;

public class PlayerActivityMonitorSystem : GameSystem, IUpdateSystem
{
    public bool playerDoingSmth;
    public void Update(uint ticks)
    {
        var filter = new Filter().With<PlayerMind>().Apply(world.objects);
        var playerCount = 0; 
        foreach (var _ in filter)
        {
            playerCount++;
            if(playerCount > 1) C.Print("WARNING: more than 1 PlayerMind detected!");
        }
    }

    public PlayerActivityMonitorSystem(World world) : base(world)
    {
    }
}

public class PlayerMind : IComponent
{
}
