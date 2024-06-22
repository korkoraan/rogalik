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

    // public class EmptyRoom
// {
//     // public Obj[,] objects;
//     public List<(Obj, Point)> objects;
//     public EmptyRoom(Location location, Point start, uint width, uint height)
//     {
//         if (width < 1 || height < 1)
//             throw new Exception("room cannot be size 0");
//         objects = new Obj[width, height];
//         for (var x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 if(x == 0 || y == 0 || x == width -1 || y == height - 1)
//                     objects[x, y] = (new Wall(start + (x,y), location));
//             }
//         }
//
//         var r = Rnd.NewInt(0, 3);
//         switch (r)
//         {
//             case 0:
//                 var y = Rnd.NewInt(1, (int)height - 1);
//                 objects[0, y] = new Door((0, y) + start, location) ;
//                 break;
//             case 1:
//                 var y1 = Rnd.NewInt(1, (int)height - 1);
//                 objects[width - 1, y1] = new Door(((int)width - 1, y1) + start, location);
//                 break;
//             case 2:
//                 var x = Rnd.NewInt(1, (int)width - 1);
//                 objects[x, 0] = new Door((x, 0) + start, location);
//                 break;
//             case 3:
//                 var x1 = Rnd.NewInt(1, (int)width - 1);
//                 objects[x1, height - 1] = new Door((x1, (int)height - 1) + start, location);
//                 break;
//         }
//     }
// }

    public void Init()
    {
        // for (int x = 0; x < 100; x++)
        // {
        //     for (int y = 0; y < 100; y++)
        //     {
        //         var surface = new Obj();
        //         surface.AddComponent(new Appearance(R.Tiles.surfaceRock.Random(), "rock surface"));
        //         Spawn(surface, (x,y));
        //     }
        // }

        var goblin = CreateHumanoid();
        goblin.AddComponent(new Appearance(R.Tiles.goblinUnarmed, "goblin"));
        goblin.AddComponent(new Mind());
        Spawn(goblin, (55, 55));

        world.player = CreateHumanoid();
        world.player.AddComponent(new Appearance(R.Tiles.playerWarrior, "human"));
        Spawn(world.player, (50, 50));
        Spawn(Sword(), (51, 51));
        // Spawn(Sword(), (11, 11));
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
            new Rigid(100),
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

    private Obj CreateGarbage()
    {
        return new Obj
        {
            new Volume(1),
            new Density(1),
            new Rigid(100),
            new Appearance(R.Tiles.cat, "piece of shit")
        };
    }

    private Obj Sword()
    {
        var sword = new Obj
        {
            new Volume(1),
            new Density(1),
            new Rigid(1),
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