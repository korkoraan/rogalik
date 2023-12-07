using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using rogalik.Common;
using rogalik.Components;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Objects;

public class PlayerMind : Mind
{
    public delegate void PlayerDidSmthHandler(Action action);
    public event PlayerDidSmthHandler PlayerDidSmth = delegate { };   
    private Action _selectedAction;
    private Legs _legs;

    public PlayerMind()
    {
        Input.KeysPressed += OnKeysPressed;
    }
    
    public override void Init()
    {
        _legs = owner.GetComponent<Legs>();
    }

    public override Action ChooseNextAction()
    {
        return _selectedAction;
    }
    
    private void OnKeysPressed(List<Keys> keys)
    {
        if (keys.Contains(Keys.S))
        {
            _selectedAction = new DoNothing(actor: owner);
            PlayerDidSmth?.Invoke(_selectedAction);
        }
        else
        {
            Point step = (0, 0, 0);
            if (keys.Contains(Keys.Up))
                step += (0, -1);
            if(keys.Contains(Keys.Down))
                step += (0, 1);
            if(keys.Contains(Keys.Left))
                step += (-1, 0);
            if(keys.Contains(Keys.Right))
                step += (1, 0);
            var enemy = owner.location[owner.point + step]?.contents.Find(o => o is Goblin);
            _selectedAction = enemy is null ? _legs.StepTo(step) : _legs.Kick(enemy);
            PlayerDidSmth?.Invoke(_selectedAction);
        }

        _selectedAction = null;
    }
}

public class Player : Obj
{
    public Player(Location location) : base(location)
    {
        AddComponent(new Appearance(R.Tiles.playerWarrior));
        AddComponent(new Legs(2));
        AddComponent(new PlayerMind());
    }
}