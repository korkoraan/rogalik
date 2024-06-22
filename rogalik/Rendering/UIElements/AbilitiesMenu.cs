using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using rogalik.Framework;
using rogalik.Rendering.Parts;

namespace rogalik.Rendering.UIElements;

public sealed class AbilitiesMenu : Grid, IInputListener
{
    public IEnumerable<Ability> abilities
    {
        get
        {
            foreach (var abilityPanel in _abilitiesListView.Widgets.Where(w => w is AbilityListItem))
            {
                yield return (abilityPanel as AbilityListItem)?.ability;
            }
        }
    }
    private readonly ListViewG _abilitiesListView;
    private AbilityInfoPanel _selectedAbilityInfoPanel;
    private readonly AbilityInfoPanel _noAbilitiesInfoListItem;
    private readonly Dictionary<int, AbilityInfoPanel> _abilityPanels = new ();
    private Renderer _renderer;
    private State _state = State.none;
    
    public delegate void AbilityReboundHandler(Ability ability, InputAction oldAction, InputAction newAction);
    public event AbilityReboundHandler AbilityRebound;
    
    private enum State
    {
        none,
        bindingAbilityKey,
    }
    public AbilitiesMenu(Renderer renderer, CharacterScreen characterScreen)
    {
        _renderer = renderer;
        Width = characterScreen.contentPanel.Width;
        Height = characterScreen.contentPanel.Height;
        
        Background = new SolidBrush("#353634");
        BorderThickness = new Thickness(5);
        Border = new SolidBrush(Color.Gray);
        ColumnsProportions.Add( new Proportion());
        ColumnsProportions.Add( new Proportion(ProportionType.Fill));
       
        _abilitiesListView = new ListViewG
        {
            Background = new SolidBrush(Color.DarkRed), 
            Width = Width / 4,
            Height = Height
        };
        _abilitiesListView.SelectedIndexChanged += (_, _) =>
        {
            var index = _abilitiesListView.SelectedIndex;
            if(index == 0) return;
            SetSelectedAbilityPanel(index is not null ? _abilityPanels[(int)index] : _noAbilitiesInfoListItem);
        };
        SetColumn(_abilitiesListView, 0);
        Widgets.Add(_abilitiesListView);
        foreach (var data in Global.GetPlayerAbilities())
        {
            AddAbility(data);
        }

        _noAbilitiesInfoListItem = new AbilityInfoPanel(new Ability("You have to abilities", "", "", R.Icons.clownfish)); 

        _selectedAbilityInfoPanel = _abilityPanels.Count == 0
            ? _selectedAbilityInfoPanel = _noAbilitiesInfoListItem
            : _selectedAbilityInfoPanel = _abilityPanels.First().Value;
       
        SetSelectedAbilityPanel(_selectedAbilityInfoPanel);
    }

    public sealed class AbilityListItem : Panel
    {
        public readonly Ability ability;
        public Label controlBtnLabel;
        
        public AbilityListItem(Ability data)
        {
            this.ability = data;
            Widgets.Add(new Label { Text = data.name, HorizontalAlignment = HorizontalAlignment.Center });
            var controlBtnName = data.inputAction.ToString().Last().ToString();
            controlBtnLabel = new Label 
            {
                Text = controlBtnName,
                Background = new SolidBrush(Color.Black),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Widgets.Add(controlBtnLabel);
        }
    }
    
    private void AddAbility(Ability ability)
    {
        _abilitiesListView.Widgets.Add(new VerticalSeparator { Height = 15, Color = Color.Transparent });
        var listItem = new AbilityListItem(ability);
        _abilitiesListView.Widgets.Add(listItem);
        var index = _abilitiesListView.Widgets.Count - 1;
        var panel = new AbilityInfoPanel(ability);
        _abilityPanels[index] = panel;
    }

    private sealed class AbilityInfoPanel : Panel
    {
        public Ability ability;
        public Label messageLabel;
        private VerticalStackPanel _contentPanel;

