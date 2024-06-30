using rogalik.Systems.Combat;
using rogalik.Framework;
using rogalik.Rendering;
using rogalik.Systems.Abilities;
using rogalik.Systems.AI;
using rogalik.Systems.Common;
using rogalik.Systems.Items;
using rogalik.Systems.Walking;

namespace rogalik.Systems.WorldGen;

public class WorldGenSystem : GameSystem, IInitSystem
{
    public WorldGenSystem(World world) : base(world)
    {
    }

    public override void Update(uint ticks)
    {
    }

    public void Init()
    {
        for (int i = 0; i < 100; i++)
        {
            var x = Rnd.NewInt(0, 100);
            var y = Rnd.NewInt(0, 100);
            Spawn(CreateRandomMonster(), (x, y));
        }

        world.player = CreateHumanoid();
        world.player.AddComponent(new Appearance(R.Tiles.playerWarrior, "human"));
        Spawn(world.player, (50, 50));
        Spawn(Sword(), (51, 51));
    }
    
    private void Spawn(Obj obj, Point point)
    {
        world.objects.Add(obj);
        obj.AddComponent(new Position(point));
    }

    private Obj CreateHumanoid()
    {
        var obj = new Obj
        {
            new Volume(1),
            new Density(1),
            new Health(100),
            new Gear(),
            new Inventory
            {
            },
            new PossessedAbilities
            {
                BestAbility()
            }
        };

        return obj;
    }

    private Obj CreateRandomMonster()
    {
        var monster = CreateHumanoid();
        var textures = new[]
        {
            R.Tiles.treant,
            R.Tiles.goblinUnarmed,
            R.Tiles.spirit,
            R.Tiles.stoneGolem,
        };
        var t = textures.Random();
        monster.AddComponent(new Appearance(t, monster.ToString()));
        monster.AddComponent(new Mind());
        return monster;
    }

    private Obj CreateGarbage()
    {
        return new Obj
        {
            new Volume(1),
            new Density(1),
            new Health(100),
            new Appearance(R.Tiles.cat, "piece of shit")
        };
    }

    private Obj Sword()
    {
        var sword = new Obj
        {
            new Volume(1),
            new Density(1),
            new Health(1),
            new Appearance(R.Tiles.sword, "sword")
        };
        world.objects.Add(sword);
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