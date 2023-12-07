using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using rogalik.Common;
using rogalik.Components;
using rogalik.Objects;
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

    public Location(World world)
    {
        this.world = world;
    }

    public void TryMoving(Obj obj, Point point)
    {
        var x = obj.cell.pos.x;
        var y = obj.cell.pos.y;
        var z = obj.cell.pos.z;

        try
        {
            var cell = cells[z + point.z][x + point.x, y + point.y];
            cells[z][x, y].contents.Remove(obj);
            cell.contents.Add(obj);
            obj.cell = cell;
        }
        catch (IndexOutOfRangeException)    
        {
            C.Print($"cannot move {obj.GetType()} from {obj.cell.pos} to {point}: destination is out of bounds");
        }
    }

    public void Spawn(Obj obj, Point point)
    {
        try
        {
            var cell = cells[point.z][point.x, point.y];
            cell.contents.Add(obj);
            obj.cell = cell;
            var mind = obj.GetComponent<Mind>();
            if(mind is not null)
                minds.Add(mind);
            obj.Init();
            
            if (obj is not Surface)
                C.Print($"spawned {obj.GetType()} at {point}");
        }
        catch (IndexOutOfRangeException)    
        {
            C.Print($"cannot spawn {obj.GetType()} at {point}: destination is out of bounds");
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
                w.action.Apply();
                //TODO: should have some logic for spending and regenerating energy
                var actor = w.action.actor;
                var mind = actor.GetComponent<Mind>();
                if (mind is not null)
                    mind.energyCurrent += w.action.energyCost;
            }
        }
        
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
                Spawn(new Surface(this), (x, y));
            } 
        }
        
        Spawn(new Goblin(this), (0,0));
        Spawn(new Spirit(this), (3,3));
    }
}

public sealed class World
{
    private List<Location> _locations;
    private PlayerMind _playerMind;
    private BigInteger _time = 0;
    public BigInteger time => _time;

    private void Update(uint timeToUpdate)
    {
        var ticks = 0;
        var s = "" + _time;
        while (ticks < timeToUpdate)
        {
            foreach (var location in _locations)
            {
                location.Update();
            }
            Tick();
            ticks++;
        }

        s += "->" + _time;
        C.Print(s);
    }
    public World()
    {
        _locations = new() { new TestLevel(this, 10) };
        var player = new Player(_locations[0]);
        
        _playerMind = player.GetComponent<PlayerMind>();
        _locations[0].Spawn(player, (5,5));
        _playerMind.PlayerDidSmth += OnPlayerDidSmth;
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
        public BigInteger endTime => startTime + action.duration - 1; 
    
        public WorldAction(Action action, BigInteger startTime)
        {
            this.action = action;
            this.startTime = startTime;
        }
    }
}