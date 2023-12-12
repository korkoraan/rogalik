using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rogalik.Framework;

namespace rogalik.Components;

/// <summary>
/// How an object is rendered.
/// </summary>
public class Appearance : Component
{
    public Texture2D texture;
    public float rotation { get; private set; }
    public Vector2 origin { get; private set; }

    public Appearance(Texture2D texture)
    {
        this.texture = texture;
    }

    public void SetDefault()
    {
        rotation = MathHelper.ToRadians(0);
        origin = new Vector2(0, texture.Height);
    }
    
    public void SetDead()
    {
        rotation = MathHelper.ToRadians(90);
        origin = new Vector2(texture.Width, texture.Height);
    }

    public override void Init()
    {
        var mind = owner.GetComponent<Mind>();
        if(mind != null)
            mind.Dies += SetDead;
        SetDefault();
    }
}