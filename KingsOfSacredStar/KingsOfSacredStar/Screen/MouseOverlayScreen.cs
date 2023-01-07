using System;
using System.Collections.Generic;
using KingsOfSacredStar.InputWrapper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    internal sealed class MouseOverlayScreen : IScreen
    {
        private readonly InputManager mInput;
        private readonly Texture2D mMousePointerDefault;
        private readonly Texture2D mMousePointerDeleteBuilding;
        private readonly Texture2D mMousePointerToggleGate;
        private Rectangle mDrawBox;
        const int MouseSize = 48;

        public MouseOverlayScreen(ContentManager content, InputManager input)
        {
            mInput = input;
            mMousePointerDefault = content.Load<Texture2D>("textures/sword");
            mMousePointerDeleteBuilding = content.Load<Texture2D>("textures/deleteBuilding");
            mMousePointerToggleGate = content.Load<Texture2D>("textures/toggleGate");
            mDrawBox = new Rectangle(Point.Zero, Point.Zero);
        }

        public void Draw(GameTime gameTime)
        {
            // No 3D to draw
        }

        public void DrawHud(SpriteBatch spriteBatch)
        {
            var mousePointerTexture = GetMousePointerTexture();
            spriteBatch.Draw(mousePointerTexture, mDrawBox,
                new Rectangle(0, 0, mousePointerTexture.Width, mousePointerTexture.Height),
                Color.White);
        }

        private Texture2D GetMousePointerTexture()
        {
            var mousePointer = mMousePointerDefault;
            var buildManager = BuildManagerWrapper.BuildManager;
            if (buildManager != null)
            {
                if (buildManager.IsInGateToggleMode())
                {
                    mousePointer = mMousePointerToggleGate;
                }
                else if (buildManager.IsInDeleteMode())
                {
                    mousePointer = mMousePointerDeleteBuilding;
                }
            }

            return mousePointer;
        }

        public bool ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            return false;
        }

        public bool ProcessMouseRightClick(Point inputLastMouseClickPosition)
        {
            return false;
        }

        public void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            mDrawBox = new Rectangle(mInput.GetMousePosition(), new Point(MouseSize, MouseSize));
        }
    }
}
