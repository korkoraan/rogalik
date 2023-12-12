using System;
using rogalik.Common;
using rogalik.Framework;

namespace rogalik.Objects;


public class EmptyRoom
{
    public Obj[,] objects;
    public EmptyRoom(Location location, Point start, uint width, uint height)
    {
        if (width < 1 || height < 1)
            throw new Exception("room cannot be size 0");
        objects = new Obj[width, height];
        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(x == 0 || y == 0 || x == width -1 || y == height - 1)
                    objects[x, y] = (new Wall(start + (x,y), location));
            }
        }

        var r = Rnd.NewInt(0, 3);
        switch (r)
        {
            case 0:
                var y = Rnd.NewInt(1, (int)height - 1);
                objects[0, y] = new Door((0, y) + start, location) ;
                break;
            case 1:
                var y1 = Rnd.NewInt(1, (int)height - 1);
                objects[width - 1, y1] = new Door(((int)width - 1, y1) + start, location);
                break;
            case 2:
                var x = Rnd.NewInt(1, (int)width - 1);
                objects[x, 0] = new Door((x, 0) + start, location);
                break;
            case 3:
                var x1 = Rnd.NewInt(1, (int)width - 1);
                objects[x1, height - 1] = new Door((x1, (int)height - 1) + start, location);
                break;
        }
    }
}