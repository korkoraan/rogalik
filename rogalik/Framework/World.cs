using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using rogalik.AI;
using rogalik.Combat;
using rogalik.Items;
using rogalik.Rendering;
using rogalik.Walking;
using rogalik.WorldGen;

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
    
    public World(Game1 game)
    {
        _game = game;
        player = new Obj();
        _systems.Add(new WorldGenSystem(this));
        _systems.Add(new PlayerActivityMonitorSystem(this));
        _systems.Add(new SimpleMindSystem(this));
        _systems.Add(new WalkingSystem(this));
        _systems.Add(new VelocitySystem(this));
        _systems.Add(new MeleeSystem(this));
        _systems.Add(new PhysicalDmgSystem(this));
        _systems.Add(new DestructionSystem(this));
        _systems.Add(new PickingSystem(this));
        _systems.Add(new DroppingSystem(this));

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
    
    public IEnumerable<(Point, Obj)> GetVisibleObjects()
    {
        var point = player.GetComponent<Position>()?.point;
        return point == null ? default : InRange(player.GetComponent<Position>().point, 10);
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
            var weapon = new Obj();
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
