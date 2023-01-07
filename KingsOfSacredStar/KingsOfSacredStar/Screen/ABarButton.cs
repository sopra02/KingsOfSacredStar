using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    internal abstract class ABarButton : AButton
    {
        private readonly Rectangle mOriginalBarBox;

        protected ABarButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            Color color) : base(content, x, y, width, height, new string[0], boxColor: color, font: content.Load<SpriteFont>("fonts/Small"))
        {
            mOriginalBarBox = mBox;
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            // No click event
        }

        protected override float GetTextPositionX(float textSizeX)
        {
            return mOriginalBarBox.X + (mOriginalBarBox.Width - textSizeX) / 2;
        }

        protected override float GetTextScale(Vector2 textSize)
        {
            var scale = 1f;
            if (textSize.X > mOriginalBarBox.Width)
            {
                scale = mOriginalBarBox.Width / textSize.X - 0.1f;
            }

            return scale;
        }

        protected void SetMultiplier(double multiplier)
        {
            mBox.Width = (int)(mOriginalBarBox.Width  * multiplier);
        }

        protected override void DrawBackground(SpriteBatch spriteBatch, float transparency)
        {
            spriteBatch.Draw(mBackground, mBox, mBoxMainColor);
        }
    }
}
