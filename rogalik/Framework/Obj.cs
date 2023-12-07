using System;
using System.Collections.Generic;
using rogalik.Common;

namespace rogalik.Framework;

public abstract class Obj
{
    public Location location;
    public Cell cell;
    private List<Component> _components = new ();
    
    public int x => cell.pos.x;
    public int y => cell.pos.y;
    public int z => cell.pos.z;

    public Point point => (x, y, z); 

    protected Obj(Location location)
    {
        this.location = location;
    }
    
    public TComponent GetComponent<TComponent>() where TComponent: Component
    {
        return (TComponent)_components.Find(c => c is TComponent);
    }

    public List<Component> GetAllComponents<T>()
    {
        return _components.FindAll(c => c is T);
    }
    
    public TComponent AddComponent<TComponent>(TComponent component) where TComponent : Component
    {
        _components.Add(component);
        component.owner = this;
        return component;
    }

    public bool HasComponent<TComponent>() where TComponent : Component
    {
        return _components.Exists(c => c is TComponent);
    }

    public void Init()
    {
        foreach (var c in _components)
        {
            c.Init();
        }
    }
}