using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using rogalik.Framework;

namespace rogalik.Rendering.UIElements;

public sealed class AbilitiesBar : Grid
{
    private readonly Renderer _renderer;
    private readonly AbilitiesMenu _abilitiesMenu;
    private Button[] _buttons = new Button[10];
    private bool _mouseOnBtn = false;
    private readonly int btnWidth = 50;
    private readonly int btnHeight = 50;

    public delegate void AbilityUsedHandler(Ability ability);
    public event AbilityUsedHandler AbilityUsed;

    public AbilitiesBar(Renderer renderer, AbilitiesMenu abilitiesMenu)
    {
        _renderer = renderer;
        ShowGridLines = true;
        ColumnSpacing = 8;
        RowSpacing = 8;
        Background = new SolidBrush(Color.Transparent);
        GridLinesColor = Color.DimGray;
        _abilitiesMenu = abilitiesMenu;
        
        foreach (var _ in _abilitiesMenu.abilities)
        {
            ColumnsProportions.Add(new Proportion());                
        }

        var bindedAbilities = abilitiesMenu.abilities.Where(data => data.inputAction != default).ToArray();
        for(var i = 0; i < _buttons.Length; i++)
        {
            if(i < bindedAbilities.Length)
                SetAbilityBtn(bindedAbilities[i], i);
            else
                ClearAbilityBtn(i);
        }
        
        _abilitiesMenu.AbilityRebound += (ability, oldAction, newAction) =>
        {
            ClearAbilityBtn((int)(AbilityActionToInt(oldAction) - 1));
            ClearAbilityBtn((int)AbilityActionToInt(newAction) - 1);
            
            SetAbilityBtn(ability, (int)AbilityActionToInt(newAction) - 1);
        };
    }

    private void SetAbilityBtn(Ability ability, int btnIndex)
    {
        var controlBtnName = AbilityActionToInt(ability.inputAction) != null
            ? AbilityActionToInt(ability.inputAction).ToString()
            : "";
        var btn = new AbilityButton(this, ability, controlBtnName, btnWidth, btnHeight);
        _buttons[btnIndex] = btn;
        SetColumn(btn, btnIndex);
        Widgets.Add(btn);
        _renderer.game.input.InputActionsPressed += btn.OnInputActionsPressed;
        btn.Click += (_, _) =>
        {
            AbilityUsed?.Invoke(btn.ability);
        };
    }
    
    private void ClearAbilityBtn(int btnIndex)
    {
        if(btnIndex > _buttons.Length || btnIndex < 0) return;
        Widgets.Remove(_buttons[btnIndex]);
        var btn = new EmptyButton(btnWidth, btnHeight);
        _buttons[btnIndex] = btn;
        SetColumn(btn, btnIndex);
        Widgets.Add(btn);
    }

    private sealed class AbilityButton : Button
    {
        public Ability ability;
        private bool _mouseIsOnMe;
        private Panel _infoPanel;
        private int _infoPanelDelay = 2000;
        private bool _infoPanelShown;
        private InputAction _toggleAction;
        public Label controlBtnLabel;
        
        public AbilityButton(AbilitiesBar owner, Ability ability, string controlBtnName, int width, int height)
        {
            this.ability = ability;
            _toggleAction = ability.inputAction;
            var icon = new Image
            {
                Renderable = new TextureRegion(ability.texture),
                ResizeMode = ImageResizeMode.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var controlBtnLabel = new Label { Text = controlBtnName, Background = new SolidBrush(Color.Black)};
        
            var panel = new Panel { Width = width, Height = height, Widgets = { icon }};
            panel.Widgets.Add(controlBtnLabel);
            Content = panel;
            Click += (_, _) =>
            {
                HideInfoPanel();
                UIData.AddLogMessage("using " + ability.name);
            };

            _infoPanel = new Panel
            {
                MinHeight = 200,
                MinWidth = 330,
                Background = new SolidBrush(new Color(Color.DimGray, 220)),
                Widgets =
                {
                    new Label {Text = this.ability.name, HorizontalAlignment = HorizontalAlignment.Center},
                },
            };
            
            MouseEntered += async (_, _) =>
            {
                var v = icon.Color.ToVector3();
                icon.Color = new Color(v.X * .8f, v.Y * .8f, v.Z * .8f);
                _mouseIsOnMe = true;
                bool mouseKept = await MouseKeptTimer();
                if(mouseKept) ShowInfoPanel();
            };
            MouseLeft += (_, _) =>
            {
                _mouseIsOnMe = false;
                icon.Color = Color.White;
            };
            _infoPanel.MouseLeft += (_, _) => HideInfoPanel();
            _infoPanel.TouchDown += (_, _) => HideInfoPanel();
            MouseMoved += (_, _) =>
            {
                if(_infoPanelShown) HideInfoPanel();
            };
        }
    
        private void ShowInfoPanel()
        {
            Desktop.ShowContextMenu(_infoPanel, Desktop.MousePosition);
            _infoPanelShown = true;
        }

        private void HideInfoPanel()
        {
            Desktop.HideContextMenu();
            _infoPanelShown = false;
        }

        private async Task<bool> MouseKeptTimer()
        {
            await Task.Delay(_infoPanelDelay);
            return _mouseIsOnMe;
        }

        public void OnInputActionsPressed(List<InputAction> keys)
        {
            if (keys.Contains(_toggleAction)) DoClick();
        }
    }

    private sealed class EmptyButton : Button
    {
        public EmptyButton(int width, int height)
        {
            var panel = new Panel
            {
                Width = width,
                Height = height,
                Background = new SolidBrush(Color.Black),
                Border = new SolidBrush(Color.White),
                BorderThickness = new Thickness(1)
            };
            Content = panel;
        }
    }
    
        //temporary solution
    private int? AbilityActionToInt(InputAction action)
    {
        var result = action switch
        {
            InputAction.ability1 => 1,
            InputAction.ability2 => 2,
            InputAction.ability3 => 3,
            InputAction.ability4 => 4,
            InputAction.ability5 => 5,
            InputAction.ability6 => 6,
            InputAction.ability7 => 7,
            InputAction.ability8 => 8,
            InputAction.ability9 => 9,
            InputAction.ability10 => 0,
            _ => -1
        };
        return result; 
    }
}
    