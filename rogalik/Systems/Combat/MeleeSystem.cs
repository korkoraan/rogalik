using System;
using rogalik.Framework;
using rogalik.Rendering;
using rogalik.Systems.Common;
using rogalik.Systems.Time;
using rogalik.Systems.Walking;

namespace rogalik.Systems.Combat;

public class ActionHit
{
    public Obj weapon;
    public Obj target;

    public ActionHit(Obj weapon, Obj target)
    {
        this.weapon = weapon;
        this.target = target;
    }
}

public class Weapon : IComponent
{
    public uint D;
    public uint diceCount;
    public uint reach;

    public Weapon(uint d, uint count, uint reach = 1)
    {
        D = d;
        diceCount = count;
        this.reach = reach;
    }
}

/// <summary>
/// damage with melee weapons
/// </summary>
public class MeleeSystem : GameSystem, IEarlyUpdateSystem, IUpdateSystem
{
    public MeleeSystem(World world) : base(world)
    {
    }
    
    public void EarlyUpdate(uint ticks)
    {
        var objs = new Filter()
            .With(o => o.GetComponent<Attempting>()?.action is ActionHit)
            .With<BasicAttributes>()
            .Apply(world.objects);
        foreach (var obj in objs)
        {
            var hit = (ActionHit)obj.GetComponent<Attempting>()?.action;
            var attrs = obj.GetComponent<BasicAttributes>();
            obj.Perform(hit, Consts.BIG_TICK / attrs.agility);
        }
    }

    public void Update(uint ticks)
    {
        var query = new Filter()
            .With(o => o.GetLastFinishedAction()?.info is ActionHit hit && hit.target.HasComponent<Position>())
            .With<BasicAttributes>()
            .Apply(world.objects);
        foreach (var obj in query)
        {
            var hit = (ActionHit)obj.GetLastFinishedAction()?.info;
            bool success = true;
            var attacker = obj == world.player ? "you" : $"{obj.Description()}";
            var victim = hit.target == world.player ? "you" : $"{obj.Description()}";
                
            var msg = hit.target != obj
                ? $"{attacker} attacks {hit.target.Description()}"
                : $"{attacker} attacks himself";
            var weaponReach = hit.weapon.GetComponent<Weapon>()?.reach ?? 1;
            var distance = Geometry.Distance(obj.GetComponent<Position>().point, hit.target.GetComponent<Position>().point);
            var attrs = obj.GetComponent<BasicAttributes>();
            var targetAgility = hit.target.GetComponent<BasicAttributes>()?.agility ?? 0;
            var circumstances = "";
            if (distance > weaponReach)
            {
                circumstances = ", but " + victim == "you" ? "you" : "" + $" can't reach that far: r {weaponReach}, d {distance}";
                success = false;
            }
            else if (targetAgility + Rnd.D(20) >= attrs.agility + Rnd.D(20))
            {
                circumstances = ", but misses";
                success = false;
            }
            UIData.AddLogMessage(msg + circumstances);
            if (!success) continue;
            
            float dmgPts;
            var weapon = hit.weapon.GetComponent<Weapon>();
            if (weapon != null)
                dmgPts = Rnd.D(weapon.D) * weapon.diceCount;
            else
                dmgPts = hit.weapon.GetComponent<Weight>()?.value ?? 0;
            dmgPts = dmgPts / 100 * (100 + (attrs.strength - 10) * 5);
            hit.target.AddComponent(new DmgPhysical((uint)Math.Round(dmgPts)));
        }
    }
}