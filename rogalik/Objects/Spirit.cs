using rogalik.Common;
using rogalik.Components;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Objects;

public class SpiritMind : Mind
{
    private Legs _legs;

    public override Action ChooseNextAction()
    {
        var p = new Point(Rnd.NewInt(-1,1), Rnd.NewInt(-1,1));
        return _legs?.StepTo(p);
    }

    public override void Init()
    {
        _legs = owner.GetComponent<Legs>();
    }
}

public class Spirit : Obj
{
    public Spirit(Location location) : base(location)
    {
        AddComponent(new Appearance(R.Tiles.spirit));
        AddComponent(new Legs(1));
        AddComponent(new SpiritMind());
    }
}