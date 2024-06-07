using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using rogalik.Framework;
using rogalik.Rendering.Parts;

namespace rogalik.Rendering.UIElements;

public enum SlotType
{
    hand,
    body,
    legs,
    head,
    feet,
    face,
    ring
}

public sealed class EquipmentMenu : Grid, IInputListener
{
    private SlotsGrid _slotsGrid;
    private ListView _itemsListView;

    public delegate void SlotEquippedHandler(Slot slot, Item equippedItem);

    public event SlotEquippedHandler SlotEquipped;

    public EquipmentMenu(CharacterScreen characterScreen)
    {
        Background = new SolidBrush(Color.DimGray);
        Width = characterScreen.contentPanel.Width;
        Height = characterScreen.contentPanel.Height;
        BorderThickness = new Thickness (5);
        Border = new SolidBrush(Color.Gray);
        ColumnsProportions.Add(new Proportion(ProportionType.Part, .3f));
        ColumnsProportions.Add(new Proportion(ProportionType.Part, .7f));
        
        var wornItemsPanel = new Panel { Background = new SolidBrush(Color.DarkSlateGray), /* Width = Width / 3, Height = Height */ };
        SetColumn(wornItemsPanel, 0);
        Widgets.Add(wornItemsPanel);

        _slotsGrid = new SlotsGrid
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            ColumnSpacing = 8
        };
        wornItemsPanel.Widgets.Add(_slotsGrid);
        
        _itemsListView = new ListView
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        
        SetColumn(_itemsListView, 1);
        Widgets.Add(_itemsListView);
        
