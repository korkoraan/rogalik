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

    public Camera(Game1 game, Obj obj)
    {
        pos = obj.GetComponent<Position>().point;
        game.world.FinishedUpdate += () => pos = obj.GetComponent<Position>().point;;
        game.input.InputActionsPressed += OnInputActionsPressed;
    }
    
    private void OnInputActionsPressed(List<InputAction> keys)
    {
        if(keys.Contains(InputAction.moveCameraUp))
            pos.y -= 1;
        if(keys.Contains(InputAction.moveCameraLeft))
            pos.x -= 1;
        if(keys.Contains(InputAction.moveCameraDown))
            pos.y += 1;
        if(keys.Contains(InputAction.moveCameraRight))
            pos.x += 1;
        if(keys.Contains(InputAction.zoomOut))
            zoom -= 0.1f;
        if(keys.Contains(InputAction.zoomIn))
            zoom += 0.1f;
    }
}