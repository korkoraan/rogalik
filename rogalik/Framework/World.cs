using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using rogalik.Systems.Combat;
using rogalik.Framework.Map;
using rogalik.Rendering;
using rogalik.Systems.AI;
using rogalik.Systems.Common;
using rogalik.Systems.Items;
using rogalik.Systems.Walking;
using rogalik.Systems.WorldGen;

namespace rogalik.Framework;

public sealed class World
{
    private Game1 _game;
    private BigInteger _time = 0;
    public BigInteger time => _time;
    public delegate void WorldUpdatedHandler();

    public event WorldUpdatedHandler StartedUpdate;
    public event WorldUpdatedHandler FinishedUpdate;
    
    public List<Obj> objects = new ();
    private List<GameSystem> _systems = new ();
    public Obj player;
    public bool initialized { get; private set; }
    public bool paused { get; private set; }
    public readonly Map.MapBase map;
    
    public World(Game1 game, Map.MapBase map)
    {
        _game = game;
        this.map = map;
        player = new Obj();
        _systems = GameSystem.CreateSystems(new List<Type>()
        {
            typeof(WorldGenSystem),
            typeof(PlayerActivityMonitorSystem),
            typeof(SimpleMindSystem),
            typeof(WalkingSystem),
            typeof(VelocitySystem),
            typeof(MeleeSystem),
            typeof(PhysicalDmgSystem),
            typeof(DestructionSystem),
            typeof(DroppingSystem),
        }, this);

        Console.WriteLine($"world.systems:");
        foreach (var s in _systems)
        {
            Console.WriteLine($"    name:{s.GetType()}");
        }
        
        foreach (var gameSystem in _systems.FindAll(s => s is IInitSystem))
        {
            (gameSystem as IInitSystem)?.Init();
        }
        
        initialized = true;
        _game.input.InputActionsPressed += OnActionPressed;
    }

    public IEnumerable<(Point, Obj)> InRange(Point center, int range)
    {
        foreach (var obj in objects)
        {
            var pos = obj.GetComponent<Position>();
            if (pos is null) continue;
            if (pos.point.InRange(center - (int)range, center + (int)range))
                yield return (pos.point, obj);
        }
    }
    
    private void Update(uint timeToUpdate)
    {
        if(paused) return;
        _time += timeToUpdate;
        foreach (var system in _systems)
        {
            system.Update(timeToUpdate);
        }
        FinishedUpdate?.Invoke();
    }

    private const ushort VisibleRangeDefault = 10;
    public IEnumerable<(Point, Obj)> GetVisibleObjects(Obj watcher, ushort radius = VisibleRangeDefault)
    {
        var point = watcher.GetComponent<Position>()?.point;
        return (point == null ? default : InRange((Point)point, radius))!; // TODO: solve this!
    }

    public Map.MapPiece? GetVisibleMap(Obj watcher, ushort radius = VisibleRangeDefault)
    {
        var point = watcher.GetComponent<Position>()?.point;
        if (point == null) 
            return null;
        var x1 = Math.Max(point.Value.x - VisibleRangeDefault, 0);
        var y1 = Math.Max(point.Value.y - VisibleRangeDefault, 0);
        var x2 = (uint)Math.Min(point.Value.x + VisibleRangeDefault, map.width);
        var y2 = (uint)Math.Min(point.Value.y + VisibleRangeDefault, map.height);
        return map.GetArea(new Area(x1, y1, (uint)(x2 - x1), (uint)(y2 - y1)));
    }


    public IEnumerable<Obj> GetObjectsAt(Point point)
    {
        return objects.Where(obj => obj.GetComponent<Position>()?.point == point);
    }

    public void Pause()
    {
        paused = true;
    }

    public void Resume()
    {
        paused = false;
    }

    private void OnActionPressed(List<InputAction> inputActions)
    {
        var didSmth = false;
        var playerPos = player.GetComponent<Position>();
        if(playerPos == null) return;
        
        Point step = default;
        if (inputActions.Contains(InputAction.goUp))
            step = (0, -1);
        if (inputActions.Contains(InputAction.goDown))
            step = (0, 1);
        if (inputActions.Contains(InputAction.goLeft))
            step = (-1, 0);
        if (inputActions.Contains(InputAction.goRight))
            step = (1, 0);
        if (inputActions.Contains(InputAction.goUpRight))
            step = (1, -1);
        if (inputActions.Contains(InputAction.goDownRight))
            step = (1, 1);
        if (inputActions.Contains(InputAction.goUpLeft))
            step = (-1, -1);
        if (inputActions.Contains(InputAction.goDownLeft))
            step = (-1, 1);
        var smthToHit = GetObjectsAt(playerPos.point + step).ToList().Find(o => o.HasComponent<Mind>());
        if (smthToHit != null)
        {
            var weapon = new Obj
            {
                new Volume(10),
                new Density(3)
            };
            player.AddComponent(new ActionHit(weapon, smthToHit));
            didSmth = true;
        }
        else if( step != default )
        {
            player.AddComponent(new ActionWalk(step));
            didSmth = true;
        }
        // else if (step != (0, 0))
        // {
        //     player.AddComponent(new ActionWalk(step));
        //     didSmth = true;
        // }
        // else if (keys.Contains(Keys.P))
        // {
        //     var items = _world.GetObjectsAt(playerPos.point).ToList();
        //     items.Remove(player);
        //     items.RemoveAll(o => o.GetComponent<Appearance>()?.description == "rock surface");
        //     switch (items.Count)
        //     {
        //         case 1:
        //             player.AddComponent(new ActionPick(items.First()));
        //             UIData.AddLogMessage($"you pick up a {items.First()}");
        //             break;
        //         case > 1:
        //             _game.renderer.ChooseItemToPick(items);
        //             break;
        //         default:
        //             UIData.AddLogMessage("nothing to pick up");
        //             break;
        //     }
        // }
        // else if (keys.Contains(Keys.D))
        // {
        // }
        //
        if(didSmth)
            Update(1);
    }
}
