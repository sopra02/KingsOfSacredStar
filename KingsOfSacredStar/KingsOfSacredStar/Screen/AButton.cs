using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    internal abstract class AButton
    {
        private const int Height = 900;  // do not touch
        private const int Width = 1600;  // do not touch
        protected readonly Texture2D mBackground;
        private readonly SpriteFont mFont;
        protected Rectangle mBox;
        protected string[] mText;
        private bool mClicked;
        protected readonly Color mBoxMainColor;
        private readonly Color mBoxBorderColor;
        private const int PixelBorderWidth = 5;

        protected AButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            string[] text,
            Color boxColor = default,
            Color borderColor = default,
            SpriteFont font = null)
        {
            mFont = font ?? content.Load<SpriteFont>("fonts/Menu");
            mBackground = content.Load<Texture2D>("textures/button_mask");
            mBox = new Rectangle(x * Globals.Resolution.X / Width, y * Globals.Resolution.Y / Height, width * Globals.Resolution.X / Width, height * Globals.Resolution.Y / Height);
            mText = text;
            mClicked = false;
            mBoxMainColor = boxColor != default ? boxColor : Color.Brown;
            mBoxBorderColor = borderColor != default ? borderColor : Color.DarkGray;
        }


        protected virtual float GetTextPositionX(float textSizeX)
        {
            return mBox.X + (mBox.Width - textSizeX) / 2;
        }

        private float GetTextPositionY(float textSizeY, int lineIndex, int lineCount)
        {
            return mBox.Y + mBox.Height * (lineIndex + 1) / (lineCount + 1) - textSizeY / 2;
        }

        protected virtual float GetTextScale(Vector2 textSize)
        {
            var scale = 1f;
            if (textSize.X > mBox.Width)
            {
                scale = mBox.Width / textSize.X - 0.1f;
            }

            return scale;
        }

        public bool InBox(Point hPoint)
        {
            mClicked = mBox.Left <= hPoint.X && hPoint.X <= mBox.Right && mBox.Top <= hPoint.Y && hPoint.Y <= mBox.Bottom;
            return mClicked;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(mBox.X, mBox.Y);
        }

        public Vector2 GetSize()
        {
            return new Vector2(mBox.Height, mBox.Width);
        }

        public string GetTextFirstLine()
        {
            return mText.Length != 0 ? mText[0] : "";
        }

        public virtual void Draw(SpriteBatch spriteBatch, float transparency)
        {
            DrawBackground(spriteBatch, transparency);
            DrawText(spriteBatch, transparency);
        }

        protected void DrawText(SpriteBatch spriteBatch, float transparency)
        {
            for (var i = 0; i < mText.Length; i++)
            {
                var text = mText[i];
                var textSize = mFont.MeasureString(text);
                var textScale = GetTextScale(textSize);
                var textPositionX = GetTextPositionX(textSize.X * textScale);
                var textPositionY = GetTextPositionY(textSize.Y * textScale, i, mText.Length);
                var textPosition = new Vector2(textPositionX, textPositionY);

                spriteBatch.DrawString(mFont,
                    text,
                    textPosition,
                    Color.Black * transparency,
                    0,
                    new Vector2(0, 0),
                    textScale,
                    SpriteEffects.None,
                    0);
            }
        }

        protected virtual void DrawBackground(SpriteBatch spriteBatch, float transparency)
        {
            var mainColor = mBoxMainColor * transparency;
            var borderColor = mBoxBorderColor * transparency;

            spriteBatch.Draw(mBackground, mBox, borderColor);
            var innerBox = new Rectangle(mBox.Location + new Point(PixelBorderWidth, PixelBorderWidth),
                mBox.Size - new Point(2 * PixelBorderWidth, 2 * PixelBorderWidth));
            spriteBatch.Draw(mBackground, innerBox, mainColor);

            var smallCornerBoxUpperLeft = new Rectangle(mBox.Location + new Point(PixelBorderWidth, PixelBorderWidth), new Point(PixelBorderWidth));
            var smallCornerBoxUpperRight = new Rectangle(mBox.Location + new Point(mBox.Size.X, 0) + new Point(-2 * PixelBorderWidth, PixelBorderWidth), new Point(PixelBorderWidth));
            var smallCornerBoxBottomLeft = new Rectangle(mBox.Location + new Point(0, mBox.Size.Y) + new Point(PixelBorderWidth, -2 * PixelBorderWidth), new Point(PixelBorderWidth));
            var smallCornerBoxBottomRight = new Rectangle(mBox.Location + mBox.Size + new Point(-2 * PixelBorderWidth, -2 * PixelBorderWidth), new Point(PixelBorderWidth));
            spriteBatch.Draw(mBackground, smallCornerBoxUpperLeft, borderColor);
            spriteBatch.Draw(mBackground, smallCornerBoxUpperRight, borderColor);
            spriteBatch.Draw(mBackground, smallCornerBoxBottomRight, borderColor);
            spriteBatch.Draw(mBackground, smallCornerBoxBottomLeft, borderColor);
        }

        public abstract void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens);

        public bool GetClicked()
        {
            if (mClicked)
            {
                mClicked = false;
                return true;
            }

            return false;
        }
    }
}