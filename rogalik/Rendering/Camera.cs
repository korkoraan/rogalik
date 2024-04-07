using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using rogalik.Common;
using rogalik.Framework;
using rogalik.Walking;
using Point = rogalik.Framework.Point;

namespace rogalik.Rendering;

class Camera
{
    public Point pos;
    public float zoom = 3f;

    public Camera(Game1 game, Point pos)
    {
        this.pos = new Point(pos.x, pos.y);
        game.input.InputActionsPressed += OnInputActionsPressed;
    }

    public Camera(World world, Obj obj)
    {
        pos = obj.GetComponent<Position>().point;
        world.FinishedUpdate += () => pos = obj.GetComponent<Position>().point;;
    }
    
    private void OnInputActionsPressed(List<InputAction> keys)
    {
        if(keys.Contains(InputAction.goUp))
            pos.y -= 1;
        if(keys.Contains(InputAction.goLeft))
            pos.x -= 1;
        if(keys.Contains(InputAction.goDown))
            pos.y += 1;
        if(keys.Contains(InputAction.goRight))
            pos.x += 1;
        //TODO: add zoom to input bindings
        // if(keys.Contains(Keys.PageUp))
            // zoom -= 0.1f;
        // if(keys.Contains(Keys.PageDown))
            // zoom += 0.1f;
    }
}