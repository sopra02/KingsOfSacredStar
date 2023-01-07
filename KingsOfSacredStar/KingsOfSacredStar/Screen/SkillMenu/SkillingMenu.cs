using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.SkillMenu
{
    internal sealed class SkillingMenu : AMenuPage
    {
        private const int MenuPosX = 100;
        private const int MenuPosY = 100;
        private const int MenuWidth = 1000;

        private readonly ContentManager mContent;


        public SkillingMenu(ContentManager content) : base(GetButtons(content))
        {
            mContent = content;

        }

        private static AButton[] GetButtons(ContentManager content)
        {
            const int buttonPadding = 20;
            const int buttonWidth = 300;
            const int buttonHeight = 100;
            const float skillButtonSizeFactor = 0.6f;

            const float skillButtonPositionOffset = (1f - skillButtonSizeFactor) / 2;

            var hero = GameState.Current.HeroesByPlayer[Players.Player];
            var skills = hero.HeroSkills.SkillList.Keys.ToList();

            var buttons = new List<AButton>();

            var buttonPositionY = MenuPosY + buttonPadding;
            foreach (var skill in skills)
            {
                var buttonPositionX = MenuPosX + buttonPadding;
                buttons.Add(new MenuButton(content,
                    buttonPositionX,
                    buttonPositionY,
                    buttonWidth,
                    buttonHeight,
                    new[] {hero.HeroSkills.GetName(skill)}));
                buttonPositionX += buttonWidth + buttonPadding;
                buttons.Add(new SkillButton(content,
                    buttonPositionX,
                    buttonPositionY + (int) (skillButtonPositionOffset * buttonHeight),
                    (int) (skillButtonSizeFactor * buttonWidth),
                    (int) (skillButtonSizeFactor * buttonHeight),
                    skill));
                buttonPositionX += (int) (skillButtonSizeFactor * buttonWidth) + buttonPadding;
                buttons.Add(new SkillLevelUpButton(content,
                    buttonPositionX,
                    buttonPositionY + (int) (skillButtonPositionOffset * buttonHeight),
                    (int) (skillButtonSizeFactor * buttonWidth),
                    (int) (skillButtonSizeFactor * buttonHeight),
                    new[] {Properties.SkillingMenu.Plus},
                    skill));
                buttonPositionY += buttonPadding + buttonHeight;
            }

            buttons.Add(new SkillPointAvailableButton(content,
                MenuPosX + MenuWidth - buttonPadding - (int) (skillButtonSizeFactor * buttonWidth),
                buttonPositionY,
                (int) (skillButtonSizeFactor * buttonWidth),
                (int) (2 * skillButtonSizeFactor * buttonHeight)));

            buttons.Add(new ActionButton(content,
                MenuPosX + MenuWidth - buttonPadding - (int) (skillButtonSizeFactor * buttonWidth),
                MenuPosY + buttonPadding,
                (int) (skillButtonSizeFactor * buttonWidth),
                (int) (skillButtonSizeFactor * buttonHeight),
                new[] {Properties.SkillingMenu.X},
                () => { GameStateWrapper.SetPause(false); },
                default,
                new List<Screen>(),
                new List<Screen> { Screen.SkillingMenu }));


            return buttons.ToArray();
        }

        public override bool ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            base.ProcessMouseLeftClick(inputLastMouseClickPosition);
            return true;
        }

        public override bool ProcessMouseRightClick(Point inputLastMouseClickPosition)
        {
            base.ProcessMouseRightClick(inputLastMouseClickPosition);
            return true;
        }


        public override void DrawHud(SpriteBatch spriteBatch)
        {
            var background = new MenuButton(mContent, MenuPosX, MenuPosY, MenuWidth, (int) (mButtons[mButtons.Length -2].GetPosition().Y + mButtons[mButtons.Length - 1].GetSize().Y), new []{""});
            background.Draw(spriteBatch, 1f);
            base.DrawHud(spriteBatch);
        }
    }
}
