using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using rogalik.Common;
using rogalik.Framework;
using rogalik.Systems.Common;
using rogalik.Systems.Items;

namespace rogalik.Rendering.UIElements;

public class InventoryScreen : ListView, IInputListener
{
       private List<Obj> _items = new ();
       /// <summary>
       /// root element
       /// </summary>
       private Renderer _renderer;
       public InventoryScreen(Renderer renderer)
       {
           _renderer = renderer;
           Width = 300;
           Height = 600;
           Background = new SolidBrush(Color.DimGray);
           Visible = false;
           
           renderer.game.input.InputActionsPressed += OnInputActionsPressed;
       }

       private void AddItem(Obj obj)
       {
           _items.Add(obj);
           var grid = new Grid
           {
               ColumnSpacing = 30,
               ShowGridLines = true
           };
           grid.ColumnsProportions.Add(new Proportion());
           grid.ColumnsProportions.Add(new Proportion());
           
           grid.Widgets.Add(new Label { Text = obj.ToString() });
           Widgets.Add(grid);
       }

       public void Toggle()
       {
           Widgets.Clear();
           _items.Clear();
           Visible = !Visible;

           if (_renderer.state == Renderer.State.inventory)
           {
               _renderer.state = Renderer.State.none;
               ControlRelinquished?.Invoke();
               return;
           }
           
           _renderer.game.input.MakeSoloListener(this);
           _renderer.state = Renderer.State.inventory;
           foreach (var item in _renderer.world.player.InventoryItems())
           {
               AddItem(item);
           }
           SetKeyboardFocus();
       } 

    public void OnInputActionsPressed(List<InputAction> keys)
    {
        if (keys.Contains(InputAction.toggleInventory))
            Toggle();

        if (keys.Contains(InputAction.drop) && _items.Count > 0 && SelectedIndex != null)
        {
            var index = (int)SelectedIndex;
            _renderer.world.player.Add(new IntentDrop(_items[index]));
            SelectedItem.Visible = false;
        }
    }

    public event IInputListener.ControlRelinquishedHandler ControlRelinquished;
}