        UpdateSlots();
        UpdateItems();
    }
    
    private void AddSection(SlotPanel slotPanel)
    {
        var section = new Section(slotPanel, (int)Width / 2, 50);
        _itemsListView.Widgets.Add(section);
    }

    private void AddItemPanel(Item item)
    {
        foreach (var section in _itemsListView.Widgets.Where(w => w is Section section && section.slotPanel.slot.type == item.slotType))
        {
            var index = _itemsListView.Widgets.IndexOf(section);
            var itemPanel = new ItemPanel(item, (int)Width / 2, 20, section as Section);
            _itemsListView.Widgets.Insert(index+1, itemPanel);
        }
    }
    
    private sealed class SlotPanel : VerticalStackPanel
    {
        public Panel panel;
        public Slot slot;
        private ItemPanel _itemPanel;
        public ItemPanel itemPanel
        {
            get => _itemPanel;
            set
            {
                panel.Widgets.Clear();
                _itemPanel = value;
                if(value == null) return;
                panel.Widgets.Add(new Icon(panel.Width, panel.Height, itemPanel.item.texture));
            }
        }

        public SlotPanel(Slot slot, int size)
        {
            this.slot = slot;
            Widgets.Add(new Label {Text = slot.name, HorizontalAlignment = HorizontalAlignment.Center});
            panel = new Panel { Background = new SolidBrush(Color.DarkGray), Width = size, Height = size };
            Widgets.Add(panel);
        }
    }

    private sealed class SlotsGrid : Grid
    {
        public SlotsGrid()
        {
            
        }
        
        public void ResetSlots(IEnumerable<Slot> newSlots)
        {
            Widgets.Clear();
            RowsProportions.Clear();
            ColumnsProportions.Clear();
            var slots = newSlots as Slot[] ?? newSlots.ToArray();

            for(var i = 0; i < slots.Select(slot => slot.y).Max(); i++)
            {
                RowsProportions.Add(new Proportion());
            }
            for(var i = 0; i < slots.Select(slot => slot.x).Max(); i++)
            {
                ColumnsProportions.Add(new Proportion());
            }

            var slotPanels = new List<SlotPanel>();
            foreach (var slot in slots)
            {
                var slotPanel = new SlotPanel(slot, 50);
                slotPanels.Add(slotPanel);
                SetColumn(slotPanel, slot.x);
                SetRow(slotPanel, slot.y);
            }
            foreach (var slotPanel in slotPanels)
            {
                Widgets.Add(slotPanel);
            }
        }
    }
    
    private void UpdateSlots()
    {
        _slotsGrid.ResetSlots(Global.GetPlayerSlots());
    }

    private void UpdateItems()
    {
        _itemsListView.Widgets.Clear();
        foreach (var slotPanel in _slotsGrid.Widgets.Where(w => w is SlotPanel))
        {
            _itemsListView.Widgets.Add(new VerticalSeparator { Height = 30, Background = new SolidBrush(Color.Transparent)});
            AddSection(slotPanel as SlotPanel);
        }
        foreach (var item in Global.GetPlayerItems())
        {
            AddItemPanel(item);
        }
    }
    
    private sealed class ItemPanel : HorizontalStackPanel
    {
        public readonly Section section;
        public readonly Item item;
        public readonly ToggleButton toggleBtn;
        public ItemPanel(Item item, int width, int height, Section section)
        {
            this.item = item;
            this.section = section;
            Left = 30;
            Width = width;
            Height = height;
            Spacing = 10;
            toggleBtn = new ToggleButton
            {
                Width = height,
                Height = height,
                Background = new SolidBrush(Color.DarkBlue),
                PressedBackground = new SolidBrush(Color.Green)
            };
            Widgets.Add(toggleBtn);

            Widgets.Add(new Label{Text = item.name});
            Widgets.Add(new HorizontalSeparator {Width = 50, Color = Color.Transparent});
            switch (item)
            {
                case Weapon weapon:
                    Widgets.Add(new Label{ Text = weapon.dmg.ToString()});
                    Widgets.Add(new Icon(15, 15, R.Icons.damage));
                    break;
                case Armor armor:
                    Widgets.Add(new Label{ Text = armor.armorPts.ToString()});
                    Widgets.Add(new HorizontalSeparator());
                    Widgets.Add(new Icon(15, 15, R.Icons.armorPoints) { VerticalAlignment = VerticalAlignment.Center });
                    break;
            }
        }
        
        
    }
    
    private sealed class Section : Panel
    {
        public readonly SlotPanel slotPanel;
        public Section(SlotPanel slotPanel, int width, int height)
        {
            this.slotPanel = slotPanel;
            Widgets.Add(new Label
            {
                Text = this.slotPanel.slot.name.ToUpper(),
                AcceptsKeyboardFocus = false, 
                Width = width,
                Height = height,
                Left = 10,
                VerticalAlignment = VerticalAlignment.Center
            });
        }
    }
    
    private void SetEquipmentItem(ItemPanel itemPanel)
    {
        var slotPanel = itemPanel.section.slotPanel;
        if (slotPanel.itemPanel == null)
        {
            slotPanel.itemPanel = itemPanel;
            itemPanel.toggleBtn.IsToggled = true;
        }
        else if (slotPanel.itemPanel == itemPanel)
        {
            slotPanel.itemPanel.toggleBtn.IsToggled = false;
            slotPanel.itemPanel = null;
        }
        else
        {
            slotPanel.itemPanel.toggleBtn.IsToggled = false;
            slotPanel.itemPanel = itemPanel;
            slotPanel.itemPanel.toggleBtn.IsToggled = true;
            SlotEquipped?.Invoke(slotPanel.slot, itemPanel.item);
        }
        //if item exists somewhere else on _slotsGrid, remove it 
        foreach (var slotWithItem in _slotsGrid.Widgets.Where(w => w is SlotPanel s && s != slotPanel && s.itemPanel != null && s.itemPanel.item == itemPanel.item))
        {
            ((SlotPanel)slotWithItem).itemPanel.toggleBtn.IsToggled = false;
            ((SlotPanel)slotWithItem).itemPanel = null;
        }
    }
    
    public class Item
    {
        public string name;
        public SlotType slotType;
        public Texture2D texture;

        public Item(string name, SlotType slotType, Texture2D texture)
        {
            this.name = name;
            this.slotType = slotType;
            this.texture = texture;
        }
    }

    public class Weapon : Item
    {
        public int dmg;

        public Weapon(string name, SlotType slotType, Texture2D texture, int dmg = 0) : base(name, slotType, texture)
        {
            this.dmg = dmg;
        }
    }

    public class Armor : Item
    {
        public int armorPts;

        public Armor(string name, SlotType slotType, Texture2D texture, int armorPts = 0) : base(name, slotType, texture)
        {
            this.armorPts = armorPts;
        }
    }
    
    public class Slot
    {
        public readonly string name;
        public readonly SlotType type;
        public readonly int x;
        public readonly int y;

        public Slot(string name, int x, int y, SlotType type)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.type = type;
        }
    } 
    
    public void OnInputActionsPressed(List<InputAction> actions)
    {
        if (actions.Contains(InputAction.UIselectDown) || actions.Contains(InputAction.UIselectUp))
            _itemsListView.SetKeyboardFocus();
        if (actions.Contains(InputAction.UIselect))
        {
            if(_itemsListView.SelectedItem is not ItemPanel itemPanel) return;
            SetEquipmentItem(itemPanel);
        }
        if(actions.Contains(InputAction.ability1))
            UpdateItems();
        if(actions.Contains(InputAction.ability2))
            UpdateSlots();
    }

    public event IInputListener.ControlReleasedHandler ControlReleased;
}