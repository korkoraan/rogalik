using System.Linq;
using rogalik.Systems.Combat;
using rogalik.Framework;
using rogalik.Rendering;
using rogalik.Systems.Abilities;
using rogalik.Systems.AI;
using rogalik.Systems.Common;
using rogalik.Systems.Items;
using rogalik.Systems.Time;
using rogalik.Systems.Walking;

namespace rogalik.Systems.WorldGen;

public class WorldGenSystem : GameSystem, IInitSystem
{
    public WorldGenSystem(World world) : base(world)
    {
    }

    public void Init()
    {
        // for (int i = 0; i < 1000; i++)
        // {
            // var x = Rnd.NewInt(0, 100);
            // var y = Rnd.NewInt(0, 100);
            // Spawn(CreateRandomMonster(), (x, y));
        // }

        world.player = Player();
        world.player.AddComponent(new Appearance(R.Tiles.playerWarrior, "human"));
        Spawn(world.player, (50, 50));
        Spawn(RandomMonster(), (50, 52));
        // Spawn(Sword(), (51, 51));
    }
    
    private void Spawn(Obj obj, Point point)
    {
        world.objects.Add(obj);
        obj.AddComponent(new Position(point));
    }

    private Obj Player()
    {
        var obj = new Obj
        {
            new Health(100),
            new Gear(),
            new Inventory
            {
            },
            new BasicAttributes(1, 2),
            new ActionPoints(Consts.BIG_TICK * 2, 0)
        };

        return obj;
    }

    private Obj RandomMonster()
    {
        var monster = new Obj
        {
            new Health(100),
            new Gear(),
            new Inventory
            {
            },
            new BasicAttributes(1, 1),
            new ActionPoints(Consts.BIG_TICK * 2, 0)
        };
        var textures = new[]
        {
            R.Tiles.treant,
            R.Tiles.goblinUnarmed,
            R.Tiles.spirit,
            R.Tiles.stoneGolem,
        };
        var t = textures.Random();
        monster.AddComponent(new Appearance(t, t.ToString().Split('/').Last()));
        monster.AddComponent(new Mind());
        return monster;
    }

    private Obj Sword()
    {
        var sword = new Obj
        {
            new Health(1),
            new Appearance(R.Tiles.sword, "sword")
        };
        return sword;
    }

    private Obj BestAbility()
    {
        return new Obj { new Ability("Angst", "Feel the weight of the world on your shoulders") };
    }
    
    private void MakeRigid(Obj obj)
    {
        // var 
        // obj.AddComponent(new Rigid());
    }
} 