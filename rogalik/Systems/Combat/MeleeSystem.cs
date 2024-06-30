using System;
using System.Collections.Generic;
using rogalik.Framework;
using rogalik.Systems.Common;

namespace rogalik.Systems.Combat;

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
    protected override List<Type> GetDependencies() => new List<Type>()
    {
        typeof(PhysicalDmgSystem)
    };
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
            obj.RemoveComponent(hit);
        }
    }
}