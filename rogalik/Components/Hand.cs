using rogalik.Framework;
using rogalik.Objects;

namespace rogalik.Components;

public class Hand : Component
{
    public override void Init()
    {
    }

    public OpenAction Open(IOpenable openable)
    {
        return new OpenAction(owner, Consts.BIG_TICK, 0, openable);
    }
}