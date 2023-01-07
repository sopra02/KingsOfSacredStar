using System;
using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using KingsOfSacredStar.World.Unit.Buildings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud
{
     internal sealed class GameHud : AMenuPage
     {
         private GameHudText mResourcesGold;
         private GameHudText mResourcesStone;
         private GameHudText mUnits;
         private GameHudText mInGameTime;
         private GameHudMiniMap mMiniMap;

         private GameHudText mSelectionMenu;
         private GameHudButton mBuildMenuButton;
         private GameHudButton mRecruitMenuButton;
         private GameHudButton mHeroMenuButton;
         private GameHudButton mInGameMenuButton;

         private GameHudText mMenuPanel;
         private Screen? mActiveScreen = Screen.RecruitMenuPanel;

         private readonly GameHudPlaceHolder mMenuBox;
         private readonly GameHudPlaceHolder mPanelBox;

         private int mUnitsCounter;
         private int mGoldCounter;
         private int mGoldIncrease;
         private int mStoneCounter;
         private int mStoneIncrease;
         private int mSeconds;
         private int mMinutes;
         private int mHours;

         private readonly ContentManager mContent;
         private readonly Camera mCamera;

         private bool mStillSelected;


         public GameHud(ContentManager content, Camera camera) : base(new AButton[0])
         {
             mMenuBox = new GameHudPlaceHolder(content, 0, 675, GetWidth(), 225);
             mPanelBox = new GameHudPlaceHolder(content, 0, 0, GetWidth(), 40);
             ResourcesPanel(content);
             MiniMap(content);
             SelectionMenu(content);
             MenuPanel(content);
             mContent = content;
             mCamera = camera;
         }

         public override void DrawHud(SpriteBatch spriteBatch)
         {
            base.DrawHud(spriteBatch);
            mMenuBox.Draw(spriteBatch, 1f);
            mPanelBox.Draw(spriteBatch, 1f);
            mResourcesGold.Draw(spriteBatch, 1f);
            mResourcesStone.Draw(spriteBatch, 1f);
            mUnits.Draw(spriteBatch, 1f);
            mInGameTime.Draw(spriteBatch, 1f);
            mMiniMap.Draw(spriteBatch, 1f);
            mSelectionMenu.Draw(spriteBatch, 1f);
            mMenuPanel.Draw(spriteBatch, 1f);
            mBuildMenuButton.Draw(spriteBatch, 1f);
            mRecruitMenuButton.Draw(spriteBatch, 1f);
            mHeroMenuButton.Draw(spriteBatch, 1f);
            mInGameMenuButton.Draw(spriteBatch, 1f);

         }

         public override bool ProcessMouseLeftClick(Point inputLastMouseClickPosition)
         {
             if (base.ProcessMouseLeftClick(inputLastMouseClickPosition))
                return true;
             if (mMiniMap.InBox(inputLastMouseClickPosition))
             {
                 mMiniMap.ProcessMouseLeftClick(inputLastMouseClickPosition);
                 return true;
             }

             return new List<AButton>
             {
                 mInGameMenuButton,
                 mBuildMenuButton,
                 mRecruitMenuButton,
                 mHeroMenuButton,
                 mPanelBox,
                 mMenuBox
             }.Any(b => b.InBox(inputLastMouseClickPosition));
         }

        public override bool ProcessMouseRightClick(Point inputLastMouseClickPosition)
        {
            return false;
        }

        private bool HandleButtons(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (mInGameMenuButton.GetClicked())
            {
                GameStateWrapper.SetPause(true);
                SetNewPanel(addScreens, removeScreens, Screen.InGameMenu);
                removeScreens(new List<Screen> { Screen.HeroMenuPanel });  // dirty fix
                mActiveScreen = null;
            }
            else if (mBuildMenuButton.GetClicked())
            {
                SetNewPanel(addScreens, removeScreens, Screen.BuildMenuPanel);
            }
            else if (mRecruitMenuButton.GetClicked())
            {
                SetNewPanel(addScreens, removeScreens, Screen.RecruitMenuPanel);
            }
            else if (mHeroMenuButton.GetClicked())
            {
                SetNewPanel(addScreens, removeScreens, Screen.HeroMenuPanel);
                var heroUnit = GameState.Current.UnitsByPlayer[Players.Player].FirstOrDefault(u => u.UnitType == UnitTypes.Hero);

                if (heroUnit != null)
                {
                    GameState.Current.SelectEntities(GameState.Current.MapPoints, Players.Player, UnitTypes.Hero);
                    var cameraPosition = mCamera.Position;
                    cameraPosition.X = heroUnit.Position.X;
                    cameraPosition.Z = heroUnit.Position.Y + 100;
                    mCamera.Position = cameraPosition;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void SetNewPanel(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens, Screen newPanel)
        {
            if (newPanel != GetActivePanel())
            {
                addScreens(new List<Screen> { newPanel });
                var activePanel = GetActivePanel();
                if (activePanel != null)
                {
                    removeScreens(new List<Screen> { activePanel.Value });
                }
            }
            SetActivePanel(newPanel);
        }

        private void HandleResources()
        {
            if (GameState.Current.Resources.Count == 0)
            {
                mGoldCounter = 0;
                mStoneCounter = 0;
                mGoldIncrease = 0;
                mStoneIncrease = 0;
            }
            else
            {
                var resources = GameState.Current.Resources[Players.Player];
                mGoldCounter = resources[Resources.Gold];
                mStoneCounter = resources[Resources.Stone];
                mGoldIncrease = 0;
                mStoneIncrease = 0;
                foreach (var unit in GameState.Current.BuildingsByPlayer[Players.Player])
                {
                    if (unit.UnitType == UnitTypes.GoldMine)
                    {
                        var mine = (Mine) unit;
                        mGoldIncrease += mine.Level;
                    }
                    if (unit.UnitType == UnitTypes.Quarry)
                    {
                        var mine = (Mine)unit;
                        mStoneIncrease += mine.Level;
                    }
                }

                mGoldIncrease++;
                mStoneIncrease++;

            }

            mUnitsCounter = 0;
            foreach (var unit in GameState.Current.UnitsByPlayer[Players.Player])
            {
                if (unit.UnitType != UnitTypes.Arrow)
                    mUnitsCounter++;
            }

            //mUnitsCounter = GameState.Current.UnitsByPlayer.Count == 0 ? 0 : GameState.Current.UnitsByPlayer[Players.Player].Count;

            ResourcesPanel(mContent);
        }

        private void HandleHeroPanel(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {

            var heroUnit = GameState.Current.UnitsByPlayer[Players.Player].FirstOrDefault(u => u.UnitType == UnitTypes.Hero);

            if (heroUnit != null && GameState.Current.IsSelected(heroUnit))
            {
                if (GetActivePanel() != Screen.HeroMenuPanel &&!mStillSelected)
                {
                    mStillSelected = !mStillSelected;
                    
                    addScreens(new List<Screen> {Screen.HeroMenuPanel});
                    var activePanel = GetActivePanel();
                    if (activePanel != null)
                    {
                        removeScreens(new List<Screen> {activePanel.Value});
                    }
                    SetActivePanel(Screen.HeroMenuPanel);
                }
            }
            else
            {
                mStillSelected = false;
            }
        }

        private void HandleTime()
        {
            int totalSecondsPlayed = GameState.Current.StatisticsByPlayer[Players.Player]["LastGamePlaytime"];
            mSeconds = totalSecondsPlayed % 60;
            mMinutes = (totalSecondsPlayed / 60) % 60;
            mHours = (totalSecondsPlayed / 3600);
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (CheckEsc(addScreens, removeScreens))
            {
                return;
            }
            if (HandleButtons(addScreens, removeScreens))
            {
                return;
            }

            HandleResources();

            HandleTime();

            HandleHeroPanel(addScreens, removeScreens);

        }

         private static int GetWidth()
         {
             return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
         }

         private void ResourcesPanel(ContentManager content)
         {
             mResourcesGold = new GameHudText(content, 5, 4, 200, 32, new[] {Properties.GameHud.Gold + mGoldCounter + Properties.GameHud.Increase + mGoldIncrease + ")"});
             mResourcesStone = new GameHudText(content, 210, 4, 200, 32, new[] {Properties.GameHud.Stone + mStoneCounter + Properties.GameHud.Increase + mStoneIncrease + ")"});
             mUnits = new GameHudText(content, 415, 4, 150, 32, new[] {Properties.GameHud.Units + mUnitsCounter});
             mInGameTime = new GameHudText(content, 1495, 4, 100, 32, new [] {mHours + " : " + mMinutes + " : " + mSeconds});
         }

         private void MiniMap(ContentManager content)
         {
             mMiniMap = new GameHudMiniMap(content, 1380, 680, 215, 215);
         }

         private void SelectionMenu(ContentManager content)
         {
             mSelectionMenu = new GameHudText(content, 1200, 680, 170, 215, new string[0]);
             mBuildMenuButton = new GameHudButton(content, 1205, 686, 160, 47, new[] {Properties.GameHud.Build});
             mRecruitMenuButton = new GameHudButton(content, 1205, 738, 160, 47, new[] {Properties.GameHud.Recruit});
             mHeroMenuButton = new GameHudButton(content, 1205, 790, 160, 47, new[] {Properties.GameHud.Hero});
             mInGameMenuButton = new GameHudButton(content, 1205, 842, 160, 47, new[] {Properties.GameHud.Menu});
         }

         private void MenuPanel(ContentManager content)
         {
            mMenuPanel = new GameHudText(content, 5, 680, 1190, 215, new string[0]);

         }

         private Screen? GetActivePanel()
         {
             return mActiveScreen;
         }

         private void SetActivePanel(Screen screen)
         {
             mActiveScreen = screen;
             GameScreen.BuildManager.IsActive = false;
         }
        protected override bool CheckEsc(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (ExitWrapper.EscClicked)
            {
                GameStateWrapper.SetPause(true);
                SetNewPanel(addScreens, removeScreens, Screen.InGameMenu);
                removeScreens(new List<Screen> { Screen.HeroMenuPanel });  // dirty fix
                mActiveScreen = null;
                return true;
            }
            return false;
        }

    }
}