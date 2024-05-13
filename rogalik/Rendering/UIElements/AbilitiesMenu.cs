using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using rogalik.Framework;

namespace rogalik.Rendering.UIElements;

public sealed class AbilitiesMenu : Grid, IInputListener
{
    public List<AbilityData> abilityDatas;
    private readonly ListView _abilitiesListView;
    private VerticalStackPanel _selectedAbilityPanel;
    private readonly VerticalStackPanel _noAbilitiesPanel;
    private readonly Dictionary<int, VerticalStackPanel> _abilityPanels = new ();
    private Renderer _renderer;
    public AbilitiesMenu(Renderer renderer)
    {
        _renderer = renderer;
        Width = 900;
        Height = 600;
        Background = new SolidBrush("#353634");
        BorderThickness = new Thickness(5);
        Border = new SolidBrush(Color.Gray);
        Visible = false;
        ColumnsProportions.Add( new Proportion());
        ColumnsProportions.Add( new Proportion(ProportionType.Fill));
       
        _abilitiesListView = new ListView
        {
            Background = new SolidBrush(Color.DarkRed), 
            Width = Width / 4,
            Height = Height
        };
        _abilitiesListView.SelectedIndexChanged += (_, _) =>
        {
            var index = _abilitiesListView.SelectedIndex;
            if(index == 0) return; // why does it allow to select the first separator? Looking at you, Myra >:|
            SetSelectedAbilityPanel(index is not null ? _abilityPanels[(int)index] : _noAbilitiesPanel);
        };
        SetColumn(_abilitiesListView, 0);
        Widgets.Add(_abilitiesListView);
        abilityDatas = GetAbilityDatas();
        foreach (var data in abilityDatas)
        {
            AddAbility(data);
        }

        _noAbilitiesPanel = new VerticalStackPanel
        {
            Widgets = 
            { 
                new Label
                {
                    Text = "You have no abilities",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            }
        };

        _selectedAbilityPanel = _abilityPanels.Count == 0
            ? _selectedAbilityPanel = _noAbilitiesPanel
            : _selectedAbilityPanel = _abilityPanels.First().Value;
       
        SetSelectedAbilityPanel(_selectedAbilityPanel);
       
        renderer.game.input.InputActionsPressed += OnInputActionsPressed;
    }

    private void AddAbility(AbilityData data)
    {
        _abilitiesListView.Widgets.Add(new VerticalSeparator { Height = 15, Color = Color.Transparent });
        _abilitiesListView.Widgets.Add(new Label { Text = data.name, HorizontalAlignment = HorizontalAlignment.Center } );
        var index = _abilitiesListView.Widgets.Count - 1;
        var panel = new VerticalStackPanel
        {
            Widgets =
            {
                new VerticalSeparator { Height = 50, Color = Color.Transparent },
                new Image
                {
                    Renderable = new TextureRegion(data.texture),
                    ResizeMode = ImageResizeMode.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                new VerticalSeparator { Height = 100, Color = Color.Transparent },
                new Label {Text = data.name, HorizontalAlignment = HorizontalAlignment.Center, Scale = new Vector2(2)},
                new VerticalSeparator { Height = 25, Color = Color.Transparent },
                new Label{Text = "type: " + data.type, HorizontalAlignment = HorizontalAlignment.Center},
                new VerticalSeparator { Height = 25, Color = Color.Transparent },
                new Label{Text = data.description, HorizontalAlignment = HorizontalAlignment.Center}
            }, 
        };
        _abilityPanels[index] = panel;
    }

    /// <summary>
    /// Displays the selected ability panel
    /// </summary>
    private void SetSelectedAbilityPanel(VerticalStackPanel selectedAbilityPanel)
    {
        Widgets.Remove(_selectedAbilityPanel);
        _selectedAbilityPanel = selectedAbilityPanel;
        SetColumn(_selectedAbilityPanel, 1);
        Widgets.Add(_selectedAbilityPanel);
    }

    private List<AbilityData> GetAbilityDatas()
    {
        return new List<AbilityData>
        {
            new ("Deadly sneeze", "Sneeze with your mouth closed.", "active",R.Icons.deadlySneeze, InputAction.ability1, "1"),
            new ("Fireball", "Throw a ball of fire at a point which explodes upon impact.", "target point", R.Icons.fireball, InputAction.ability2, "2"),
            new ("Sneak", "Enter a stealth mode, making you harder to notice.","toggle", R.Icons.sneak, InputAction.ability3, "3"),
            new ("Devour", "Eat a selected corpse.","target unit", R.Icons.devour, InputAction.ability4, "4"),
            new ("Hell fire", "Summon an area of engulfing unholy flame.","target unit", R.Icons.hellFire, InputAction.ability5, "5"),
            new ("Frost beam", "Cast a beam of energy that frosts everything in it's way.","target unit", R.Icons.frostBeam, InputAction.ability6, "6"),
        };
    }
    

    public void Toggle()
    {
        Visible = !Visible;

        if (_renderer.state == Renderer.State.abilitiesMenu)
        {
            _renderer.state = Renderer.State.none;
            ControlRelinquished?.Invoke();
            return;
        }
       
        _renderer.game.input.MakeSoloListener(this);
        _renderer.state = Renderer.State.abilitiesMenu;
        _abilitiesListView.SetKeyboardFocus();
    } 

    public void OnInputActionsPressed(List<InputAction> keys)
    {
        if (keys.Contains(InputAction.toggleAbilitiesMenu))
            Toggle();
    }

    public event IInputListener.ControlRelinquishedHandler ControlRelinquished;
}

public class AbilityData
{
    public string name;
    public string description;
    public string type;
    public Texture2D texture;
    public InputAction inputAction;
    public string controlBtnName;

    //TODO: Add more constructors
    public AbilityData(string name, string description, string type, Texture2D texture, InputAction inputAction, string controlBtnName)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.texture = texture;
        this.inputAction = inputAction;
        this.controlBtnName = controlBtnName;
    }
}