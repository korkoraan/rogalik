using rogalik.Common;
using rogalik.Framework;

namespace rogalik.AI;

public class PlayerActivityMonitorSystem : GameSystem
{
    public bool playerDoingSmth;
    public override void Update(uint ticks)
    {
        var filter = new Filter().With<PlayerMind>().Apply(world.objects);
        var playerCount = 0; 
        foreach (var player in filter)
        {
            playerCount++;
            if(playerCount > 1) C.Print("WARNING: more than 1 PlayerMind detected!");

            playerDoingSmth = player.GetAllComponents<IAction>().Count > 0;
        }
    }

    public PlayerActivityMonitorSystem(World world) : base(world)
    {
    }
}

public class PlayerMind : IComponent
{
}
