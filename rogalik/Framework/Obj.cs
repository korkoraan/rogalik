using System.Collections;
using System.Collections.Generic;
using rogalik.Common;

namespace rogalik.Framework;

public class Obj : IEnumerable
{
    public List<IComponent> components = new();

    public Obj()
    {
    }
    
    public Obj(IEnumerable<IComponent> components)
    {
        foreach (var component in components)
        {
            this.components.Add(component);
        }
    }
    
    public TComponent GetComponent<TComponent>() where TComponent : IComponent
    {
        return (TComponent)components.Find(c => c is TComponent);
    }

    public List<IComponent> GetAllComponents<T>()
    {
        return components.FindAll(c => c is T);
    }

    public Obj AddComponent<TComponent>(TComponent component) where TComponent :  IComponent
    {
        components.Add(component);
        return this;
    }

    public bool HasComponent<TComponent>() where TComponent : IComponent
    {
        return components.Exists(c => c is TComponent);
    }

    public void Init()
    {
    }

    public void RemoveComponent(IComponent component)
    {
        components.Remove(component);
    }

    public override string ToString()
    {
        return GetComponent<Appearance>()?.description ?? "unknown object";
    }

    public void Add(IComponent component)
    {
        components.Add(component);
    }

    public IEnumerator GetEnumerator()
    {
        return components.GetEnumerator();
    }
}
