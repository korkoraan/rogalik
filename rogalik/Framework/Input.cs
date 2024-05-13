using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace rogalik.Framework;

public interface IInputListener
{
    public void OnInputActionsPressed(List<InputAction> keys);
    
    public delegate void ControlRelinquishedHandler();
    
    public event ControlRelinquishedHandler ControlRelinquished;
}

public enum InputAction
{
    goDown,
    goUp,
    goLeft,
    goRight,
    pickUp,
    drop,
    goDownRight,
    goUpRight,
    goDownLeft,
    goUpLeft,
    toggleInventory,
    zoomOut,
    zoomIn,
    moveCameraUp,
    moveCameraDown,
    moveCameraLeft,
    moveCameraRight,
    toggleAbilitiesMenu,
    exitWindow,
    ability1,
    ability2,
    ability3,
    ability4,
    ability5,
    ability6,
    ability7,
    ability8,
    ability9,
    ability10
}

/// <summary>
/// Subscribe to InputActionsPressed to listen for input actions.
/// <param name="_delay">number of milliseconds waited for player to enter a key combination</param>
/// </summary>
public class Input
{
    private Game1 _game;
    private World _world;
    public delegate void InputActionsPressedHandler(List<InputAction> key);
    public event InputActionsPressedHandler InputActionsPressed;

    public delegate void MouseClickHandler(MouseState mouseState);
    public event MouseClickHandler OnMouseClick;

    private List<Keys> _pressedKeys = new ();
    private List<Keys> _unpressedKeys = new ();
    private bool _isUnpressingKeys;
    private double _delay;
    private double _remainingDelay;
    private Mode _mode;
    
    private IInputListener _soloListener;
    private InputActionsPressedHandler _listenersOnHold;

    private enum Mode
    {
        ui,
        game,
    }

    public Input(Game1 game)
    {
        _game = game;
        _delay = 50;
        _remainingDelay = _delay;
    }

    public void Init()
    {
        _world = _game.world;
    }
    
    public void Update(GameTime gameTime)
    {
        var ks = Keyboard.GetState();
        var nkeys = ks.GetPressedKeys();
        foreach (var key in nkeys)
        {
            if (!_pressedKeys.Contains(key))
            {
                _pressedKeys.Add(key);
            }
        }
    
        foreach (var key in _pressedKeys.Where(key => ks.IsKeyUp(key)))
        {
            _unpressedKeys.Add(key);
        }
        _pressedKeys.RemoveAll(k => ks.IsKeyUp(k));
    
        if (_unpressedKeys.Count > 0)
        {
            _isUnpressingKeys = true;
        }
    
        if (_isUnpressingKeys)
        {
            _remainingDelay -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    
        if (_remainingDelay <= 0)
        {
            InvokeInputActions(_unpressedKeys);
            _unpressedKeys.Clear();
            _remainingDelay = _delay;
            _isUnpressingKeys = false;
        }

        var ms = Mouse.GetState();
        if (ms.RightButton == ButtonState.Pressed || ms.LeftButton == ButtonState.Pressed || ms.MiddleButton == ButtonState.Pressed)
        {
            OnMouseClick?.Invoke(ms);
        }
            
    }

    //TODO: able to rewrite at runtime, not good
    public readonly Dictionary<InputAction, IEnumerable<Keys>> keyBindings = new()
    {
        [InputAction.goUp] = new []{ Keys.Up},
        [InputAction.goDown] = new []{ Keys.Down },
        [InputAction.goLeft] = new []{ Keys.Left },
        [InputAction.goRight] = new []{ Keys.Right },
        [InputAction.goDownLeft] = new []{ Keys.Down, Keys.Left },
        [InputAction.goDownRight] = new []{ Keys.Down, Keys.Right },
        [InputAction.goUpLeft] = new []{ Keys.Up, Keys.Left },
        [InputAction.goUpRight] = new []{ Keys.Up, Keys.Right },
        [InputAction.toggleInventory] = new []{ Keys.I },
        [InputAction.drop] = new []{ Keys.D },
        [InputAction.pickUp] = new []{ Keys.P },
        [InputAction.zoomIn] = new []{ Keys.PageDown },
        [InputAction.zoomOut] = new []{ Keys.PageUp },
        [InputAction.moveCameraUp] = new []{ Keys.NumPad8 },
        [InputAction.moveCameraDown] = new []{ Keys.NumPad2 },
        [InputAction.moveCameraLeft] = new []{ Keys.NumPad4 },
        [InputAction.moveCameraRight] = new []{ Keys.NumPad6 },
        [InputAction.toggleAbilitiesMenu] = new []{ Keys.A },
        [InputAction.exitWindow] = new []{ Keys.Escape },
        [InputAction.ability1] = new []{ Keys.D1 },
        [InputAction.ability2] = new []{ Keys.D2 },
        [InputAction.ability3] = new []{ Keys.D3 },
        [InputAction.ability4] = new []{ Keys.D4 },
        [InputAction.ability5] = new []{ Keys.D5 },
        [InputAction.ability6] = new []{ Keys.D6 },
        [InputAction.ability7] = new []{ Keys.D7 },
        [InputAction.ability8] = new []{ Keys.D8 },
        [InputAction.ability9] = new []{ Keys.D9 },
        [InputAction.ability10] = new []{ Keys.D0 },
    };
    
    private List<InputAction> _lastInputActions = new();
    
    private void InvokeInputActions(ICollection<Keys> keysList)
    {
        _lastInputActions.Clear();
        foreach (var (action, keys) in keyBindings)
        {
            if(keys.All(key => keysList.Contains(key))) 
                _lastInputActions.Add(action);
        }
        InputActionsPressed?.Invoke(_lastInputActions);
    }

    /// <summary>
    /// Makes something the only one listening to input, until it fires RelinquishControl event.
    /// As soon as RelinquishControl is fired, listeners are returned. 
    /// </summary>
    public void MakeSoloListener(IInputListener listener)
    {
        if(_soloListener != null) return;
        
        _soloListener = listener;
        listener.ControlRelinquished += OnSoloControlRelinquished;
        _listenersOnHold = InputActionsPressed;
        InputActionsPressed = listener.OnInputActionsPressed;
    }

    private void OnSoloControlRelinquished()
    {
        _soloListener = null;
        InputActionsPressed = _listenersOnHold;
    }
}