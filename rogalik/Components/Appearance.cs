using Microsoft.Xna.Framework.Graphics;

namespace rogalik.Components;

/// <summary>
/// How an object is rendered.
/// </summary>
public class Appearance : Component
{
    public Texture2D texture;

    public Appearance(Texture2D texture)
    {
        this.texture = texture;
    }
}