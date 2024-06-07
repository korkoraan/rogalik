using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using rogalik.Common;
using rogalik.Framework;
using rogalik.Rendering.Parts;

namespace rogalik.Rendering.UIElements;

public sealed class StatsMenu : Grid
{
    public StatsMenu(Renderer renderer, CharacterScreen characterScreen)
    {
        Width = characterScreen.contentPanel.Width;
        Height = characterScreen.contentPanel.Height;
        Background = new SolidBrush(Color.DarkSlateGray);
        ShowGridLines = true;
        Border = new SolidBrush(Color.Gray);
        BorderThickness = new Thickness(5);
        ColumnsProportions.Add(new Proportion());
        ColumnsProportions.Add(new Proportion());
        ColumnsProportions.Add(new Proportion());

        var playerData = Global.GetPlayerData();
        var generalInfoGrid = new GeneralInfoGrid(playerData, Width / 2, Height) ;
        var skillsList = new SkillsList(playerData, GetSkills(), Width / 4, Height);
        var selectedSkillPanel = new SelectedSkillPanel(playerData, Width / 4, Height);
        skillsList.SelectedIndexChanged += (_, _) =>
        {
            if (skillsList.SelectedItem is SkillListItem item)
            {
                selectedSkillPanel.DisplaySkillInfo(item.skill);
            }
        };
        SetColumn(generalInfoGrid, 0);
        SetColumn(skillsList, 1);
        SetColumn(selectedSkillPanel, 2);
        Widgets.Add(generalInfoGrid);
        Widgets.Add(skillsList);
        Widgets.Add(selectedSkillPanel);
    }

    private sealed class GeneralInfoGrid : Grid
    {
        private int _attrPts = 5;
        public int attrPts
        {
            get => _attrPts;

            set
            {
                _attrPts = value;
                _attrPtsLabel.Text = $"attribute points: {_attrPts}";
            }
        }
        private Label _attrPtsLabel;
        private Label _lvlLabel;
        private Button _upgradeAttrsBtn;
        private HorizontalStackPanel _stats;
        
        public GeneralInfoGrid(PlayerData data, int? width, int? height)
        {
            ShowGridLines = true;
            Width = width;
            Height = height;
            RowSpacing = 25;
            ShowGridLines = true;
            RowsProportions.Add(new Proportion());
            RowsProportions.Add(new Proportion());
            RowsProportions.Add(new Proportion());
            RowsProportions.Add(new Proportion());
            var name = new Label { Text = $"{ data.name }, a {data.gender} {data.race} {data.carreer}", Left = 50, Top = 5};
            SetRow(name, 0);
            var horPanel = new HorizontalStackPanel {Spacing = 100};
            _lvlLabel = new Label { Text = $"level {data.lvl}", Left = 50};
            _attrPtsLabel = new Label { Text = $"attribute points {attrPts}" };
            horPanel.Widgets.Add(_lvlLabel);
            horPanel.Widgets.Add(_attrPtsLabel);
            SetRow(horPanel, 1);
            _stats = new HorizontalStackPanel { Spacing = 10, HorizontalAlignment = HorizontalAlignment.Center};
            SetRow(_stats, 2);
            _upgradeAttrsBtn = new Button { Content = new Label { Text = "UPGRADE" }, HorizontalAlignment = HorizontalAlignment.Center, Visible = false };
            SetRow(_upgradeAttrsBtn, 3);
            
            
            Widgets.Add(name);
            Widgets.Add(horPanel);
            Widgets.Add(_stats);
            Widgets.Add(_upgradeAttrsBtn);
            
            foreach (var attrWidget in data.attributes.Select(attr => new AttrGrid(attr, this)))
            {
                _stats.Widgets.Add(attrWidget);
                attrWidget.AttributeChanged += () =>
                {
                    if (_stats.Widgets.Any(w => w is AttrGrid attrGrid && attrGrid.attrValueChange != 0))
                        _upgradeAttrsBtn.Visible = true;
                    else
                        _upgradeAttrsBtn.Visible = false;
                };
            }
            _upgradeAttrsBtn.Click += (_, _) =>
            {
                // INSERT UPGRADING ATTRIBUTES CODE
                var upgradedAttrs = _stats.Widgets.OfType<AttrGrid>().Where(attrGrid => attrGrid.attrValueChange > 0)
                    .Select(attrGrid => (attrGrid.attr, attrGrid.attrValueChange));
                foreach (var (attr, valueChange) in upgradedAttrs)
                {
                    UIData.AddLogMessage(attr.name + " is upgraded by " + valueChange);
                }
                // DON'T FORGET TO RESET ATTRGRIDS 
            };
        }

