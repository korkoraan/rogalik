using System.Collections.Generic;
using rogalik.Framework;
namespace rogalik.Systems.Time;

public class Attempting(object action) : IComponent
{
    public object action = action;
}

public class ActionPoints(int limit) : IComponent
{
    public readonly int limit = limit;
    public int value = limit;

    public ActionPoints(int limit, int startValue) : this(limit)
    {
        value = startValue;
    }
}

public class Actions : IComponent
{
    public class Action
    {
        public object info;
        public readonly uint timeCost;
        public int timeRemaining;
        public bool finished;

        public Action(object info, uint timeCost)
        {
            this.info = info;
            this.timeCost = timeCost;
            timeRemaining = (int)timeCost;
        }
    }
    public readonly Queue<Action> queue = new ();
}

public class ActionWait : IComponent
{
}

public static class ObjActionExtensions
{
    public static bool IsAttempting<TAction>(this Obj obj)
    {
        foreach (var attempt in obj.GetAllComponents<Attempting>())
        {
            if (attempt.action is TAction)
                return true;
        }

        return false;
    }

    public static TAction? GetFirstAttempt<TAction>(this Obj obj)
    {
        foreach (var attempt in obj.GetAllComponents<Attempting>())
        {
            if (attempt.action is TAction action)
                return action;
        }

        return default;
    }
    
    public static bool LastFinishedActionIs<TAction>(this Obj obj)
    {
        Actions.Action? lastAction = null;
        obj.GetComponent<Actions>()?.queue.TryPeek(out lastAction);
        if (lastAction == null) return false;
        return lastAction is { finished: true, info: TAction };
    }

    public static Actions.Action? GetLastFinishedAction(this Obj obj)
    {
        var queue = obj.GetComponent<Actions>()?.queue;
        if (queue?.Count > 0 && queue.Peek().finished)
            return queue.Peek();
        return null;
    } 
    
    public static void Attempt(this Obj obj, object action)
    {
        obj.AddComponent(new Attempting(action));
    }
    
    public static void Perform(this Obj obj, object action, uint timeCost)
    {
        var actions = obj.GetComponent<Actions>() ?? obj.AddComponent(new Actions());
        actions.queue.Enqueue(new Actions.Action(action, timeCost));
    }

    public static bool IsDoingSomething(this Obj obj)
    {
        return obj.GetComponent<Actions>()?.queue.Count > 0;
    } 
}

/// <summary>
/// Transforms PerformingAction into FinishedAction. Cleans AttemptingAction and FinishedAction.
/// </summary>
public class ActionSystem : GameSystem, IEarlyUpdateSystem, IUpdateSystem, ILateUpdateSystem
{
    public ActionSystem(World world) : base(world)
    {
    }

    public void EarlyUpdate(uint ticks)
    {
        var query = new Filter().With(o => o.IsAttempting<ActionWait>()).Apply(world.objects);
        foreach (var obj in query)
        {
            var wait = obj.GetFirstAttempt<ActionWait>();
            obj.Perform(wait, Consts.BIG_TICK);
        } 
            
        query = new Filter().With(o => o.GetComponent<Actions>()?.queue.Count > 0).With<ActionPoints>().Apply(world.objects);
        foreach (var obj in query)
        {
            var AP = obj.GetComponent<ActionPoints>();

            var lastAction = obj.GetComponent<Actions>().queue.Peek();

            AP.value++;
            var r = lastAction.timeRemaining;
            lastAction.timeRemaining -= AP.value;
            if (AP.value > r)
                AP.value -= r;
            else
                AP.value = 0;
            if (lastAction.timeRemaining <= 0)
                lastAction.finished = true;
        }
    }

    public void Update(uint ticks)
    {
    }

    public void LateUpdate(uint ticks)
    {
        var query = new Filter().With(o => o.HasComponent<Attempting>()).Apply(world.objects);
        foreach (var o in query)
        {
            o.RemoveComponentsOfType<Attempting>();
        }

        query = new Filter()
            .With(o =>
            {
                var actions = o.GetComponent<Actions>();
                if (actions == null) return false;
                actions.queue.TryPeek(out var lastAction);
                return lastAction?.finished == true;
            })
            .Apply(world.objects);
        foreach (var obj in query)
        {
            obj.GetComponent<Actions>().queue.Dequeue();
        }
    }
}