using rogalik.Common;
using rogalik.Framework;
using Action = rogalik.Framework.Action;

namespace rogalik.Components;

public class Legs : Component
{
    //TODO: probably should be declared as attribute somewhere in owner 
    public readonly uint speed;
    public Legs(uint speed)
    {
        this.speed = speed;
    }
    
    public override void Init()
    {
    }
    
    public Action StepTo(Point point)
    {
        return new MoveSelf(owner, Consts.BIG_TICK / speed, 1000, point);
    }

    public Action Kick(Obj target)
    {
        var dmg = Rnd.NewInt(1, 10);
        return new BluntDamage(
            actor: owner, 
            duration: Consts.BIG_TICK / speed,
            energyCost: 1000,
            target: target,
            damage: dmg
            );
    }
}