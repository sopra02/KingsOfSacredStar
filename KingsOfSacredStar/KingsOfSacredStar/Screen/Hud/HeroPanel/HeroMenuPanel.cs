using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Screen.SkillMenu;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.Hud.HeroPanel
{
    internal sealed class HeroMenuPanel : AMenuPage
    {
        public HeroMenuPanel(ContentManager content) : base(GetButtons(content))
        {
        }
        public override bool ProcessMouseRightClick(Point inputLastMouseClickPosition)
        {
            return false;
        }
        private static AButton[] GetButtons(ContentManager content)
        {

            var buttons = new List<AButton>();


            CreateSkillButtons(content, buttons);
            CreateStatusButtons(content, buttons);
            CreateSkillAndRespawnButtons(content, buttons);

            return buttons.ToArray();
        }

        private static void CreateSkillAndRespawnButtons(ContentManager content, ICollection<AButton> buttons)
        {
            const int buttonPositionX = 850;
            var buttonPositionY = 690;
            const int buttonPadding = 10;
            const int buttonWidth = 210;
            const int buttonHeight = 60;
            buttons.Add(new MenuButton(content,
                buttonPositionX,
                690,
                buttonWidth,
                buttonHeight,
                new[] {Properties.HeroMenuPanel.SkillTree},
                new List<Screen> {Screen.SkillingMenu},
                new List<Screen>()));

            buttonPositionY += buttonPadding + buttonHeight;
            buttons.Add(new HeroRespawnTimerButton(content, buttonPositionX, buttonPositionY, buttonWidth, buttonHeight));
        }

        private static void CreateStatusButtons(ContentManager content, ICollection<AButton> buttons)
        {
            const int buttonPositionX = 195;
            var buttonPositionY = 800;
            const int buttonPadding = 10;
            const int buttonWidth = 905;
            const int buttonHeight = 20;


            buttons.Add(new ExperienceButton(content, buttonPositionX, buttonPositionY, buttonWidth, buttonHeight));
            buttonPositionY += buttonPadding + buttonHeight;
            buttons.Add(new HealthButton(content, buttonPositionX, buttonPositionY, buttonWidth, buttonHeight));
            buttonPositionY += buttonPadding + buttonHeight;
            buttons.Add(new ManaButton(content, buttonPositionX, buttonPositionY, buttonWidth, buttonHeight));
        }

        private static void CreateSkillButtons(ContentManager content, ICollection<AButton> buttons)
        {
            var skills = new List<Skills>
            {
                Skills.Skill1,
                Skills.Skill2,
                Skills.Skill3
            };
            var buttonPositionX = 195;
            const int buttonPadding = 15;
            const int buttonWidth = 100;

            var hero = GameState.Current.HeroesByPlayer[Players.Player];

            foreach (var skill in skills)
            {
                var skillText = new[]
                {
                    hero.HeroSkills.GetName(skill),
                    hero.HeroSkills.GetManaCost(skill).ToString(),
                    hero.HeroSkills.GetRemainingCooldown(skill).ToString()
                };
                buttons.Add(new SkillExecuteButton(content,
                    buttonPositionX,
                    690,
                    buttonWidth,
                    100,
                    skillText,
                    skill));
                buttonPositionX += buttonPadding + buttonWidth;
            }
        }

        protected override bool CheckEsc(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            return false;
        }
    }
}