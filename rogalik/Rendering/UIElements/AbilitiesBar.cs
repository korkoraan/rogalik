using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using rogalik.Framework;

namespace rogalik.Rendering.UIElements;

public sealed class AbilitiesBar : Grid
{
        private AbilitiesMenu _abilitiesMenu;
        private AbilityButton[] _buttons;
        private bool _mouseOnBtn = false;

        private sealed class AbilityButton : Button, IInputListener
        {
            public AbilityData data;
            private bool _mouseIsOnMe;
            private Panel _infoPanel;
            private int _infoPanelDelay = 2000;
            private bool _infoPanelShown;
            private InputAction _toggleAction;

            public AbilityButton(AbilityData data)
            {
                this.data = data;
                _toggleAction = data.inputAction;
                var icon = new Image
                {
                    Renderable = new TextureRegion(data.texture),
                    ResizeMode = ImageResizeMode.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var controlBtnLabel = new Label { Text = data.controlBtnName, Background = new SolidBrush(Color.Black)};
            
                var panel = new Panel { Width = 50, Height = 50, Widgets = { icon } };
                panel.Widgets.Add(controlBtnLabel);
                Content = panel;
                Click += (_, _) =>
                {
                    HideInfoPanel();
                    Toggle();
                };

                _infoPanel = new Panel
                {
                    MinHeight = 200,
                    MinWidth = 330,
                    Background = new SolidBrush(new Color(Color.DimGray, 220)),
                    Widgets =
                    {
                        new Label {Text = this.data.name, HorizontalAlignment = HorizontalAlignment.Center},
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

            public void Toggle()
            {
                UIData.AddLogMessage("using " + data.name);
            }

            public void OnInputActionsPressed(List<InputAction> keys)
            {
                if (keys.Contains(_toggleAction)) Toggle();
            }

            public event IInputListener.ControlRelinquishedHandler ControlRelinquished;
        }

        public AbilitiesBar(Renderer renderer, AbilitiesMenu abilitiesMenu)
        {
            ShowGridLines = true;
            ColumnSpacing = 8;
            RowSpacing = 8;
            Background = new SolidBrush(Color.Transparent);
            GridLinesColor = Color.DimGray;
            
            _abilitiesMenu = abilitiesMenu;

            foreach (var _ in abilitiesMenu.abilityDatas)
            {
                ColumnsProportions.Add(new Proportion());                
            }

            _buttons = new AbilityButton[abilitiesMenu.abilityDatas.Count];
            for(var i = 0; i < abilitiesMenu.abilityDatas.Count; i++)
            {
                _buttons[i] = new AbilityButton(abilitiesMenu.abilityDatas[i]);
                SetColumn(_buttons[i], i);
                Widgets.Add(_buttons[i]);
                renderer.game.input.InputActionsPressed += _buttons[i].OnInputActionsPressed;
            }
        }
    }