using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using rogalik.Framework;

namespace rogalik.Rendering.UIElements;

public class PickUpMenu
{
    private readonly Renderer _renderer;
    public ListView listView { get; }
    public PickUpMenu(Renderer renderer)
    {
        _renderer = renderer;

        listView = new ListView
        {
            Width = 300,
            Height = 100,
            Background = new SolidBrush(Color.DimGray), 
            Visible = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private void AddItem(Obj obj)
    {
        var grid = new Grid
        {
            ColumnSpacing = 30,
            ShowGridLines = true
        };
        grid.ColumnsProportions.Add(new Proportion());
        grid.ColumnsProportions.Add(new Proportion());
            
        grid.Widgets.Add(new Label { Text = obj.ToString() });
            
        listView.Widgets.Add(grid);
    }

    public void Toggle(IEnumerable<Obj> objects)
    {
        listView.Widgets.Clear();
        foreach (var obj in objects)
        {
            AddItem(obj);
        }
        
        listView.Visible = !listView.Visible;
        if(listView.Visible)
            UIData.AddLogMessage("you open up your satchel");
            
        listView.SetKeyboardFocus();
    }
}