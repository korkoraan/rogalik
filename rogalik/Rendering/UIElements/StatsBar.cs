using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace rogalik.Rendering.UIElements;

public sealed class StatsBar : Grid
{
    private Label _nameLabel;
    private Label _lvlLabel;
    private Label _hpLabel;

    public StatsBar()
    {
        Border = new SolidBrush(Color.Gray);
        BorderThickness = new Thickness(1);
        ShowGridLines = true;
        RowsProportions.Add(new Proportion());
        RowsProportions.Add(new Proportion());
        ColumnsProportions.Add(new Proportion());
        ColumnsProportions.Add(new Proportion());

        var player = Global.GetPlayerData();
        _nameLabel = new Label { Text = player.name };
        SetColumn(_nameLabel, 0);
        SetRow(_nameLabel, 0);
        _lvlLabel = new Label { Text = $" LVL: {player.lvl} experience points: {player.expPoints} / {player.expToNextLvl} " };
        SetColumn(_lvlLabel, 1);
        SetRow(_lvlLabel, 0);
        var h = Global.GetPlayerHealthStatus();
        _hpLabel = new Label { Text = " HP: " + h.healthPts + " / " + h.healthPtsLimit };
        SetColumn(_hpLabel, 0);
        SetRow(_hpLabel, 1);
        
        Widgets.Add(_nameLabel);
        Widgets.Add(_lvlLabel);
        Widgets.Add(_hpLabel);
    }

    public void UpdatePlayerInfo(StatsMenu.PlayerData playerData)
    {
        _hpLabel.Text =
            $" LVL: {playerData.lvl} experience points: {playerData.expPoints} / {playerData.expToNextLvl} ";
    }

    public void UpdateHealthStatus(Global.HealthStatus healthStatus)
    {
        _hpLabel.Text = " HP: " + healthStatus.healthPts + " / " + healthStatus.healthPtsLimit ;
    }
}