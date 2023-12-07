namespace rogalik.Framework;

/// <summary>
/// Base class for all components.
/// </summary>
public abstract class Component
{
    public Obj owner;

    public abstract void Init();
}