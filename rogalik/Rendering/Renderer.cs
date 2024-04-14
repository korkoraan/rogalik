using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using rogalik.AI;
using rogalik.Common;
using rogalik.Framework;
using rogalik.Items;
using Point = rogalik.Framework.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace rogalik.Rendering;

//TODO: not a thread safe solution. singleton also might be a bad idea. rather do an observer of player surroundings.
public static class UIData
{
    private static readonly List<string> _log = new();
    public delegate void LogUpdatedHandler(string newMessage);

    public static event LogUpdatedHandler LogUpdated;

    public static void AddLogMessage(string message)
    {
        _log.Add(message);
        LogUpdated?.Invoke(message);
    }

    public static IEnumerable<string> GetLastMessages(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if(i > _log.Count - 1) yield break;
            yield return _log[^i];
        }
    }
}

/// <summary>
/// Draws everything.
/// </summary>
public class Renderer
{
    private Game1 _game;
    private SpriteBatch _spriteBatch;
    public XnaPoint windowSize => new (_halfWindowSize.X * 2, _halfWindowSize.Y * 2);
    private XnaPoint _halfWindowSize;
    private Camera _mainCamera;
    private int _cellSize = 12;
    private World _world;
    private List<List<(Point, Obj)>> _layers = new ();
    private List<Point> _debugDots;
    public State state { get; private set; }
   
    public Renderer(Game1 game, Point windowSize, SpriteBatch spriteBatch)
    {
        _game = game;
        _spriteBatch = spriteBatch;
        _halfWindowSize = new XnaPoint(windowSize.x / 2, windowSize.y / 2);
        _debugDots = new List<Point>();

        _game.input.InputActionsPressed += OnInputActionsPressed;
        
        state = State.none;
    }

    public enum State
    {
        none,
        inventory,
        choosingItem,
    }

    public void Init()
    {
        _world = _game.world;
        GetVisibleObjects();
        _world.FinishedUpdate += OnFinishedUpdate;
        _mainCamera = new Camera(_game, _world.player);
        UIData.LogUpdated += OnLogUpdated;
        _game.desktop.TouchDown += ShowOptions;
    }

    private void OnInputActionsPressed(List<InputAction> keys)
    {
        var old = state.ToString();
        // UIData.AddLogMessage(old + " -> " + state);
    }

    /// <summary>
    /// Contains all elements that make up an inventory window
    /// </summary>

    public void DrawVisibleArea()
    {
        foreach (var layer in _layers)
        {
            foreach (var (point, obj) in layer)
            {
                var appearance = obj.GetComponent<Appearance>();
                DrawTexture(point, appearance.texture);
            }
        }
        

        // foreach (var point in _debugDots)
        // {
            // var texture = R.UI.charToTexture['s'];
            // DrawTexture(point, texture);
        // }
    }

    private void DrawTexture(Point point, Texture2D texture2D)
    {
        //TODO: hardcoded offset, to be fixed
        var pos = new Vector2(point.x * _cellSize, point.y * _cellSize) - new Vector2(100, 0);
        var origin = new Vector2(0, texture2D.Height);
        _spriteBatch.Draw(texture2D, pos, null, Color.White, 0f, origin, 1, SpriteEffects.None, 0);
    }

    public Matrix GetTransformMatrix()
    {
        var zoom = _mainCamera.zoom;
        var dx = _halfWindowSize.X / zoom - _mainCamera.pos.x * 12 ;
        var dy = _halfWindowSize.Y / zoom - _mainCamera.pos.y * 12;
        return Matrix.CreateTranslation(dx, dy, 0f) * Matrix.CreateScale(zoom);
    }
    
    private void OnFinishedUpdate()
    {
        GetVisibleObjects();
    }

    /// <summary>
    /// retrieves visible objects from the world and filters them into layers
    /// </summary>
    private void GetVisibleObjects()
    {
        var visibleObjects = _world.GetVisibleObjects();
        _layers.Clear();
        _layers.Add(new List<(Point, Obj)>());
        _layers.Add(new List<(Point, Obj)>());
        _layers.Add(new List<(Point, Obj)>());
        foreach (var (point, obj) in visibleObjects)
        {
            if (obj.HasComponent<Mind>() || obj == _world.player)
            {
                _layers[1].Add((point, obj));
            }
            else
                _layers[0].Add((point, obj));
            
        }
    }

