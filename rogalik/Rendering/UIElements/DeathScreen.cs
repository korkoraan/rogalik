using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using rogalik.Rendering.Parts;

namespace rogalik.Rendering.UIElements;

public sealed class DeathScreen : VerticalStackPanel
{
    public DeathScreen()
    {
        Background = new SolidBrush(Color.Black);
        Width = 400;
        Height = 300;
        Visible = false;
        var player = Global.GetPlayerData();
        Widgets.Add(new Label { Text = $"{player.name} has died", HorizontalAlignment = HorizontalAlignment.Center });
        Widgets.Add(new Icon(Width, Height, R.Images.deathScreen) { HorizontalAlignment = HorizontalAlignment.Center });   
    }

    public void OnPlayerDied()
    {
        Visible = true;
    }
}