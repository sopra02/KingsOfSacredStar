using System;
using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    /// <summary>
    /// The actual game will run here.
    /// </summary>
    internal sealed class GameScreen : IScreen
    {
        public const int GridSize = 16;

        private readonly Camera mCamera;

        private readonly InputManager mInput;

        private readonly Texture2D mWhitePixel;
        private readonly Effect mHeroEffect;
        private readonly StatusBarRenderer mStatusBarRenderer;
        private readonly PlaneRenderer mGroundPlaneRenderer;
        private readonly PlaneRenderer mHeroEffectPlaneRenderer;
        private readonly SpriteFont mFontMenu;
        private readonly SpriteFont mFontSmall;
        private readonly GraphicsDeviceManager mGraphics;

        public static BuildManager BuildManager { get; private set; }

        public GameScreen(ContentManager content, GraphicsDeviceManager graphics, Camera camera, InputManager input)
        {
            GameState.Current = new GameState(camera, GridSize, content);
            mGroundPlaneRenderer = new PlaneRenderer(graphics, camera, new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                Texture = content.Load<Texture2D>("textures/ground_sand")
            });
            mHeroEffect = content.Load<Effect>("effects/HeroEffect").Clone();
            mHeroEffectPlaneRenderer = new PlaneRenderer(graphics, camera, mHeroEffect);
            LoadAndSaveManager.Current = new LoadAndSaveManager(GridSize, camera, this, content);
            mWhitePixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            mWhitePixel.SetData(new[] { Color.White });
            mGraphics = graphics;
            mCamera = camera;
            mInput = input;
            BuildManager = new BuildManager(input);
            BuildManagerWrapper.Init(BuildManager);
            BuildManagerWrapper.LoadInputManager(input);
            GameStateWrapper.LoadInputManager(input);
            input.SetSelectAction(SelectUnits);
            mFontMenu = content.Load<SpriteFont>("fonts/Menu");
            mFontSmall = content.Load<SpriteFont>("fonts/Small");

            LoadAndSaveManager.Current.LoadMap("map01", true);
            mStatusBarRenderer = new StatusBarRenderer(graphics.GraphicsDevice, content, camera);
            camera.Position = new Vector3(GridSize / -2f, GridSize * GridSize, 4 * GridSize);
            GameStateWrapper.StartGame();
        }

        private static void SelectUnits(Rectangle selection)
        {
            var topLeft = ToolBox.Current.ConvertScreenToGameFieldPosition(new Point(selection.X, selection.Y)).ToVector2();
            var topRight = ToolBox.Current.ConvertScreenToGameFieldPosition(new Point(selection.X + selection.Width, selection.Y)).ToVector2();
            var bottomLeft = ToolBox.Current.ConvertScreenToGameFieldPosition(new Point(selection.X, selection.Y + selection.Height)).ToVector2();
            var bottomRight = ToolBox.Current.ConvertScreenToGameFieldPosition(new Point(selection.X + selection.Width, selection.Y + selection.Height)).ToVector2();

            GameState.Current.SelectEntities(new Trapeze(topLeft, topRight, bottomLeft, bottomRight), Players.Player);
        }

        internal void SetMapSize(int sizeX, int sizeZ)
        {
            mGroundPlaneRenderer.InitializePlane(sizeX, 0, sizeZ, (float)sizeX / GridSize, (float)sizeZ / GridSize);
            mHeroEffectPlaneRenderer.InitializePlane(1, 0.05f, 1);
            mCamera.SetClippingSize();
        }

        public void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (GameState.Current.Update(gameTime))
            {
                addScreens(new List<Screen> { Screen.EndGame });
                removeScreens(new List<Screen> { Screen.GameHud, Screen.GameScreen, Screen.RecruitMenuPanel, Screen.BuildMenuPanel, Screen.HeroMenuPanel });
            }

        }

        private static ModelManager.RenderingEffect GetEffectForUnit(IUnit unit)
        {
            if (unit is IDamageableUnit damageableUnit && damageableUnit.IsHit)
            {
                return ModelManager.RenderingEffect.Hit;
            }
            return GameState.Current.IsSelected(unit)
                ? ModelManager.RenderingEffect.Selected
                : ModelManager.RenderingEffect.None;
        }

        public void Draw(GameTime gameTime)
        {
            mGroundPlaneRenderer.Draw();

            foreach (var entry in GameState.Current.UnitsByModel)
            {
                entry.Key.Draw(gameTime, entry.Value
                    .Select(unit => new ModelManager.RenderingInformation(unit.RenderPosition, PlayerConstants.sPlayerColors[(int) unit.Owner], GetEffectForUnit(unit)))
                    .ToArray());
                mStatusBarRenderer.Draw(Color.Red, unit => (float)unit.Health / unit.MaxHealth, entry.Value.OfType<IDamageableUnit>().ToArray());
            }

            foreach (var buildings in GameState.Current.BuildingsByPlayer)
            {
                var unitsToDraw = buildings.Value.OfType<ICapturable>().Where(unit => unit.NeedsDrawing).ToArray();
                mStatusBarRenderer.Draw(PlayerConstants.sPlayerColors[(int)Players.Player], unit => unit.FriendlyPercentage, unitsToDraw);
                mStatusBarRenderer.DrawReversed(PlayerConstants.sPlayerColors[(int)Players.Ai], unit => unit.EnemyPercentage, unitsToDraw);
            }

            var heroDrawOffset = new Vector2(GridSize / 2f, GridSize / 2f);
            foreach (var hero in GameState.Current.HeroesByPlayer.Values)
            {
                mHeroEffect.Parameters["Color"].SetValue(hero.ActiveEffectColor.ToVector4());
                mHeroEffectPlaneRenderer.Draw(hero.Position - heroDrawOffset, hero.ActiveEffectRange);
            }

            BuildManager.Draw(gameTime);
        }

        public bool ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            if (BuildManager.IsActive)
            {
                BuildManager.ProcessMouseLeftClick(inputLastMouseClickPosition);
                return true;
            }
            var targetEntityGameFieldPosition = ToolBox.Current.ConvertScreenToGameFieldPosition(inputLastMouseClickPosition);
            GameState.Current.SelectEntity(targetEntityGameFieldPosition, Players.Player);
            return true;
        }

        public bool ProcessMouseRightClick(Point inputLastMouseClickPosition)
        {
            if (BuildManager.IsActive)
            {
                BuildManager.ProcessMouseRightClick();
                return true;
            }
            var entityTarget = ToolBox.Current.ConvertScreenToGameFieldPosition(inputLastMouseClickPosition);
            if (!GameState.Current.AttackUnitsAtPosition(entityTarget))
            {
                GameState.Current.MoveSelectedUnits(entityTarget.X, entityTarget.Y);
            }
            return true;
        }

        public void DrawHud(SpriteBatch spriteBatch)
        {
            var selectionBox = mInput.GetMouseRectangle();
            if (!selectionBox.IsEmpty)
            {
                const int borderWidth = 1;
                // top line
                spriteBatch.Draw(mWhitePixel,
                    new Rectangle(selectionBox.Left, selectionBox.Top, selectionBox.Width, borderWidth),
                    Color.White);
                // bottom line
                spriteBatch.Draw(mWhitePixel,
                    new Rectangle(selectionBox.Left,
                        selectionBox.Bottom - borderWidth,
                        selectionBox.Width,
                        borderWidth),
                    Color.White);
                // left line
                spriteBatch.Draw(mWhitePixel,
                    new Rectangle(selectionBox.Left, selectionBox.Top, borderWidth, selectionBox.Height),
                    Color.White);
                // right line
                spriteBatch.Draw(mWhitePixel,
                    new Rectangle(selectionBox.Right - borderWidth, selectionBox.Top, borderWidth, selectionBox.Height),
                    Color.White);
            }

            if (GameState.Current.IsPaused)
            {
                DrawPauseMessage(spriteBatch);
            }
        }

        private void DrawPauseMessage(SpriteBatch spriteBatch)
        {
            var screenHeight = mGraphics.PreferredBackBufferHeight;
            var screenWidth = mGraphics.PreferredBackBufferWidth;

            var messagesWithFont = new List<(string, SpriteFont)>
            {
                ("Paused", mFontMenu),
                ("Press 'p' to continue", mFontSmall)
            };

            for (var i = 0; i < messagesWithFont.Count; i++)
            {
                var message = messagesWithFont[i];
                var lineSpacing = 30;
                var stringSize = message.Item2.MeasureString(message.Item1);
                var stringPosX = (screenWidth - stringSize.X) / 2;
                var stringPosY = screenHeight / 2f - (messagesWithFont.Count - 2*i) * lineSpacing;
                spriteBatch.DrawString(message.Item2, message.Item1, new Vector2(stringPosX, stringPosY), Color.Yellow);
            }
        }
    }
}