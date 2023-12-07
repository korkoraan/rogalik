using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rogalik.Components;
using rogalik.Framework;
using Point = rogalik.Common.Point;

namespace rogalik.Rendering;

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
    public Renderer(Point windowSize, SpriteBatch spriteBatch)
    {
        _mainCamera = new Camera(new Point(500, 300));
        _spriteBatch = spriteBatch;
        _halfWindowSize = new Point(windowSize.x / 2, windowSize.y / 2);
    }
    
    public void DrawCells(Cell[,] cells)
    {
        foreach (var cell in cells)
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
}