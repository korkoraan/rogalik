using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rogalik.Components;
using rogalik.Framework;
using rogalik.Objects;
using Point = rogalik.Common.Point;

namespace rogalik.Rendering;

//TODO: not a thread safe solution. singleton also might be a bad idea. rather do an observer of player surroundings.
public static class UIData
{
    public static List<string> messages = new();
}

/// <summary>
/// Draws everything.
/// </summary>
public class Renderer
{
    private SpriteBatch _spriteBatch;
    public Point windowSize => new (_halfWindowSize.x * 2, _halfWindowSize.y * 2);
    private Point _halfWindowSize;
    private Camera _mainCamera;
    private int _cellSize = 12;
    private World _world;
    private Cell[,] _cells;
    public Renderer(Point windowSize, SpriteBatch spriteBatch, World world)
    {
        _world = world;
        _world.WorldUpdated += OnWorldUpdated;
        _cells = _world.GetVisibleCells();
        _mainCamera = new Camera(new Point(700, 400));
        _spriteBatch = spriteBatch;
        _halfWindowSize = new Point(windowSize.x / 2, windowSize.y / 2);
        _logBox = new OutlinedBox(windowSize.x - 500, 0, 500, 500, 5, Color.Black, Color.DimGray, new Context(_spriteBatch, windowSize));
    }

    public void DrawCells()
    {
        foreach (var cell in _cells)
        {
            foreach (var obj in cell.contents)
            {
                var a = obj.GetComponent<Appearance>();
                var pos = new Vector2(cell.pos.x * _cellSize, cell.pos.y * _cellSize);
                _spriteBatch.Draw(a.texture, pos, null, Color.White, a.rotation, a.origin, 1, SpriteEffects.None, 0);
            }
        }
    }
    public Matrix GetTransformMatrix()
    {
        var dx = _halfWindowSize.x - _mainCamera.pos.x;
        var dy = _halfWindowSize.y - _mainCamera.pos.y;

        var zoom = _mainCamera.zoom;
        return Matrix.CreateTranslation(dx, dy, 0f) * Matrix.CreateScale(zoom);
    }

    private void OnWorldUpdated(IEnumerable<World.WorldAction> lastExecutedActions)
    {
        _cells = _world.GetVisibleCells();
        foreach (var wAction in lastExecutedActions)
        {
            //TODO: shouldn't work like this. Sufficient to request world of visible tiles. This way there is no need to move camera.
            if (wAction.successful && wAction.action is MoveSelf moveSelf && wAction.action.actor is Player)
            {
                _mainCamera.Move(moveSelf.point);
            }
        }
    }

    private void Text(int x, int y, string text, float scale = 1)
    {
        var i = 0;
        foreach (var c in text)
        {
            i++;
            if (c == ' ')
                continue;
            var texture = R.UI.charToTexture[c];
            var origin = new Vector2(0, texture.Height);
            var pos = new Vector2(i * 10 * scale + x, y);
            _spriteBatch.Draw(texture, pos, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
        }
    }

    private class Context
    {
        public SpriteBatch spriteBatch;
        public Point windowSize;

        public Context(SpriteBatch spriteBatch, Point windowSize)
        {
            this.spriteBatch = spriteBatch;
            this.windowSize = windowSize;
        }
    }

    private class OutlinedBox
    {
        public readonly int x;
        public readonly int y;
        public readonly int width;
        public readonly int height;
        private readonly int _thickness;
        private Context _context;
        private Color _color;
        private Color _colorOutline;
        private Rectangle _mainRect;
        private Rectangle _upperLine;
        private Rectangle _lowerLine;
        private Rectangle _leftLine;
        private Rectangle _rightLine;

        public OutlinedBox(int x, int y, int width, int height, int thickness, Color color, Color colorOutline, Context context)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            _thickness = thickness;
            _color = color;
            _colorOutline = colorOutline;
            _context = context;
            _mainRect = new Rectangle(x, y, width, height);
            _upperLine = new Rectangle(x, y, width, thickness);
            _lowerLine = new Rectangle(x, y + height - thickness, width, thickness);
            _leftLine = new Rectangle(x, y, thickness, height);
            _rightLine = new Rectangle(x + width - thickness, y, thickness, height);
        }
        
        public void Draw()
        {
            _context.spriteBatch.Draw(R.UI.whitePixel, _mainRect, _color);
            _context.spriteBatch.Draw(R.UI.whitePixel, _upperLine, _colorOutline);
            _context.spriteBatch.Draw(R.UI.whitePixel, _lowerLine, _colorOutline);
            _context.spriteBatch.Draw(R.UI.whitePixel, _leftLine, _colorOutline);
            _context.spriteBatch.Draw(R.UI.whitePixel, _rightLine, _colorOutline);
        }
    }


    //TODO: temporary measure, until UI library is made
    private OutlinedBox _logBox;
    private void DrawLog(IEnumerable<string> messages)
    {
        _logBox.Draw();
        var i = 1;
        foreach (var msg in messages.TakeLast(20))
        {
            Text(_logBox.x + 20, _logBox.y + 20 * i + 40, msg, 1.1f);
            i++;
        }
    }

    public void DrawUI()
    {
        DrawLog(UIData.messages);
    }
}