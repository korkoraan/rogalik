using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace rogalik.Rendering.UIElements;

public class Log : ListView
{
    private readonly Renderer _renderer;
    private SpriteFontBase _font;

    public Log(Renderer renderer)
    {
        AcceptsKeyboardFocus = false;
        Border = new SolidBrush(Color.Gray);
        BorderThickness = new Thickness(1);
        _renderer = renderer;
        Width = 600;
        Height = 200;
        Background = new SolidBrush("#2a2829");
        //TODO: can't load fonts
        // _font = renderer.fontSystem.GetFont(30);

        UIData.LogUpdated += OnLogUpdated;
    }

    public void OnLogUpdated(string newMessage)
    {
        Widgets.Add(new HorizontalSeparator { Color = Color.Transparent });
        Widgets.Add(new Label
        {
            Text = newMessage, Left = 100,
        });
        Desktop.UpdateLayout();
        ScrollViewer.ScrollPosition = ScrollViewer.ScrollMaximum;
    }
}