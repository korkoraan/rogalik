using System.Collections.Generic;
using System.Linq;
using Myra.Graphics2D.UI;

namespace rogalik.Rendering.Parts;

public class ListViewG : ListView
{
    public bool receivesInput = true;
    public int? previousSelectedIndex { get; private set; }

    public ListViewG()
    {
        SelectedIndexChanged += (_, _) =>
        {
            if (receivesInput)
                previousSelectedIndex = SelectedIndex;
            else
                SelectedIndex = previousSelectedIndex;
        };
    }
}

public class FilteredListView : ListViewG
{
    public delegate bool FilterFunc(object x);
    private List<FilterFunc> _filterFunctions = new();
    private List<Widget> _selectableWidgets = new ();
    private int? _lastSelectedIndex = 0;

    public FilteredListView()
    {
    }

    public void AddFilterFunc(FilterFunc filterFunc)
    {
        _filterFunctions.Add(filterFunc);
    }

    public void Exclude<T>()
    {
        _filterFunctions.Add(obj => obj is not T);
    }

    public void Exclude(Widget excludedWidget)
    {
        _filterFunctions.Add(obj => obj != excludedWidget);
    }
    public void Add(Widget widget)
    {
        Widgets.Add(widget);
        if(_filterFunctions.All(f => f(SelectedItem)))
            _selectableWidgets.Add(widget);
    }
    
    public void SelectNextElement()
    {
        var index = _selectableWidgets.IndexOf(SelectedItem);
        SelectedItem = index == _selectableWidgets.Count - 1 ? _selectableWidgets.First() : _selectableWidgets[index + 1];
    }

    public void SelectPreviousElement()
    {
        var index = _selectableWidgets.IndexOf(SelectedItem);
        SelectedItem = index == 0 ? _selectableWidgets.Last() : _selectableWidgets[index - 1];
    }

    public Widget this[int index]
    {
        get => _selectableWidgets[index];
        set => _selectableWidgets[index] = value;
    }
}