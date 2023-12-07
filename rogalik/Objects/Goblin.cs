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
        return _legs.StepTo((1, 1));
    }

    public override void Init()
    {
        _legs = owner.GetComponent<Legs>();        
    }
}

public class Goblin : Obj
{
    public Goblin(Location location) : base(location)
    {
        AddComponent(new Legs(1));
        AddComponent(new Appearance(R.Tiles.goblinUnarmed));
        AddComponent(new Heart());
        AddComponent(new GoblinMind());
    }
}