using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using rogalik.Combat;
using rogalik.Common;
using rogalik.Items;
using rogalik.Rendering;
using rogalik.Walking;

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
    hit,
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
    moveCameraRight
}

/// <summary>
/// Subscribe to KeyPressed to listen for any pressed keys.
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

    public delegate void ActionChosenHandler();
    public event ActionChosenHandler ActionChosen;

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
        InputActionsPressed += OnActionPressed;
    }

    public void Init()
    {
        _world = _game.world;
    }

    private void OnActionPressed(List<InputAction> actions)
    {
        
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

    private Dictionary<IEnumerable<Keys>, InputAction> _keyBindings = new()
    {
        [new[] { Keys.Up }] = InputAction.goUp,
        [new[] { Keys.Down }] = InputAction.goDown,
        [new[] { Keys.Left }] = InputAction.goLeft,
        [new[] { Keys.Right }] = InputAction.goRight,
        [new[] { Keys.Right, Keys.Down }] = InputAction.goDownRight,
        [new[] { Keys.Right, Keys.Up }] = InputAction.goUpRight,
        [new[] { Keys.Left, Keys.Down }] = InputAction.goDownLeft,
        [new[] { Keys.Left, Keys.Up }] = InputAction.goUpLeft,
        [new[] { Keys.I }] = InputAction.toggleInventory,
        [new[] { Keys.D }] = InputAction.drop,
        [new[] { Keys.P }] = InputAction.pickUp,
        [new[] { Keys.PageDown }] = InputAction.zoomIn,
        [new[] { Keys.PageUp }] = InputAction.zoomOut,
        [new[] { Keys.NumPad8 }] = InputAction.moveCameraUp,
        [new[] { Keys.NumPad2 }] = InputAction.moveCameraDown,
        [new[] { Keys.NumPad4 }] = InputAction.moveCameraLeft,
        [new[] { Keys.NumPad6 }] = InputAction.moveCameraRight,
    };

    private List<InputAction> _lastInputActions = new();
    
    private void InvokeInputActions(ICollection<Keys> keysList)
    {
        _lastInputActions.Clear();
        foreach (var (keys, action) in _keyBindings)
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