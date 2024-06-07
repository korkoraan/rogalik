using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using rogalik.Framework;

namespace rogalik.Rendering.UIElements;

public sealed class CharacterScreen : Grid, IInputListener
{
    private readonly Renderer _renderer;
    private HorizontalStackPanel _buttonsPanel;
    public readonly Panel contentPanel;
    private Dictionary<InputAction, Widget> _inputActionsToSection = new ();
    private List<Widget> _sections = new ();
    private Widget _selectedSection;
    public AbilitiesMenu abilitiesMenu;
    public EquipmentMenu equipmentMenu;
    public StatsMenu statsMenu;

    public CharacterScreen(Renderer renderer)
    {
        _renderer = renderer;
        Visible = false;
        _buttonsPanel = new HorizontalStackPanel { };
        contentPanel = new Panel
        {
            Width = 1280,
            Height = 720,
        };

        equipmentMenu = new EquipmentMenu(this);
        abilitiesMenu = new AbilitiesMenu(_renderer, this);
        statsMenu = new StatsMenu(_renderer, this); 
        AddSection(equipmentMenu, InputAction.equipmentMenu, R.Icons.equipment);
        AddSection(abilitiesMenu, InputAction.abilitiesMenu, R.Icons.abilities);
        AddSection(statsMenu, InputAction.statsMenu, R.Icons.stats);

        RowsProportions.Add(new Proportion(ProportionType.Auto));
        RowsProportions.Add(new Proportion(ProportionType.Fill));
        SetRow(_buttonsPanel, 0);
        Widgets.Add(_buttonsPanel);
        SetRow(contentPanel, 1);
        Widgets.Add(contentPanel);
        renderer.game.input.InputActionsPressed += OnInputActionsPressed;
    }

    private void AddSection(Widget section, InputAction inputAction, Texture2D icon)
    {
        _sections.Add(section);
        _inputActionsToSection[inputAction] = section;
        _buttonsPanel.Widgets.Add(new SectionBtn(section, icon, this) );
    }
    
    private sealed class SectionBtn : Button
    {
        public SectionBtn(Widget widget, Texture2D icon, CharacterScreen characterScreen)
        {
            Content = new Image
            {
                Width = 64,
                Height = 64,
                Renderable = new TextureRegion(icon),
                ResizeMode = ImageResizeMode.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            Click += (_, _) => characterScreen.Show(widget);
        }
    }

    public void OnInputActionsPressed(List<InputAction> actions)
    {
        if(_renderer.state != Renderer.State.none) return;
        
        Widget section = null;
        foreach (var action in actions)
        {
            _inputActionsToSection.TryGetValue(action, out section);
        }
        if (section != null)
        {
            if(section is IInputListener s)
                _renderer.game.input.MakeSoloListeners(this, s);
            else
                _renderer.game.input.MakeSoloListeners(this);
            Show(section);
        }
        
        if (actions.Contains(InputAction.UIswitchToNextMenu))
        {
            SwitchToNextSection();
            return;
        }
        if (actions.Contains(InputAction.exitWindow))
        {
            Hide();
        }
    }

    private void Show(Widget section)
    {
        Visible = true;
        contentPanel.Widgets.Clear();
        contentPanel.Widgets.Add(section);
        _selectedSection = section;
        if(section is IInputListener listener)
            _renderer.game.input.MakeSoloListeners(this, listener);
    }
    
    private void SwitchToNextSection()
    {
        if(_selectedSection == null) return;
        var index = _sections.IndexOf(_selectedSection);
        
        Show(_sections[index < _sections.Count - 1 ? index+1 : 0]);
    }

    private void Hide()
    {
        Visible = false;
        _selectedSection = null;
        _renderer.game.input.ReleaseListeners();
    }

    public event IInputListener.ControlReleasedHandler ControlReleased;
}