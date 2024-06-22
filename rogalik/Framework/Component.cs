namespace rogalik.Framework;

/// <summary>
/// Base class for all components.
/// </summary>
public abstract class Component
{
    public Obj owner;

    /// <summary>
    /// Use it to find other owner's components and finish setup.
    /// </summary>
    public virtual void Init()
    {
    }
}