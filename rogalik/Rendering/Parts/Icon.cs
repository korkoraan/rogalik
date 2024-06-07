using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace rogalik.Rendering.Parts;

public class Icon : Panel
{
    protected Image _image;
    private Texture2D _texture;
    public readonly int? width;
    public readonly int? height;
    /// <summary>
    /// Change the value to change icon's picture
    /// </summary>
    public Texture2D texture
    {
        get => _texture;
        set
        {
            _texture = value;
            Widgets.Clear();
            _image = new Image
            {
                Width = width,
                Height = height,
                Renderable = new TextureRegion(texture),
                ResizeMode = ImageResizeMode.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            Widgets.Add(_image);
        }
    }
    
    public Icon(int? width, int? height)
    {
        Width = width;
        Height = height;
    }
    
    public Icon(int? width, int? height, Color backgroundColor) : this(width, height)
    {
        Background = new SolidBrush(backgroundColor);
    }

    public Icon(int? width, int? height, Texture2D texture) : this(width, height)
    {
        this.texture = texture;
    }
}

public class InformativeIcon : Icon
{
    private bool _mouseIsOnMe;
    public int infoPanelDelay = 2000;
    private bool _infoPanelShown;
    public readonly Widget popUpInfoWidget;
    
    public InformativeIcon(int? width, int? height, Texture2D texture, Widget popUpInfoWidget) : base(width, height, texture)
    {
        this.popUpInfoWidget = popUpInfoWidget;
        
        MouseEntered += async (_, _) =>
        {
            var v = _image.Color.ToVector3();
            _image.Color = new Color(v.X * .8f, v.Y * .8f, v.Z * .8f);
            _mouseIsOnMe = true;
            bool mouseKept = await MouseKeptTimer();
            if(mouseKept) ShowInfoPanel();
        };
        MouseLeft += (_, _) =>
        {
            _mouseIsOnMe = false;
            _image.Color = Color.White;
        };
        popUpInfoWidget.MouseLeft += (_, _) => HideInfoPanel();
        popUpInfoWidget.TouchDown += (_, _) => HideInfoPanel();
        MouseMoved += (_, _) =>
        {
            if(_infoPanelShown) HideInfoPanel();
        };
    }
    
    private void ShowInfoPanel()
    {
        Desktop.ShowContextMenu(popUpInfoWidget, Desktop.MousePosition);
        _infoPanelShown = true;
    }

    private void HideInfoPanel()
    {
        Desktop.HideContextMenu();
        _infoPanelShown = false;
    }

    private async Task<bool> MouseKeptTimer()
    {
        await Task.Delay(infoPanelDelay);
        return _mouseIsOnMe;
    }
}