using System.Collections;
using System.Collections.Generic;
using rogalik.Framework;
using rogalik.Systems.Walking;

namespace rogalik.Systems.Items;

public class Gear : IComponent
{
    public List<Obj> items = new ();
}

public class Inventory : IComponent, IEnumerable
{
    public List<Obj> items = new ();

    public void Add(Obj obj)
    {
        items.Add(obj);
    }

    public IEnumerator GetEnumerator()
    {
        return items.GetEnumerator();
    }
}

public class ActionPick : IComponent
{
    public Obj item;
    public Inventory inventory;

    public ActionPick(Obj item)
    {
        this.item = item;
    }
}

public class PickingSystem : GameSystem, IUpdateSystem
{
    public PickingSystem(World world) : base(world)
    {
    }

    public void Update(uint ticks)
    {
        var filter = new Filter()
            .With<ActionPick>()
            .With<Inventory>()
            .Apply(world.objects);

        foreach (var obj in filter)
        {
            var pick = obj.GetComponent<ActionPick>();
            var inventory = obj.GetComponent<Inventory>();
            
            inventory.items.Add(pick.item);
            var position = pick.item.GetComponent<Position>();
            pick.item.RemoveComponent(position);
        }
    }
}