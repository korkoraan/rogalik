using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using rogalik.Common;

namespace rogalik.Rendering;

class Camera
{
    public Point pos;
    public float zoom = 3f;

    public Camera(Point pos)
    {
        this.pos = pos;
        Input.KeysPressed += OnKeysPressed;
    }
    
    private void OnKeysPressed(List<Keys> keys)
    {
        //TODO: this is hardcode needs fixing
        if(keys.Contains(Keys.NumPad8))
            pos.y -= 12;
        if(keys.Contains(Keys.NumPad4))
            pos.x -= 12;
        if(keys.Contains(Keys.NumPad2))
            pos.y += 12;
        if(keys.Contains(Keys.NumPad6))
            pos.x += 12;
        if(keys.Contains(Keys.PageUp))
            zoom -= 0.1f;
        if(keys.Contains(Keys.PageDown))
            zoom += 0.1f;
    }

    public void Move(Point point)
    {
        pos.x += point.x * 12;
        pos.y += point.y * 12;
    }
}