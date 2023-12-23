using rogalik.Common;
using rogalik.Components;
using rogalik.Objects;

namespace rogalik.Framework;

public abstract class Action
{
    public readonly Obj actor;
    public uint duration { get; }
    public readonly uint energyCost;
    public readonly Obj target;
    protected Action(Obj actor, uint duration = Consts.BIG_TICK, uint energyCost = 1, Obj target = null)
    {
        this.actor = actor;
        this.duration = duration;
        this.energyCost = energyCost;
        this.target = target;
    }
    
    public abstract bool Apply();
}

public class DoNothing : Action
{
    public DoNothing(Obj actor) : base(actor) { }

    public override bool Apply()
    {
        return true; 
    }
}

public class MoveSelf : Action
{
    public readonly Point point;

    public MoveSelf(Obj actor, uint duration, uint energyCost, Point point) : base(actor: actor, duration: duration, energyCost: energyCost)
    {
        this.point = point;
    }
    public override bool Apply()
    {
        return actor.location.TryMoving(actor, point);
    }
}

public class BluntDamage : Action
{
    public readonly int damage;
    public BluntDamage(Obj actor, uint duration, uint energyCost, Obj target, int damage) : base(actor, duration, energyCost, target)
    {
        this.damage = damage;
    }

    public override bool Apply()
    {
        var destructible = target.GetAllComponents<IDestructible>();
        if(destructible.Count < 1) return false;
        var selected = destructible[Rnd.NewInt(0, destructible.Count - 1)];
        if(selected is IDestructible d)
            d.ReceiveDamage(damage);
        return true;
    }
}

public class OpenAction : Action
{
    private IOpenable _openable;
    public OpenAction(Obj actor, uint duration, uint energyCost, IOpenable openable) : base(actor: actor, duration: duration, energyCost: energyCost)
    {
        _openable = openable;
    }

    public override bool Apply()
    {
        return _openable.Open();
    }
}