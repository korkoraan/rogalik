using rogalik.Common;
using rogalik.Components;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Objects;

public class GoblinMind : Mind
{
    private Legs _legs;

    public override Action ChooseNextAction()
    {
        if (energyCurrent < 1) return null;
        var x = Rnd.NewInt(0, 2);
        var y = Rnd.NewInt(0, 2);
        return _legs.StepTo((x, y));
    }

    public override void Init()
    {
        _legs = owner.GetComponent<Legs>();        
    }
}

public class Goblin : Obj
{
    public Goblin(Point point, Location location) : base(point, location)
    {
        AddComponent(new Legs(1));
        AddComponent(new Appearance(R.Tiles.goblinUnarmed));
        AddComponent(new Heart());
        AddComponent(new GoblinMind());
    }
}