        private sealed class AttrGrid : Grid
        {
            public Attribute attr;
            public int attrValueChange { get; private set; }
            public delegate void AttributeChangedHandler();
            public event AttributeChangedHandler AttributeChanged;
            private Label _label;
            
            public AttrGrid(Attribute attr, GeneralInfoGrid generalInfoGrid)
            {
                this.attr = attr;
                attrValueChange = 0;
                Border = new SolidBrush(Color.Gray);
                BorderThickness = new Thickness(1);
                ColumnsProportions.Add(new Proportion());
                ColumnsProportions.Add(new Proportion());
                RowsProportions.Add(new Proportion());
                RowsProportions.Add(new Proportion());
                
                var infoPanel = new Panel { Width = 200, Height = 100, Widgets = { new Label { Text = attr.desc } } };
                var infoIcon = new InformativeIcon(75, 75, attr.texture, infoPanel)
                {
                    HorizontalAlignment = HorizontalAlignment.Center, infoPanelDelay = 500
                };
                SetColumn(infoIcon, 0);
                SetRow(infoIcon, 0);

                var btnsPanel = new VerticalStackPanel {VerticalAlignment = VerticalAlignment.Center};
                var plusBtn = new Button { Content = new Label { Text = "[+]" } };
                var minusBtn = new Button { Content = new Label { Text = "[-]" } };
                btnsPanel.Widgets.Add(plusBtn);
                btnsPanel.Widgets.Add(minusBtn);
                SetColumn(btnsPanel, 1);
                SetRow(btnsPanel, 0);
                
                _label = new Label
                {
                    Text = attr.name + ": " + attr.value, HorizontalAlignment = HorizontalAlignment.Center
                };
                SetColumn(_label, 0);
                SetRow(_label, 1);
                SetColumnSpan(_label, 2);
                
                Widgets.Add(infoIcon);
                Widgets.Add(btnsPanel);
                Widgets.Add(_label);
                
                plusBtn.Click += (_, _) =>
                {
                    if(generalInfoGrid.attrPts - 1 < 0) return;
                    attrValueChange++;
                    if (generalInfoGrid.attrPts > 0)
                    {
                        generalInfoGrid.attrPts--;
                        _label.Text = attr.name + ": " + (attr.value + attrValueChange);
                    }
                    if (attrValueChange > 0)
                        _label.TextColor = Color.Green;
                    AttributeChanged?.Invoke();
                };
                minusBtn.Click += (_, _) =>
                {
                    if(attrValueChange == 0) return;
                    attrValueChange--;
                    generalInfoGrid.attrPts++;
                    _label.Text = attr.name + ": " + (attr.value - attrValueChange);
                    if (attrValueChange == 0)
                        _label.TextColor = Color.White;
                    AttributeChanged?.Invoke();
                };
            }
        }
    }

    private class SkillsList : ListViewG
    {
        public SkillsList(PlayerData playerData, IEnumerable<Skill> skills, int? width, int? height)
        {
            Background = new SolidBrush(Color.DarkSlateGray);
            Width = width;
            Height = height;
            // receivesInput = false;
            foreach (var skillGroup in skills.GroupBy(skill => skill.category))
            {
                var skillCategoryBtn = new SkillCategoryButton(skillGroup.Key, Width, 25);
                foreach (var skill in skillGroup)
                {
                    skillCategoryBtn.skillListItems.Add(new SkillListItem(skill, height: 25));
                }
                skillCategoryBtn.Click += (_, _) =>
                {
                    var index = Widgets.IndexOf(skillCategoryBtn);
                    if (skillCategoryBtn.opened)
                    {
                        skillCategoryBtn.skillListItems.ForEach(item => Widgets.Insert(index+1, item));
                    }
                    else
                    {
                        skillCategoryBtn.skillListItems.ForEach(item => Widgets.Remove(item));
                    }
                };
                Widgets.Add(new VerticalSeparator {Width = 50});
                Widgets.Add(skillCategoryBtn);
                Widgets.Add(new VerticalSeparator {Width = 50});
            }
        }
    }

    private sealed class SkillCategoryButton : Button
    {
        public List<SkillListItem> skillListItems = new ();
        public bool opened { get; private set; }

