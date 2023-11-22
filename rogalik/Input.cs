using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace rogalik;

/// <summary>
/// Subscribe to KeyPressed to listen for any pressed keys.
/// <param name="_delay">number of milliseconds waited to read a key combination</param>
/// </summary>
public static class Input
{
    public delegate void KeyPressedHandler(List<Keys> key);
    public static event KeyPressedHandler KeysPressed;

    private static List<Keys> _keys = new();
    private static double _delay = 200;
    private static double _remainingDelay = _delay;
    
    public static void Update(GameTime gameTime)
    {
        if (_keys.Count > 0 && _remainingDelay <= 0)
        {
            KeysPressed?.Invoke(_keys);
            _keys.Clear();
            _remainingDelay = _delay;
            return;
        }
        
        foreach (var key in Keyboard.GetState().GetPressedKeys())
            if(!_keys.Contains(key))
                _keys.Add(key);

        if(_keys.Count > 0)
            _remainingDelay -= gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    private static void OnKeysRelease()
    {
        if (_keys.Count > 0)
            KeysPressed?.Invoke(_keys);
    }
}