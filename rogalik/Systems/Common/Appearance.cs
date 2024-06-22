using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rogalik.Framework;

namespace rogalik.Systems.Common;

/// <summary>
/// How an object looks like.
/// </summary>
public class Appearance : IComponent
{
    public Texture2D texture;
    public readonly string description; 
    public float rotation { get; private set; }
    public Vector2 origin { get; private set; }

    public Appearance(Texture2D texture, string description)
    {
        this.texture = texture;
        this.description = description;
    }
}
