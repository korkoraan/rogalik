using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using rogalik.Common;
using rogalik.Components;
using rogalik.Rendering;

namespace rogalik.Objects;

public class Player : Obj
{
    private readonly Appearance _appearance;
    private readonly Legs _legs;

    public Player(Location location) : base(location)
    {
        _appearance = AddComponent(new Appearance(R.Tiles.playerWarrior));
        _legs = AddComponent(new Legs(1));
        Input.KeysPressed += OnKeysPressed;
    }

    private void OnKeysPressed(List<Keys> keys)
    {
        Point point = (0, 0, 0);
        if (keys.Contains(Keys.Up))
            point += (0, -1);
        if(keys.Contains(Keys.Down))
            point += (0, 1);
        if(keys.Contains(Keys.Left))
            point += (-1, 0);
        if(keys.Contains(Keys.Right))
            point += (1, 0);
        _legs.Move(point);
    }
}