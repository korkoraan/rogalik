using System.Collections.Generic;
using rogalik.Framework;

namespace rogalik.Common;

public struct WorldAction
{
    public readonly int startTime;
    public readonly int endTime;
    public readonly IComponent action;

    public WorldAction(int startTime, int endTime, IComponent action)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.action = action;
    }
}

public class ActionQueue : IComponent
{
    public Queue<WorldAction> queue = new ();
}