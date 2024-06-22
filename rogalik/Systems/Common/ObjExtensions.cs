using System.Collections.Generic;
using rogalik.Framework;
using rogalik.Systems.Abilities;
using rogalik.Systems.Items;

namespace rogalik.Systems.Common;

public static class ObjExtensions
{
    public static IEnumerable<Obj> InventoryItems(this Obj obj)
    {
        var inventory = obj.GetComponent<Inventory>();
        foreach (var item in inventory.items)
        {
            yield return item;
        }
        // var containers = obj.GetComponent<Gear>()?.items.FindAll(o => o.HasComponent<Container>()) ?? new List<Obj>();
        // var selfContainer = obj.GetComponent<Container>();
        // if(selfContainer != null) containers?.Add(obj); 
        // foreach (var container in containers)
        // {
        // yield return container.GetComponent<Container>();
        // }
    }

    public static IEnumerable<Obj> Abilities(this Obj obj)
    {
        var abilities = obj.GetComponent<PossessedAbilities>();

        if (abilities?.items == null) yield break;
        foreach (var abilityObj in abilities.items)
        {
            yield return abilityObj;
        }
    }
}