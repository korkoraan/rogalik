using rogalik.Common;
using rogalik.Framework;

namespace rogalik.Combat;

public class ActionHit : IComponent
{
    public Obj weapon;
    public Obj target;

    public ActionHit(Obj weapon, Obj target)
    {
        this.weapon = weapon;
        this.target = target;
    }
}

/// <summary>
/// damage with melee weapons
/// </summary>
public class MeleeSystem : GameSystem
{
    public MeleeSystem(World world) : base(world)
    {
    }

    public override void Update(uint ticks)
    {
        var filter = new Filter().With<ActionHit>().Apply(world.objects);

        foreach (var obj in filter)
        {
            var hit = obj.GetComponent<ActionHit>();
            var volume = hit.weapon.GetComponent<Volume>()?.value ?? 0;
            var density = hit.weapon.GetComponent<Density>()?.value ?? 0;
            var mass = volume * density;
            uint speed = 1;
            var dmgPts = mass * speed;

            hit.target.AddComponent(new DmgPhysical(dmgPts));
        }
    }
}