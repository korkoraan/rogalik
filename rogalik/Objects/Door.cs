using Microsoft.Xna.Framework.Graphics;
using rogalik.Common;
using rogalik.Components;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik.Objects;

public class Door : Obj, IOpenable
{
    private bool _closed = true;
    private readonly Appearance _appearance;
    private readonly Texture2D _textureClosed;
    private readonly Texture2D _textureOpened;
    private readonly Impassible _impassible;

    public Door(Point point, Location location) : base(point, location)
    {
        _textureClosed = R.Tiles.door.Random();
        _textureOpened = R.Tiles.doorOpened;
        _appearance = AddComponent(_closed ? new Appearance(_textureClosed) : new Appearance(_textureOpened));
        _impassible = AddComponent(new Impassible());
    }

    public bool Open()
    {
        if(!_closed) return false;
        _closed = false;
        _impassible.enabled = false;
        _appearance.texture = _textureOpened;
        return true;
    }
    
    public bool Close()
    {
        if(_closed) return false;
        _closed = true;
        _impassible.enabled = true;
        _appearance.texture = _textureClosed;
        return true;
    }
}