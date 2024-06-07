using System.Collections.Generic;
using rogalik.Common;
using rogalik.Framework;
using rogalik.Rendering;
using rogalik.Rendering.UIElements;

namespace rogalik;

public static class Global
{
    public static List<Ability> GetPlayerAbilities()
    {
        return new List<Ability>
        {
            new ("Deadly sneeze", "Sneeze with your mouth closed.", "active",R.Icons.Abilities.deadlySneeze, InputAction.ability1),
            new ("Fireball", "Throw a ball of fire at a point which explodes upon impact.", "target point", R.Icons.Abilities.fireball, InputAction.ability2),
            new ("Sneak", "Enter a stealth mode, making you harder to notice.","toggle", R.Icons.Abilities.sneak, InputAction.ability3),
            new ("Devour", "Eat a selected corpse.","target unit", R.Icons.Abilities.devour, InputAction.ability4),
            new ("Hell fire", "Summon an area of engulfing unholy flame.","target unit", R.Icons.Abilities.hellFire, InputAction.ability5),
            new ("Frost beam", "Cast a beam of energy that frosts everything in it's way.","target unit", R.Icons.Abilities.frostBeam, InputAction.ability6),
            new ("Mighty kick", "Kick anything with an unmatched fury.", "target point", R.Icons.Abilities.mightyKick, InputAction.ability7),
            new ("Disarm", "Attempt to strike out opponents weapon from him", "target unit", R.Icons.Abilities.disarm),
            new ("Shield guard", "Get in the all-defense position, drastically increasing your AC, but making you unable to attack", "toggle", R.Icons.Abilities.shieldGuard),
        };
    }
    
    public static IEnumerable<EquipmentMenu.Slot> GetPlayerSlots()
    {
        var one = new[]
        {
            new EquipmentMenu.Slot("head", 1, 0, SlotType.head),
            new EquipmentMenu.Slot("right hand", 2, 1, SlotType.hand),
            new EquipmentMenu.Slot("left hand", 0, 1, SlotType.hand),
            new EquipmentMenu.Slot("body", 1, 1, SlotType.body),
            new EquipmentMenu.Slot("legs", 1, 2, SlotType.legs),
            new EquipmentMenu.Slot("boots", 1, 3, SlotType.feet),
        };
        var two = new[]
        {
            new EquipmentMenu.Slot("head", 1, 0, SlotType.head),
            new EquipmentMenu.Slot("right hand", 2, 1, SlotType.hand),
            new EquipmentMenu.Slot("left hand", 0, 1, SlotType.hand),
            new EquipmentMenu.Slot("body", 1, 1, SlotType.body),
            new EquipmentMenu.Slot("legs", 1, 2, SlotType.legs),
            new EquipmentMenu.Slot("boots", 1, 3, SlotType.feet),
        };

        var rnd = Rnd.NewInt(1, 3);
        return rnd switch
        {
            1 => one,
            _ => two,
        };
    }

    public static IEnumerable<EquipmentMenu.Item> GetPlayerItems()
    {
        var items = new EquipmentMenu.Item[]
        {
            new EquipmentMenu.Weapon("iron sword", SlotType.hand, R.Icons.Items.sword, dmg: 10),
            new EquipmentMenu.Weapon("wooden staff", SlotType.hand, R.Icons.Items.staff, dmg: 5),
            new EquipmentMenu.Armor("leather armor", SlotType.body, R.Icons.Items.armorLeather, armorPts: 3),
            new EquipmentMenu.Armor("iron armor", SlotType.body, R.Icons.Items.armorIron, armorPts: 7),
            new EquipmentMenu.Armor("plate armor", SlotType.body, R.Icons.Items.armorPlate, armorPts: 10),
            new EquipmentMenu.Armor("iron helmet", SlotType.head, R.Icons.Items.helmetIron, armorPts: 7),
            new EquipmentMenu.Armor("leather helmet", SlotType.head, R.Icons.Items.helmetLeather, armorPts: 2),
            new EquipmentMenu.Armor("plate helmet", SlotType.head, R.Icons.Items.helmetPlate, armorPts: 10),
        };
        return items[Rnd.NewInt(0, items.Length)..];
    }
    
    public static StatsMenu.PlayerData GetPlayerData()
    {
        return new StatsMenu.PlayerData
        {
            name = "Kevin",
            race = "gremlin",
            gender = "female",
            lvl = 5,
            skillPts = 1,
            attributes = new []
            {
                new StatsMenu.Attribute("strength", 8, texture: R.Icons.Stats.strength),
                new StatsMenu.Attribute("agility", 7, "ability to move quickly and easily", R.Icons.Stats.agility),
                new StatsMenu.Attribute("will", 21, "mental strength and determination to stay focused", R.Icons.Stats.will),
            },
        };
    }

    public static HealthStatus GetPlayerHealthStatus()
    {
        return new HealthStatus
        {
            healthPtsLimit = 200,
            healthPts = 100
        };
    }

    public struct HealthStatus
    {
        public uint healthPts;
        public uint healthPtsLimit;
    }
}