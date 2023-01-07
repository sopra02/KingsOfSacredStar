using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.Screen.DebugClasses;
using KingsOfSacredStar.Screen.Hud;
using KingsOfSacredStar.Screen.Hud.HeroPanel;
using KingsOfSacredStar.Screen.InGameMenu;
using KingsOfSacredStar.Screen.MainMenu;
using KingsOfSacredStar.Screen.SkillMenu;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.Statistic;
using KingsOfSacredStar.Statistic.Achievement;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    /// <summary>
    /// This class contains every screen and calls the one set as active.
    /// </summary>
    internal sealed class ScreenManager
    {
        private struct ScreenInformation
        {
            public IScreen mScreenInstance;
            public Screen mScreenName;
        }
        private readonly List<ScreenInformation> mActiveScreensStack = new List<ScreenInformation>();
        private readonly Dictionary<Screen, Func<IScreen>> mScreens = new Dictionary<Screen, Func<IScreen>>();

        /// <param name="content">Is needed to load content</param>
        /// <param name="graphics">Used for 3D Rendering</param>
        /// <param name="camera">Used for 3D Rendering</param>
        /// <param name="inputManager">Used to map keys</param>
        /// <param name="musicManager"></param>
        [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
        public ScreenManager(ContentManager content,
            GraphicsDeviceManager graphics,
            Camera camera,
            InputManager inputManager,
            MusicManager musicManager)
        {
            // ImplicitlyCapturedClosure is ignored because the map is not modified
            // for the lifetime of the application. Therefore all references
            // need to stay in memory for the lifetime of this instance
            // anyways. So syntactic sugar is more important here
            // than creating a factory class, essentially doing
            // everything twice just for this issue to go away.
            mScreens.Add(Screen.MainMenu, () => new MainMenu.MainMenu(content, graphics, camera));
            mScreens.Add(Screen.SinglePlayerMenu, () => new SinglePlayerMenu(content, graphics, camera));
            mScreens.Add(Screen.LoadGameMenu, () => new LoadGameMenu(content, graphics, camera));
            mScreens.Add(Screen.OptionsMenu, () => new OptionsMenu(content, graphics, camera));
            mScreens.Add(Screen.CreditsMenu, () => new CreditsMenu(content, graphics, camera));
            mScreens.Add(Screen.HelpMenu, () => new HelpMenu(content, graphics, camera));
            mScreens.Add(Screen.AudioMenu, () => new AudioMenu(content, graphics, camera));
            mScreens.Add(Screen.VideoMenu, () => new VideoMenu(content, graphics, camera));
            mScreens.Add(Screen.AchievementsMenu, () => new AchievementsMenu(content, graphics, camera));
            mScreens.Add(Screen.AchievementManager, () => new AchievementManager(content, graphics, camera));
            mScreens.Add(Screen.Statistics, () => new Statistics(content, graphics, camera));
            mScreens.Add(Screen.GameHud, () => new GameHud(content, camera));
            mScreens.Add(Screen.BuildMenuPanel, () => new BuildMenuPanel(content));
            mScreens.Add(Screen.RecruitMenuPanel, () => new RecruitMenuPanel(content));
            mScreens.Add(Screen.HeroMenuPanel, () => new HeroMenuPanel(content));
            mScreens.Add(Screen.SkillingMenu, () => new SkillingMenu(content));
            mScreens.Add(Screen.InGameMenu, () => new InGameMenu.InGameMenu(content, camera));
            mScreens.Add(Screen.SaveScreen, () => new LoadSaveScreen(content, false));
            mScreens.Add(Screen.LoadScreen, () => new LoadSaveScreen(content, true));
            mScreens.Add(Screen.InGameAudioMenu, () => new InGameAudioMenu(content));
            mScreens.Add(Screen.InGameVideoMenu, () => new InGameVideoMenu(content, graphics));
            mScreens.Add(Screen.GameScreen, () => new GameScreen(content, graphics, camera, inputManager));
            mScreens.Add(Screen.DebugScreen, () => new DebugScreen(content, inputManager, camera, graphics));
            mScreens.Add(Screen.EndGame, () => new EndGame(content, graphics, camera, musicManager));
            mScreens.Add(Screen.InGameStatistics, () => new InGameStatistics(content, graphics, camera));
            mScreens.Add(Screen.MouseOverlayScreen, () => new MouseOverlayScreen(content, inputManager));

#if DEBUG
            mActiveScreensStack.Add(new ScreenInformation{mScreenInstance = mScreens[Screen.DebugScreen](), mScreenName = Screen.DebugScreen});
#endif
            mActiveScreensStack.Add(new ScreenInformation { mScreenInstance = mScreens[Screen.MouseOverlayScreen](), mScreenName = Screen.MouseOverlayScreen });
            mActiveScreensStack.Add(new ScreenInformation { mScreenInstance = mScreens[Screen.MainMenu](), mScreenName = Screen.MainMenu });
        }

        /// <summary>
        /// This is also used to replace the screen.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            for (var i = 0; i < mActiveScreensStack.Count; i++)
            {
                var indexInsert = i;
                mActiveScreensStack[i].mScreenInstance.Update(gameTime,
      addScreens =>
                    {
                        for (var j = addScreens.Count - 1; j >= 0; j--)
                        {
                            var screen = mScreens[addScreens[j]]();
                            var j1 = j;
                            if (mActiveScreensStack.Any(screenInformation =>
                                screenInformation.mScreenName == addScreens[j1]))
                            {
                                Console.WriteLine(@"WARNING: Screen {0} is already on the stack: [{1}]",
                                    screen.GetType().Name,
                                    string.Join(", ", mActiveScreensStack.Select(s => s.GetType().Name)));
                            }
                            else
                            {
                                mActiveScreensStack.Insert(indexInsert,
                                    new ScreenInformation {mScreenInstance = screen, mScreenName = addScreens[j]});
                            }
                        }
                    },
    removeScreens =>
                {
                    mActiveScreensStack.RemoveAll(screenInformation => removeScreens.Contains(screenInformation.mScreenName));
                });
            }
        }

        public void ProcessMouseLeftClick(Point mouseClickPosition)
        {
            foreach (var screen in mActiveScreensStack)
            {
                if (screen.mScreenInstance.ProcessMouseLeftClick(mouseClickPosition))
                {
                    break;
                }
            }
        }


        public void ProcessMouseRightClick(Point mouseClickPosition)
        {
            foreach (var screen in mActiveScreensStack)
            {
                if (screen.mScreenInstance.ProcessMouseRightClick(mouseClickPosition))
                {
                    break;
                }
            }
        }

        public void DrawHud(SpriteBatch spriteBatch)
        {
            for (var i = mActiveScreensStack.Count - 1; i >= 0; i--)
            {
                mActiveScreensStack[i].mScreenInstance.DrawHud(spriteBatch);
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (var i = mActiveScreensStack.Count - 1; i >= 0; i--)
            {
#if DEBUG
                if (mActiveScreensStack[i].mScreenName == Screen.DebugScreen && mActiveScreensStack.All(screen => screen.mScreenName != Screen.GameScreen))
                {
                    continue;
                }
#endif
                mActiveScreensStack[i].mScreenInstance.Draw(gameTime);
            }
        }
    }
}
