using System.Collections.Generic;
using rogalik.Components;

namespace rogalik;

public abstract class Obj
{
    public Location location;
    public Cell cell;
    private List<Component> _components = new ();
    public int x => cell.pos.x;
    public int y => cell.pos.y;
    public int z => cell.pos.z;

    protected Obj(Location location)
    {
        this.location = location;
    }
    
    public TComponent GetComponent<TComponent>() where TComponent: Component
    {
        return (TComponent)_components.Find(c => c is TComponent);
    }

    
    public TComponent AddComponent<TComponent>(TComponent component) where TComponent : Component
    {
        if (_components.Exists(c => c is TComponent))
            return null;
        _components.Add(component);
        component.parent = this;
        return component;
    }
}