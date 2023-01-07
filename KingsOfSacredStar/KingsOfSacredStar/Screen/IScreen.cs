using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    /// <summary>
    /// Interface of the Screen the ScreenManager handles.
    /// </summary>
    internal interface IScreen
    {
        void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens);

        void DrawHud(SpriteBatch spriteBatch);

        void Draw(GameTime gameTime);
        bool ProcessMouseLeftClick(Point inputLastMouseClickPosition);

        bool ProcessMouseRightClick(Point inputLastMouseClickPosition);
    }
}
