using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Systems.Combat;

public class Rigid : IComponent
{
    public int maxIntegrityPts; 
    public int integrityPts;

    public Rigid(int maxIntegrityPts)
    {
        this.maxIntegrityPts = maxIntegrityPts;
        integrityPts = maxIntegrityPts;
    }

    public Rigid(int maxIntegrityPts, int initialIntegrityPts)
    {
        this.maxIntegrityPts = maxIntegrityPts;
        integrityPts = initialIntegrityPts;
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
        var filter = new Filter().With<DmgPhysical>().With<Rigid>().Apply(world.objects);

        foreach (var obj in filter)
        {
            var rigid = obj.GetComponent<Rigid>();
            UIData.AddLogMessage($"{obj} gets hit for {obj.GetComponent<DmgPhysical>().dmgPts}");
            rigid.integrityPts -= (int)obj.GetComponent<DmgPhysical>().dmgPts;
            UIData.AddLogMessage($"{obj} now has {rigid.integrityPts} integrity");
            if (rigid.integrityPts <= 0)
            {
                obj.AddComponent(new Destroyed());
                UIData.AddLogMessage($"{obj} is destroyed");
            }
        }
    }
}