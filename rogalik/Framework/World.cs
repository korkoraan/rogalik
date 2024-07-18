using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using rogalik.Systems.Combat;
using rogalik.Framework.Map;
using rogalik.Systems.AI;
using rogalik.Systems.Items;
using rogalik.Systems.Time;
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
    public event WorldUpdatedHandler? FinishedUpdate;

    public delegate void PlayerDiedHandler();

    public event PlayerDiedHandler PlayerDied;
    
    public List<Obj> objects = new ();
    private List<GameSystem> _systems = new ();
    private IEnumerable<IEarlyUpdateSystem> _earlyUpdateSystems;
    private IEnumerable<IUpdateSystem> _updateSystems;
    private IEnumerable<ILateUpdateSystem> _lateUpdateSystems;
    public Obj player;
    public bool initialized { get; private set; }
    public bool paused { get; private set; }
    public readonly Map.MapBase map;
    public int lastUpdateCycleCount = 0;
    
    public World(Game1 game, Map.MapBase map)
    {
        _game = game;
        this.map = map;
        _systems = GameSystem.CreateSystems([
            typeof(WorldGenSystem),
            typeof(PlayerActivityMonitorSystem),
            typeof(SimpleMindSystem),
            typeof(WalkingSystem),
            typeof(VelocitySystem),
            typeof(MeleeSystem),
            typeof(PhysicalDmgSystem),
            typeof(DestructionSystem),
            typeof(DroppingSystem),
            typeof(ActionSystem),
        ], this);
        player = new Obj();

        Console.WriteLine($"world.systems:");
        foreach (var s in _systems)
        {
            Console.WriteLine($"    name:{s.GetType()}");
        }

        _earlyUpdateSystems = _systems.OfType<IEarlyUpdateSystem>();
        _updateSystems = _systems.OfType<IUpdateSystem>();
        _lateUpdateSystems = _systems.OfType<ILateUpdateSystem>();
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

    private void Update()
    {
        foreach (var system in _earlyUpdateSystems)
        {
            system.EarlyUpdate(1);
        }

        foreach (var system in _updateSystems)
        {
            system.Update(1);
        }

        foreach (var system in _lateUpdateSystems)
        {
            system.LateUpdate(1);
        }

        if (player.HasComponent<Destroyed>())
            PlayerDied?.Invoke();
            
        _time++;
    }
    private void Update(uint timeToUpdate)
    {
        for (var i = 0; i < timeToUpdate; ++i)
        {
            Update();
        }
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
        // return objects.Where(obj => obj.GetComponent<Position>()?.point == point);
        foreach (var o in objects.Where(obj => obj.GetComponent<Position>()?.point == point))
        {
            yield return o;
        }

        objects.Where(obj => obj.GetComponent<Position>()?.point == point);
    }

    public IEnumerable<(Point, IEnumerable<Obj>)> GetPointToObjInRadius(Point point, uint radius)
    {
        List<(Point, IEnumerable<Obj>)> result = [];
        for (var x = point.x - (int)radius; x <= point.x + radius; x++)
        {
            for (var y = point.y - (int)radius; y <= point.y + radius; y--)
            {
                result.Add((point, GetObjectsAt(new Point(x, y))));
            }
        }

        return result;
    }
    
    public IEnumerable<Obj> GetObjectsInRadius(Point point, uint radius)
    {
        List<Obj> result = [];
        for (var x = point.x - (int)radius; x <= point.x + radius; x++)
        {
            for (var y = point.y - (int)radius; y <= point.y + radius; y++)
            {
                foreach (var o in GetObjectsAt(new Point(x, y)))
                {
                    yield return o;
                }
            }
        }
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
                new Weapon(10, 5)
            };
            player.Attempt(new ActionHit(weapon, smthToHit));
        }
        else if( step != default )
        {
            player.Attempt(new ActionWalk(step));
        }
        else if(inputActions.Contains(InputAction.wait))
        {
            player.Attempt(new ActionWait());
        }
        
        if(!player.HasComponent<Attempting>()) return; 
        while (true)
        {
            Update(1);
            if (!player.IsDoingSomething() || player.HasComponent<Destroyed>())
            {
                FinishedUpdate?.Invoke();
                break;
            }
            
            lastUpdateCycleCount++;
            if (lastUpdateCycleCount > 100000)
                throw new Exception("Update cycle exceeded 100k iterations");
        }
        C.Print($"time: {time}, AP: {player.GetComponent<ActionPoints>().value}");
    }
}
