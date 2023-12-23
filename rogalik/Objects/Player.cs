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
    private Hand _hand;
    private string _comboActionName = "";

    public PlayerMind()
    {
        Input.KeysPressed += OnKeysPressed;
    }
    
    public override void Init()
    {
        _legs = owner.GetComponent<Legs>();
        _hand = owner.GetComponent<Hand>();
    }

    public override Action ChooseNextAction()
    {
        return _selectedAction;
    }
    
    private void OnKeysPressed(List<Keys> keys)
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
        var cell = owner.location[owner.point + step];
        
        if (keys.Contains(Keys.O))
        {
            _comboActionName = "open";
            return;
        }
        
        if (keys.Contains(Keys.Space))
        {
            _selectedAction = new DoNothing(actor: owner);
        }
        
        else if (_comboActionName != "")
        {
            switch (_comboActionName)
            {
                case "open":
                    if (cell.contents.Find(o => o is IOpenable) is not IOpenable openable)
                        UIData.messages.Add("nothing to open");
                    else
                        _selectedAction = _hand.Open(openable);
                    break;
            }

            _comboActionName = "";
        }
        else
        {
            var enemy = owner.location[owner.point + step]?.contents.Find(o => o is Goblin);
            var ded = false;
            if (enemy != null)
                ded = enemy.GetComponent<Mind>().dead;
            _selectedAction = enemy is null || ded ? _legs.StepTo(step) : _legs.Kick(enemy);
        }
        
        if(_selectedAction == null) return;
        PlayerDidSmth?.Invoke(_selectedAction);
        _selectedAction = null;

    }
}

public class Player : Obj
{
    public Player(Point point, Location location) : base(point, location)
    {
        AddComponent(new Appearance(R.Tiles.playerWarrior));
        AddComponent(new Legs(2));
        AddComponent(new PlayerMind());
        AddComponent(new Hand());
    }
}