    private void OnLogUpdated(string newMessage)
    {
        _log.Widgets.Add(new Label
        {
            Text = newMessage,
        });
    }

    private ListView _log;
    
    public class Inventory : IInputListener
    {
        private readonly Renderer _renderer;
        private List<Obj> _items = new ();
        /// <summary>
        /// root element
        /// </summary>
        public ListView listView { get; }
        public Inventory(Renderer renderer)
        {
            _renderer = renderer;

            listView = new ListView
            {
                Width = 300,
                Height = 600,
                Background = new SolidBrush(Color.DimGray), 
                Visible = false, 
            };
            
            _renderer._game.input.InputActionsPressed += OnInputActionsPressed;
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
            
            listView.Widgets.Add(grid);
        }

        public void Toggle()
        {
            listView.Widgets.Clear();
            _items.Clear();
            listView.Visible = !listView.Visible;

            if (_renderer.state == State.inventory)
            {
                _renderer.state = State.none;
                ControlRelinquished?.Invoke();
                return;
            }
            
            _renderer._game.input.MakeSoloListener(this);
            _renderer.state = State.inventory;
            foreach (var item in _renderer._world.player.InventoryItems())
            {
                AddItem(item);
            }
            listView.SetKeyboardFocus();
        }

        public void OnInputActionsPressed(List<InputAction> keys)
        {
            if (keys.Contains(InputAction.toggleInventory))
                Toggle();

            if (keys.Contains(InputAction.drop) && _items.Count > 0 && listView.SelectedIndex != null)
            {
                var index = (int)listView.SelectedIndex;
                _renderer._world.player.Add(new IntentDrop(_items[index]));
                listView.SelectedItem.Visible = false;
            }
        }

        public event IInputListener.ControlRelinquishedHandler ControlRelinquished;
    }
    private class PickUpMenu
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
    
    private Inventory _inventory;
    private PickUpMenu _pickUpMenu;
    public void InitUI(Desktop desktop)
    {
        var rootPanel = new Panel
        {
        };
        
        _log = new ListView
        {
            Width = 500,
            Height = 700,
            Top = 100,
            Left = -100,
            ChildrenLayout = new GridLayout { RowSpacing = 30 },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        
        _inventory = new Inventory(this)
        {
            listView =
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        
        _pickUpMenu = new PickUpMenu(this)
        {
            listView =
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        
        rootPanel.Widgets.Add(_inventory.listView);
        rootPanel.Widgets.Add(_log);
        desktop.Root = rootPanel;
    }
    
    public void ChooseItemToPick(IEnumerable<Obj> items)
    {
        if (state != State.none) return;
        state = State.choosingItem;
        
        _pickUpMenu.Toggle(items);
    }
    
    private void ShowOptions(object sender, EventArgs e)
    {
        if (_game.desktop.ContextMenu != null)
        {
            // Dont show if it's already shown
            return;
        }

        var container = new VerticalStackPanel
        {
            Spacing = 4
        };

        var titleContainer = new Panel ();

        var titleLabel = new Label
        {
            Text = "Choose Option",
            HorizontalAlignment = HorizontalAlignment.Center
        };

        titleContainer.Widgets.Add(titleLabel);
        container.Widgets.Add(titleContainer);

        var menuItem1 = new MenuItem();
        menuItem1.Text = "Pick up";
        menuItem1.Selected += (s, a) =>
        {
            // "Start New Game" selected
        };

        var menuItem2 = new MenuItem();
        menuItem2.Text = "Options";

        var menuItem3 = new MenuItem();
        menuItem3.Text = "Quit";

        var verticalMenu = new VerticalMenu();

        verticalMenu.Items.Add(menuItem1);
        verticalMenu.Items.Add(menuItem2);
        verticalMenu.Items.Add(menuItem3);

        container.Widgets.Add(verticalMenu);

        if(_game.desktop.TouchPosition != null)
            _game.desktop.ShowContextMenu(container, (Microsoft.Xna.Framework.Point)_game.desktop.TouchPosition);
    }
}