using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace rogalik.Framework;

public class Obj : IEnumerable
{
    private List<IComponent> _components = new();
    public Obj()
    {
    }
    public Obj(IEnumerable<IComponent> components)
    {
        foreach (var component in components)
        {
            AddComponent(component);
        }
    }
    public TComponent? GetComponent<TComponent>() where TComponent : IComponent
    {
        return _components.OfType<TComponent>().FirstOrDefault();
    }
    
    public IEnumerable<TComponent> GetAllComponents<TComponent>() where TComponent : IComponent
    {
        return _components.OfType<TComponent>();
    }
    public IEnumerable<IComponent> GetAllComponents(Func<IComponent, bool> predicate)
    {
        return _components.FindAll(c => predicate(c));
    } 
    public TComponent AddComponent<TComponent>(TComponent component) where TComponent :  IComponent
    {
        _components.Add(component);
        return component;
    }
    public bool HasComponent<TComponent>() where TComponent : IComponent
    {
        return _components.Exists(c => c is TComponent);
    }
    public void Init()
    {
    }
    public void RemoveComponent(IComponent component)
    {
        _components.Remove(component);
    }

    public void RemoveMultipleComponents(IEnumerable<IComponent> components)
    {
        foreach (var c in components)
        {
            _components.Remove(c);
        }
    }

    public void RemoveComponentsOfType<TComponent>() where TComponent : IComponent
    {
        _components.RemoveAll(c => c is TComponent);
    }

    public void RemoveAllComponentsWhere(Func<IComponent, bool> predicate)
    {
        _components.RemoveAll(c => predicate(c));
    }
    
    public void Add(IComponent component)
    {
        _components.Add(component);
    }
    public IEnumerator GetEnumerator()
    {
        return _components.GetEnumerator();
    }
}
