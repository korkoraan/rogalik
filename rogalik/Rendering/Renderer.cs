using System.Collections.Generic;
using System.Linq;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using rogalik.Framework;
using rogalik.Framework.Map;
using rogalik.Rendering.UIElements;
using rogalik.Systems.AI;
using rogalik.Systems.Common;
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

    public static void AddLogMessage(object message)
    {
        var msg = message.ToString();
        _log.Add(msg);
        LogUpdated?.Invoke(msg);
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
    public readonly Game1 game;
    public XnaPoint windowSize => new(_halfWindowSize.X * 2, _halfWindowSize.Y * 2);
    public World world;
    public readonly FontSystem fontSystem;
    private SpriteBatch _spriteBatch;
    private XnaPoint _halfWindowSize;
    private Camera _mainCamera;
    private int _cellSize = 12;
    private List<List<(Point, Obj)>> _layers = new();
    private MapPiece? _visibleMap;
    private List<Point> _debugDots;
    private Panel _rootPanel;
    private IInputListener _controllingListener;
    public State state { get; set; }

    public Renderer(Game1 game, Point windowSize, SpriteBatch spriteBatch, FontSystem fontSystem)
    {
        this.game = game;
        _spriteBatch = spriteBatch;
        this.fontSystem = fontSystem;
        _halfWindowSize = new XnaPoint(windowSize.x / 2, windowSize.y / 2);
        _debugDots = new List<Point>();

        state = State.none;
    }

    public enum State
    {
        none,
        inventory,
        choosingItem,
        abilitiesMenu
    }

    public void Init()
    {
        world = game.world;
        GetVisibleObjects();
        world.FinishedUpdate += OnFinishedUpdate;
        _mainCamera = new Camera(game, world.player);
    }

    public static Texture2D? GetTexture4Tile(Framework.Map.Tile.Data tileData)
    {
        return tileData.kind switch
        {
            Framework.Map.Tile.Kind.rockFloor => R.Tiles.surfaceRock[tileData.subKind],
            Framework.Map.Tile.Kind.wallRock => R.Tiles.wallRock[tileData.subKind],
            _ => null
        };
    }
        
    /// <summary>
    /// Contains all elements that make up an inventory window
    /// </summary>
    public void DrawVisibleArea()
    {
        if (_visibleMap != null)
        {
            var area = _visibleMap.Value.area;
            var tiles = _visibleMap.Value.Tiles;
            var stop = new MapPoint(area.EndX, area.EndY);
            for (int x = area.X, i = 0; x <= stop.X; ++x, ++i)
            {
                for (int y = area.Y, j = 0; y <= stop.Y; ++y, ++j)
                {
                    var texture = GetTexture4Tile(tiles[i, j]);
                    if (texture != null)
                        DrawTexture(new Point(x, y), texture);
                }
            }
        }
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
        var pos = new Vector2(point.x * _cellSize, point.y * _cellSize);
        var origin = new Vector2(0, texture2D.Height);
        _spriteBatch.Draw(texture2D, pos, null, Color.White, 0f, origin, 1, SpriteEffects.None, 0);
    }

    public Matrix GetTransformMatrix()
    {
        var zoom = _mainCamera.zoom;
        var dx = _halfWindowSize.X / zoom - _mainCamera.pos.x * 12;
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
        _layers.Clear();
        _layers.Add(new List<(Point, Obj)>());
        _layers.Add(new List<(Point, Obj)>());
        _layers.Add(new List<(Point, Obj)>());
        _visibleMap = world.GetVisibleMap(world.player);
        var visibleObjects = world.GetVisibleObjects(world.player);
        foreach (var (point, obj) in visibleObjects)
        {
            if (obj.HasComponent<Mind>() || obj == world.player)
            {
                _layers[1].Add((point, obj));
            }
            else
                _layers[0].Add((point, obj));

        }
    }
    
    public void InitUI(Desktop desktop)
    {
        _rootPanel = new Panel();
        
        var characterScreen = new CharacterScreen(this)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center, ZIndex = 1
        };

        var abilitiesBar = new AbilitiesBar(this, characterScreen.abilitiesMenu)
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Center,
            Top = - 75
        };
        
        var log = new Log(this)
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        var inventoryScreen = new InventoryScreen(this)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        var statsBar = new StatsBar
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Top = 20,
            Left = 60,
        };

        _rootPanel.Widgets.Add(characterScreen);
        _rootPanel.Widgets.Add(log);
        _rootPanel.Widgets.Add(inventoryScreen);
        _rootPanel.Widgets.Add(abilitiesBar);
        _rootPanel.Widgets.Add(statsBar);
        
        desktop.Root = _rootPanel;
    }
}