        public AbilityInfoPanel(Ability ability)
        {
            this.ability = ability;
            _contentPanel = new VerticalStackPanel();
            _contentPanel.Widgets.Add(new VerticalSeparator { Height = 100, Color = Color.Transparent });
            _contentPanel.Widgets.Add(new Icon(100, 100, ability.texture)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });    
            _contentPanel.Widgets.Add(new VerticalSeparator { Height = 100, Color = Color.Transparent });
            _contentPanel.Widgets.Add(new Label {Text = ability.name, HorizontalAlignment = HorizontalAlignment.Center, Scale = new Vector2(2)});
            _contentPanel.Widgets.Add(new VerticalSeparator { Height = 25, Color = Color.Transparent });
            _contentPanel.Widgets.Add(new Label{Text = "type: " + ability.type, HorizontalAlignment = HorizontalAlignment.Center});
            _contentPanel.Widgets.Add(new VerticalSeparator { Height = 25, Color = Color.Transparent });
            _contentPanel.Widgets.Add(new Label{Text = ability.description, HorizontalAlignment = HorizontalAlignment.Center});
            Widgets.Add(_contentPanel);
            
            messageLabel = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Top = -10,
                Text = "PRESS ENTER TO BIND AN ABILITY"
            };
            Widgets.Add(messageLabel);
        }
    }

    /// <summary>
    /// Displays the selected ability panel
    /// </summary>
    private void SetSelectedAbilityPanel(AbilityInfoPanel selectedAbilityInfoPanel)
    {
        Widgets.Remove(_selectedAbilityInfoPanel);
        _selectedAbilityInfoPanel = selectedAbilityInfoPanel;
        SetColumn(_selectedAbilityInfoPanel, 1);
        Widgets.Add(_selectedAbilityInfoPanel);
    }
    
    public void OnInputActionsPressed(List<InputAction> actions)
    {
        if (_state == State.none) // switch?
        {
            if (actions.Contains(InputAction.UIselectDown) || actions.Contains(InputAction.UIselectUp))
                _abilitiesListView.SetKeyboardFocus();
            if (actions.Contains(InputAction.UIselect) && _abilitiesListView.SelectedItem != null)
            {
                _state = State.bindingAbilityKey;
                _abilitiesListView.receivesInput = false;
                _abilitiesListView.Enabled = false;
                _selectedAbilityInfoPanel.messageLabel.Text = "CHOOSE A NUMBER BETWEEN 0-9";
            }
            return;
        }
        if (_state == State.bindingAbilityKey)
        {
            InputAction? newAction = null;
            if (actions.Contains(InputAction.ability1))
                newAction = InputAction.ability1;
            if (actions.Contains(InputAction.ability2))
                newAction = InputAction.ability2;
            if (actions.Contains(InputAction.ability3))
                newAction = InputAction.ability3;
            if (actions.Contains(InputAction.ability4))
                newAction = InputAction.ability4;
            if (actions.Contains(InputAction.ability5))
                newAction = InputAction.ability5;
            if (actions.Contains(InputAction.ability6))
                newAction = InputAction.ability6;
            if (actions.Contains(InputAction.ability7))
                newAction = InputAction.ability7;
            if (actions.Contains(InputAction.ability8))
                newAction = InputAction.ability8;
            if (actions.Contains(InputAction.ability9))
                newAction = InputAction.ability9;
            if (actions.Contains(InputAction.ability10))
                newAction = InputAction.ability10;
            if (newAction != null && _abilitiesListView.SelectedItem is AbilityListItem abilityListItem)
            {
                var ability = _selectedAbilityInfoPanel.ability;
                var oldAction = ability.inputAction; 
                ability.inputAction = (InputAction)newAction;
                AbilityRebound?.Invoke(ability, oldAction, (InputAction)newAction);
                foreach (var w in _abilitiesListView.Widgets.Where(w => w is AbilityListItem item && item.ability.inputAction == newAction))
                {
                    (w as AbilityListItem).controlBtnLabel.Text = "";
                }
                abilityListItem.controlBtnLabel.Text = newAction.ToString().Last().ToString();
            }
            
            _state = State.none;
            _abilitiesListView.receivesInput = true;
            _abilitiesListView.Enabled = true;
            _selectedAbilityInfoPanel.messageLabel.Text = "";
            return;
        }
    }

    public event IInputListener.ControlReleasedHandler ControlReleased;
}

public class Ability
{
    public string name;
    public string description;
    public string type;
    public Texture2D texture;
    public InputAction inputAction;

    //TODO: Add more constructors
    public Ability(string name, string description, string type, Texture2D texture)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.texture = texture;
    }

    public Ability(string name, string description, string type, Texture2D texture,
        InputAction inputAction) : this(name, description, type, texture)
    {
        this.inputAction = inputAction;
    }
}