        public SkillCategoryButton(SkillCategory category, int? width, int? height)
        {
            var horPanel = new HorizontalStackPanel();
            Background = new SolidBrush(Color.Black);
            Width = width;
            Height = height;
            var plusMinus = new Label {Text = "[+]"};
            horPanel.Widgets.Add(plusMinus);
            horPanel.Widgets.Add(new HorizontalSeparator {Width = 50, Color = Color.Transparent});
            horPanel.Widgets.Add(new Label {Text = category.ToString().ToUpper()});
            Content = horPanel;
            
            Click += (_, _) =>
            {
                opened = !opened;
                plusMinus.Text = opened ? "[-]" : "[+]";
            };
        }
    }
    
    private sealed class SkillListItem : Grid
    {
        public readonly Skill skill;
        public bool learned { get; private set; }
        public SkillListItem(Skill skill, int? width = 0, int? height = 0, bool learned = false)
        {
            this.learned = learned;
            this.skill = skill;
            if(width != 0)
                Width = width;
            if(height != 0)
                Height = height;
            Background = new SolidBrush(Color.Gray);
            
            ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            var startSign = new Label
            {
                Text = " | ".ToUpper(), VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Label nameLabel;
            if (this.skill.name != null)
            {
                nameLabel = new Label { Text = skill.name };
            }
            else if (skill.gainedAbilities?.Count() > 0)
            {
                var text = "Learn " + skill.gainedAbilities.First().name;
                if (skill.gainedAbilities?.Count() > 1)
                    text += ",...";
                nameLabel = new Label {Text = text};
            }
            else
            {
                nameLabel = new Label { Text = "unknown skill" };
            }
            nameLabel.VerticalAlignment = VerticalAlignment.Center;
            
            var learnBtn = new Button
            {
                Content = new Label { Text = "LEARN" },
                VerticalAlignment = VerticalAlignment.Center,
                Visible = !this.learned,
                Left = -10
            };
            learnBtn.Click += (_, _) =>
            {
                if (LearnSkill(skill))
                    this.learned = true;
                learnBtn.Visible = false;
                nameLabel.TextColor = Color.Green;
            };
            
            SetColumn(startSign, 0);
            SetColumn(nameLabel, 1);
            SetColumn(learnBtn, 2);
            Widgets.Add(startSign);
            Widgets.Add(nameLabel);
            Widgets.Add(learnBtn);
        }

        public bool LearnSkill(Skill skill1)
        {
            return true;
        }
    }
    
    private sealed class SelectedSkillPanel : VerticalStackPanel
    {
        private readonly PlayerData _playerData;

        public SelectedSkillPanel(PlayerData playerData, int? width, int? height)
        {
            _playerData = playerData;
            Width = width;
            Height = height;
        }

        public void DisplaySkillInfo(Skill skill)
        {
            Widgets.Clear();
            Widgets.Add(new CostPanel(skill, _playerData) { HorizontalAlignment = HorizontalAlignment.Center, Top = 15});
            Widgets.Add(new VerticalSeparator {Height = 50, Color = Color.Transparent});
            if (skill is { name: not null, attributeModifiers: not null, desc: not null })
                Widgets.Add(new SubSection(skill.name, skill.desc, skill.attributeModifiers));
            if(skill.gainedAbilities == null) return;
            foreach (var ability in skill.gainedAbilities)
            {
                Widgets.Add(new SubSection(ability));
            }
        }

        private sealed class CostPanel : Grid
        {
            public CostPanel(Skill skill, PlayerData player)
            {
                ShowGridLines = true;
                ColumnsProportions.Add(new Proportion(ProportionType.Part, 2));
                ColumnsProportions.Add(new Proportion(ProportionType.Part, 2));
                
                var horPanelLVL = new HorizontalStackPanel {HorizontalAlignment = HorizontalAlignment.Center};
                horPanelLVL.Widgets.Add(new Label
                {
                    Text = "LVL required: ",
                });
                var available = player.lvl >= skill.lvlCap;
                horPanelLVL.Widgets.Add(new Label
                {
                    Text = skill.lvlCap.ToString(),
                    TextColor = available ? Color.Green : Color.Red,
                });
                SetColumn(horPanelLVL, 0);
                
                var horPanelSP = new HorizontalStackPanel {HorizontalAlignment = HorizontalAlignment.Center};
                horPanelSP.Widgets.Add(new Label
                {
                    Text = "SP required: "
                });
                available = player.skillPts >= skill.skillPtsCost;
                horPanelSP.Widgets.Add(new Label
                {
                    Text = skill.skillPtsCost.ToString(),
                    TextColor = available ? Color.Green : Color.Red
                });
                SetColumn(horPanelSP, 1);
                
                Widgets.Add(horPanelSP);
                Widgets.Add(horPanelLVL);
            }
        }

