using System;
using System.Collections.Generic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.Screen.Hud;
using KingsOfSacredStar.Screen.MainMenu;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.InGameMenu
{
    internal sealed class InGameMenu : IScreen
    {

        private readonly Camera mCamera;
        private readonly GameHudText mInGameMenu;
        private readonly GameHudButton mLoadButton;
        private readonly GameHudButton mSaveButton;
        private readonly GameHudButton mGraphicsButton;
        private readonly GameHudButton mSoundButton;
        private readonly GameHudButton mBackToMainMenuButton;
        private readonly GameHudButton mBackToGameButton;



        public InGameMenu(ContentManager content, Camera camera)
        {
            mInGameMenu = new GameHudText(content, 600, 200, 300, 500, new[] {""});
            mLoadButton = new GameHudButton(content, 620, 250, 260, 50, new[] {Properties.InGameMenu.LoadGame});
            mSaveButton = new GameHudButton(content, 620, 320, 260, 50, new[] {Properties.InGameMenu.SaveGame});
            mGraphicsButton = new GameHudButton(content, 620, 390, 260, 50, new[] {Properties.InGameMenu.GraphicSettings});
            mSoundButton = new GameHudButton(content, 620, 460, 260, 50, new[] {Properties.InGameMenu.SoundSettings});
            mBackToMainMenuButton = new GameHudButton(content, 620, 530, 260, 50, new[] {Properties.InGameMenu.BackToMainMenu});
            mBackToGameButton = new GameHudButton(content, 620, 600, 260, 50, new[] {Properties.InGameMenu.BackToGame});
            mCamera = camera;
        }
        

        public void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            GameStateWrapper.SetPause(true);
            removeScreens(new List<Screen> { Screen.HeroMenuPanel });
            if (CheckEsc(removeScreens))
                return;

            if (mLoadButton.GetClicked())
            {
                addScreens(new List<Screen> { Screen.LoadScreen });
                removeScreens(new List<Screen> { Screen.InGameMenu });
            }
            else if (mSaveButton.GetClicked()) {
                addScreens(new List<Screen> { Screen.SaveScreen });
                removeScreens(new List<Screen> { Screen.InGameMenu });
            }else if (mGraphicsButton.GetClicked())
            {
                removeScreens(new List<Screen> { Screen.InGameMenu });
                addScreens(new List<Screen> {Screen.InGameVideoMenu});
            }
            else if (mSoundButton.GetClicked())
            {
                removeScreens(new List<Screen> { Screen.InGameMenu });
                addScreens(new List<Screen> { Screen.InGameAudioMenu});
            }
            else if (mBackToMainMenuButton.GetClicked())
            {
                mCamera.Position = AMainMenuPage.sCameraPosition;
                addScreens(new List<Screen> { Screen.MainMenu });
                removeScreens(new List<Screen> {Screen.InGameMenu, Screen.GameHud, Screen.GameScreen});
            }
            else if (mBackToGameButton.GetClicked())
            {
                GameStateWrapper.SetPause(false);
                removeScreens(new List<Screen> {Screen.InGameMenu});
            }
        }

        private static bool CheckEsc(Action<List<Screen>> removeScreens)
        {
            if (ExitWrapper.EscClicked)
            {
                GameStateWrapper.SetPause(false);
                removeScreens(new List<Screen> { Screen.InGameMenu });
            }
            return false;
        }

        public void DrawHud(SpriteBatch spriteBatch)
        {
            mInGameMenu.Draw(spriteBatch, 1f);
            mLoadButton.Draw(spriteBatch, 1f);
            mSaveButton.Draw(spriteBatch, 1f);
            mGraphicsButton.Draw(spriteBatch, 1f);
            mSoundButton.Draw(spriteBatch, 1f);
            mBackToMainMenuButton.Draw(spriteBatch, 1f);
            mBackToGameButton.Draw(spriteBatch, 1f);
        }

        public void Draw(GameTime gameTime)
        {
            // No 3D Stuff to draw
        }

        public bool ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            if (mLoadButton.InBox(inputLastMouseClickPosition)) return true;
            if (mSaveButton.InBox(inputLastMouseClickPosition)) return true;
            if (mGraphicsButton.InBox(inputLastMouseClickPosition)) return true;
            if (mSoundButton.InBox(inputLastMouseClickPosition)) return true;
            if (mBackToMainMenuButton.InBox(inputLastMouseClickPosition)) return true;
            mBackToGameButton.InBox(inputLastMouseClickPosition);
            return true;
        }

        public bool ProcessMouseRightClick(Point inputLastMouseClickPosition) => false;
    }
}