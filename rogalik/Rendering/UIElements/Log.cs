using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace rogalik.Rendering.UIElements;

public class Log : ListView
{
    private readonly Renderer _renderer;
    private SpriteFontBase _font;

    public Log(Renderer renderer)
    {
        _renderer = renderer;
        Width = 600;
        Height = 200;
        Background = new SolidBrush("#2a2829");
        //TODO: can't load fonts
        // _font = renderer.fontSystem.GetFont(30);
    }

    public void OnLogUpdated(string newMessage)
    {
        Widgets.Insert(0, new HorizontalSeparator { Color = Color.Transparent });
        Widgets.Insert(0, new Label
        {
            Text = newMessage, Left = 100,
            // Font = _renderer.game.font
        });
    }
}