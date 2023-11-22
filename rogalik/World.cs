using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using rogalik.Common;
using rogalik.Components;
using rogalik.Objects;
using Point = rogalik.Common.Point;

namespace rogalik;

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
    public Cell[][,] cells;

    public void TryMoving(Obj obj, Point point)
    {
        var x = obj.cell.pos.x;
        var y = obj.cell.pos.y;
        var z = obj.cell.pos.z;

        try
        {
            cells[z][x, y].contents.Remove(obj);
            Point oldPos = (x, y, z);
            var cell = cells[z + point.z][x + point.x, y + point.y];
            cell.contents.Add(obj);
            obj.cell = cell;
            C.Print($"moved {obj.GetType()} from {oldPos} to {obj.cell.pos}");
        }
        catch (IndexOutOfRangeException)    
        {
            C.Print($"cannot move {obj.GetType()} from {obj.cell.pos} to {point}: destination is out of bounds");
        }
    }

    protected void Spawn(Obj obj, Point point)
    {
        try
        {
            var cell = cells[point.z][point.x, point.y];
            cell.contents.Add(obj);
            obj.cell = cell;
            if(obj is not Surface)
                C.Print($"spawned {obj.GetType()} at {point}");
        }
        catch (IndexOutOfRangeException)    
        {
            C.Print($"cannot spawn {obj.GetType()} at {point}: destination is out of bounds");
        }
    }
}

public class TestLevel : Location
{
    public TestLevel(int size)
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
        Spawn(new Player(this), (0,0));
    }
}

public class World
{
    private List<Location> _locations; 
    public World()
    {
        _locations = new() { new TestLevel(10) };
    }
    
    public Cell[,] GetVisibleCells()
    {
        return _locations[0].cells[0];
    }
}