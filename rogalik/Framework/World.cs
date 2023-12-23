using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using rogalik.Common;
using rogalik.Components;
using rogalik.Objects;
using rogalik.Rendering;
using Point = rogalik.Common.Point;

namespace rogalik.Framework;

public class Cell
{
    public readonly Point pos;
    public readonly List<Obj> contents;

    public Cell(Point pos)
    {
        this.pos = pos;
        contents = new List<Obj>();
    }
    
    public Cell(Point pos, List<Obj> contents)
    {
        this.pos = pos;
        this.contents = contents;
    }
}

public abstract class Location
{
    protected readonly World world;
    public Cell[][,] cells;
    public List<Mind> minds = new();
    public List<World.WorldAction> worldActions = new();
    public List<World.WorldAction> lastExecutedActions = new();

    public Location(World world)
    {
        this.world = world;
    }

    public bool TryMoving(Obj obj, Point point)
    {
        var x = obj.x;
        var y = obj.y;
        var z = obj.z;

        try
        {
            var cell = cells[z + point.z][x + point.x, y + point.y];
            var impassible = cell.contents.Find(o => o.HasComponent<Impassible>())?.GetComponent<Impassible>();
            if (impassible != null && impassible.enabled)
                return false;
            cells[z][x, y].contents.Remove(obj);
            obj.point += point;
            cell.contents.Add(obj);
        }
        catch (IndexOutOfRangeException)    
        {
            C.Print($"cannot move {obj.GetType()} from {obj.point} to {point}: destination is out of bounds");
            return false;
        }

        return true;
    }

    public void Spawn(Obj obj)
    {
        try
        {
            var cell = cells[obj.z][obj.x, obj.y];
            cell.contents.Add(obj);
            var mind = obj.GetComponent<Mind>();
            if(mind is not null)
                minds.Add(mind);
            obj.Init();
        }
        catch (IndexOutOfRangeException)    
        {
            C.Print($"cannot spawn {obj.GetType()} at {obj.point}: destination is out of bounds");
        }
    }

    public Cell this[Point point]
    {
        get
        {
            var z = point.z;
            var x = point.x;
            var y = point.y;
            if (cells.Length < z || z < 0 || cells[z].GetLength(0) < x || x < 0 || cells[z].GetLength(1) < y || y < 0)
                return null;
            return cells[z][x, y];
        }
    }

    public void Update()
    {
        foreach (var mind in minds)
        {
            if (mind.dead) continue;
            var action = mind.ChooseNextAction();
            if(action is null) continue;
            if(action.energyCost > mind.energyCurrent) continue;
            mind.energyCurrent -= action.energyCost;
            worldActions.Add(world.CreateWorldAction(action));
        }

        minds.RemoveAll(m => m.dead);

        var dueActions = worldActions.Where(w => w.endTime == world.time).ToList();
        foreach (var w in dueActions)
        {
            if (w.endTime == world.time)
            {
                w.successful = w.action.Apply();
                //TODO: should have some logic for spending and regenerating energy
                var actor = w.action.actor;
                if(actor is Player && w.action is MoveSelf && !w.successful)
                    UIData.messages.Add("failed to go");
                var mind = actor.GetComponent<Mind>();
                if (mind is not null)
                    mind.energyCurrent += w.action.energyCost;
            }
        }

        lastExecutedActions = dueActions;
        worldActions = worldActions.Except(dueActions).ToList();
    }
}

public class TestLevel : Location
{
    public TestLevel(World world, int size) : base(world)
    {
        cells = new Cell[1][,];
        cells[0] = new Cell[size, size];
        for (var x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                cells[0][x, y] = new Cell(new Point(x, y));
                Spawn(new Surface((x, y), this));
            } 
        }

        var lastWidth = 0;
        var lastStart = new Point(1, 10);
        var rooms = Rnd.NewInt(5, 10);
        for (int i = 0; i < rooms; i++)
        {
            var newStart = new Point(lastWidth + lastStart.x + 1, lastStart.y);
            var newWidth = (uint)Rnd.NewInt(4, 10);
            var room = new EmptyRoom(this,  newStart , newWidth, (uint)Rnd.NewInt(4,10));
        
            foreach (var o in room.objects)
            {
                if(o is null) continue;
                Spawn(o);
            }

            lastStart = newStart;
            lastWidth = (int)newWidth;
        }
        
        Spawn(new Goblin((0,0),this));
        Spawn(new Spirit((3,3),this));
    }
}

public sealed class World
{
    private List<Location> _locations;
    public PlayerMind playerMind;
    private BigInteger _time = 0;
    public BigInteger time => _time;
    public delegate void WorldUpdatedHandler(IEnumerable<WorldAction> lastExecutedActions);
    public event WorldUpdatedHandler WorldUpdated;

    private void Update(uint timeToUpdate)
    {
        var ticks = 0;
        while (ticks < timeToUpdate)
        {
            foreach (var location in _locations)
            {
                location.Update();
            }
            Tick();
            ticks++;
        }
        //TODO: should return only actions that player sees
        WorldUpdated?.Invoke(playerMind.owner.location.lastExecutedActions);
    }
    public World()
    {
        _locations = new() { new TestLevel(this, 100) };
        var player = new Player((5,5), _locations[0]);
        
        playerMind = player.GetComponent<PlayerMind>();
        _locations[0].Spawn(player);
        playerMind.PlayerDidSmth += OnPlayerDidSmth;
    }

    private void OnPlayerDidSmth(Action action)
    {
        Update(action.duration);
    }
    
    public Cell[,] GetVisibleCells()
    {
        return _locations[0].cells[0];
    }

    private void Tick()
    {
        _time++;
    }

    public WorldAction CreateWorldAction(Action action)
    {
        return new WorldAction(action, _time);
    }
    
    public class WorldAction
    {
        public readonly Action action;
        public readonly BigInteger startTime;
        public bool successful;
        public BigInteger endTime => startTime + action.duration - 1; 
    
        public WorldAction(Action action, BigInteger startTime)
        {
            this.action = action;
            this.startTime = startTime;
        }
    }
}