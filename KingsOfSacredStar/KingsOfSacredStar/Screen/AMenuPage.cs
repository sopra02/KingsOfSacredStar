using System;
using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    internal abstract class AMenuPage : IScreen
    {
        protected readonly AButton[] mButtons;
        
        protected AMenuPage(AButton[] buttons)
        {
            mButtons = buttons;
        }

        public virtual void Draw(GameTime gameTime)
        {
            // Nothing to draw
        }

        public virtual void DrawHud(SpriteBatch spriteBatch)
        {
            foreach (var button in mButtons)
            {
                button.Draw(spriteBatch, 1f);
            }
        }

        public virtual bool ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            return mButtons.Any(button => button.InBox(inputLastMouseClickPosition));
        }

        public virtual bool ProcessMouseRightClick(Point inputLastMouseClickPosition)
        {
            return mButtons.Any(button => button.InBox(inputLastMouseClickPosition));
        }

        public virtual void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (CheckEsc(addScreens, removeScreens))
                return;
            foreach (var button in mButtons)
            {
                if (button.GetClicked())
                {
                    if (button.GetTextFirstLine() == HeroMenuPanel.SkillTree)
                        GameStateWrapper.SetPause(true);
                    else if (button.GetTextFirstLine() == SkillingMenu.X)
                        GameStateWrapper.SetPause(false);
                    button.IsClicked(addScreens, removeScreens);
                }
            }
        }
        protected virtual bool CheckEsc(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens) {
            if (ExitWrapper.EscClicked)
            {
                mButtons[mButtons.Length - 1].IsClicked(addScreens, removeScreens);
                return true;
            }
            return false;
        }
    }
}
