using System.Collections.Generic;
using System.Linq;

namespace rogalik.Framework;

public class Filter
{
    public delegate bool FilterFunc(Obj x);

    public List<FilterFunc> filterFunctions = new ();

    public Filter With<TComponent>() where TComponent : IComponent
    {
        var result = new Filter
        {
            filterFunctions = new List<FilterFunc>(filterFunctions) { x=> x.HasComponent<TComponent>() }
        };
        return result;
    }

    public Filter With(FilterFunc f)
    {
        var result = new Filter();
        result.filterFunctions = new List<FilterFunc>(filterFunctions);
        result.filterFunctions.Add(f);
        return result;
    }

    private bool Check(Obj x)
    {
        return filterFunctions.All(f => f(x));
    }
    
    public IEnumerable<Obj> Apply(List<Obj> container)
    {
        return container.Where(x => Check(x)).ToList();
    }
}