        private sealed class SubSection : VerticalStackPanel 
        {
            public SubSection(string name, string desc, IEnumerable<Attribute> attributeModifiers)
            {
                Border = new SolidBrush(Color.Gray);
                BorderThickness = new Thickness(5);
                Widgets.Add(new Headline(name));
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
                Widgets.Add(new ModiiersSection(attributeModifiers) {HorizontalAlignment = HorizontalAlignment.Center});
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
                Widgets.Add(new Headline("Description"));
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
                Widgets.Add(new Label {Text = desc, HorizontalAlignment = HorizontalAlignment.Center});
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
            }

            private sealed class ModiiersSection : HorizontalStackPanel
            {
                public ModiiersSection(IEnumerable<Attribute> attributeModifiers)
                {
                    foreach (var attr in attributeModifiers)
                    {
                        Widgets.Add(ModifierSign(attr.value));
                        Widgets.Add(new Label{Text = attr.value + " " + attr.name, });
                    }
                }
                private Widget ModifierSign(int modifier)
                {
                    if(modifier >= 0)
                        return new Label { Text = "+", TextColor = Color.Green };
                    return new Label { Text = "-", TextColor = Color.Red };
                }
            }

            public SubSection(Ability gainedAbility)
            {
                Border = new SolidBrush(Color.Gray);
                BorderThickness = new Thickness(5);
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
                if(gainedAbility.texture != null)
                    Widgets.Add(new Icon(Width / 3, Height / 3, gainedAbility.texture));
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
                Widgets.Add(new Headline(gainedAbility.name));
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
                Widgets.Add(new Headline("Description"));
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
                Widgets.Add(new Label {Text = gainedAbility.description, HorizontalAlignment = HorizontalAlignment.Center});
                Widgets.Add(new VerticalSeparator {Height = 25, Color = Color.Transparent });
            }
        }
    }

    private sealed class Headline : Grid
    {
        public Headline(string text)
        {
            ColumnsProportions.Add(new Proportion(ProportionType.Part, 3));
            ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            ColumnsProportions.Add(new Proportion(ProportionType.Part, 3));
            var separator1 = new HorizontalSeparator();
            SetColumn(separator1, 0);
            var separator2 = new HorizontalSeparator();
            SetColumn(separator2, 2);
            var label = new Label { Text = text };
            SetColumn(label, 1);
            Widgets.Add(separator1);
            Widgets.Add(separator2);
            Widgets.Add(label);
        }
    }

    private IEnumerable<Skill> GetSkills()
    {
        return new[]
        {
            new Skill
            {
                category = SkillCategory.warrior,
                name = "Resilience",
                desc = "Become as resilient as steel.",
                lvlCap = 6,
                skillPtsCost = 1,
                attributeModifiers = new [] { new Attribute("strength", 5) },
            },
            new Skill
            {
                category = SkillCategory.warrior,
                lvlCap = 2,
                skillPtsCost = 3,
                gainedAbilities = new []
                {
                    new Ability(
                        "Mighty kick",
                    "Kick anything with an unmatched fury.",
                        "target point",
                        R.Icons.Abilities.mightyKick, InputAction.ability7),
                }
            },
            new Skill
            {
                category = SkillCategory.wizard
            },
            new Skill
            {
                category = SkillCategory.thief
            }
        };
    }

    public struct Skill
    {
        public SkillCategory category;
        public string name;
        public string desc;
        public uint skillPtsCost;
        public uint lvlCap;
        public IEnumerable<Ability> gainedAbilities;
        public IEnumerable<Skill> requiredSkills;
        public IEnumerable<Attribute> attributeModifiers;
        public Texture2D texture;
    }

    public struct PlayerData
    {
        public string name;
        public string race;
        public string gender;
        public string carreer;
        public uint lvl;
        public uint expPoints;
        public uint expToNextLvl;
        public uint skillPts;
        public uint attrPts;
        public IEnumerable<Attribute> attributes;
        
    }
    
    public struct Attribute
    {
        public string name { get; }
        public string desc { get; }
        public int value { get; }
        public Texture2D texture { get; }

        public Attribute(string name, int valueModifier, string desc = "", Texture2D texture = null)
        {
            this.name = name;
            this.desc = desc;
            value = valueModifier;
            this.texture = texture;
        }
    }

    public enum SkillCategory
    {
        warrior,
        wizard,
        thief,
        other
    }
}