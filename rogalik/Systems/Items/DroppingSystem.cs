using System.Numerics;
using rogalik.Framework;
using rogalik.Rendering;
using rogalik.Systems.Walking;

namespace rogalik.Systems.Items;

public class IntentDrop : IComponent
{
    public Obj droppedObj;

    public IntentDrop(Obj droppedObj)
    {
        this.droppedObj = droppedObj;
    }
}

public class ActionDrop : IComponent
{
    public Obj droppedObj;
    public BigInteger startTime;
    public BigInteger endTime;

    public ActionDrop(Obj droppedObj, BigInteger startTime, BigInteger endTime)
    {
        this.droppedObj = droppedObj;
        this.startTime = startTime;
        this.endTime = endTime;
    }
}

public class DroppingSystem : GameSystem
{
    public DroppingSystem(World world) : base(world)
    {
    }

    public override void Update(uint ticks)
    {
        var intentFilter = new Filter().With<Position>().With<IntentDrop>().With(o => o.HasComponent<Inventory>()).Apply(world.objects);
        foreach (var obj in intentFilter)
        {
            var dropIntent = obj.GetComponent<IntentDrop>();
            var droppedObj = dropIntent.droppedObj;
            var startTime = world.time;
            var endTime = startTime + 0;
            obj.Add(new ActionDrop(droppedObj, startTime, endTime));
            UIData.AddLogMessage($"start {startTime} end {endTime}");
            obj.RemoveComponent(dropIntent);
        }
        
        var actionFilter = new Filter()
            .With<Position>()
            .With(o => o.GetComponent<ActionDrop>()?.endTime == world.time)
            .With(o => o.HasComponent<Inventory>())
            .Apply(world.objects);
        foreach (var obj in actionFilter)
        {
            var actionDrop = obj.GetComponent<ActionDrop>();
            var droppedObj = actionDrop.droppedObj;
            var inventory = obj.GetComponent<Inventory>();
            inventory.items.Remove(droppedObj);
            var ownerPoint = obj.GetComponent<Position>().point;
            var pos = droppedObj.GetComponent<Position>();
            if (pos == null)
                droppedObj.Add(new Position(ownerPoint));
            else
                pos.point = ownerPoint;
            obj.RemoveComponent(actionDrop);
        }
    }
}