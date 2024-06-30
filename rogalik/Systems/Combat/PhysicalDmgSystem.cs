using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Systems.Combat;

public class Health : IComponent
{
    public int maxHealthPts; 
    public int healthPts;

    public Health(int maxHealthPts)
    {
        this.maxHealthPts = maxHealthPts;
        healthPts = maxHealthPts;
    }

    public Health(int maxHealthPts, int initialHealthPts)
    {
        this.maxHealthPts = maxHealthPts;
        healthPts = initialHealthPts;
    }
}

public class Destroyed : IComponent
{
}

public class DmgPhysical : IComponent
{
    public uint dmgPts;

    public DmgPhysical(uint dmgPoints)
    {
        dmgPts = dmgPoints;
    }
}

public class PhysicalDmgSystem : GameSystem
{
    public PhysicalDmgSystem(World world) : base(world)
    {
    }

    public override void Update(uint ticks)
    {
        var filter = new Filter().With<DmgPhysical>().With<Health>().Apply(world.objects);

        foreach (var obj in filter)
        {
            var rigid = obj.GetComponent<Health>();
            var dmg = obj.GetComponent<DmgPhysical>();
            UIData.AddLogMessage($"{obj} gets hit for {obj.GetComponent<DmgPhysical>().dmgPts} damage points");
            rigid.healthPts -= (int)dmg.dmgPts;
            UIData.AddLogMessage($"{obj} now has {rigid.healthPts} health");
            obj.RemoveComponent(dmg);
            if (rigid.healthPts <= 0)
            {
                obj.AddComponent(new Destroyed());
                UIData.AddLogMessage($"{obj} is destroyed");
            }
        }
    }
}