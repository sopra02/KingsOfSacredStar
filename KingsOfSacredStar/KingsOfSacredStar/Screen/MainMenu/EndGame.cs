using System;
using System.Collections.Generic;
using System.Diagnostics;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class EndGame : AMainMenuPage
    {
        private readonly MenuText mMessage;
        public EndGame(ContentManager content, GraphicsDeviceManager graphics, Camera camera, MusicManager musicManager) : base(content, graphics, camera, new AButton[]{
            new MenuButton(content, 300, 300, 400, 100, new[] {Properties.AchievementsMenu.Statistics}, new List<Screen> {Screen.InGameStatistics}, new List<Screen> {Screen.EndGame}),
            new ActionButton(content, 300, 500, 400, 100, new[] {Properties.InGameMenu.BackToMainMenu}, () => {musicManager.Play(0);}, followingScreens: new List<Screen> {Screen.MainMenu}, deletingScreens: new List<Screen> {Screen.EndGame})
        })
        {
            Debug.Assert(GameState.Current != null);
            GameState.Current.SaveStatistics();
            mMessage = new MenuText(content,600,100,400,100, new[] {GameState.Current.VillagesByPlayer[Players.Player].Health > 0 ? Properties.EndGame.Win : Properties.EndGame.Loss});
            musicManager.Play(GameState.Current.VillagesByPlayer[Players.Player].Health > 0 ? 1 : 2);
        }

        public override void DrawHud(SpriteBatch spriteBatch)
        {
            base.DrawHud(spriteBatch);
            mMessage.Draw(spriteBatch, 1f);
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            if (GameState.Current == null)
            {
                return;
            }

            if (GameState.Current.VillagesByPlayer[Players.Player].Health > 0)
            {
                mCamera.Position = sCameraPosition + new Vector3(30, 0, 0) * GameScreen.GridSize;
            }
            else
            {
                mCamera.Position = sCameraPosition + new Vector3(-30, 0, 0) * GameScreen.GridSize;
            }
        }
    }
}
