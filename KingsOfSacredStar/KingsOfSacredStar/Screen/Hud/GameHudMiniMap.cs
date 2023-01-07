using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud
{
    internal sealed class GameHudMiniMap : AButton
    {
        private const int UnitSize = 4;
        public GameHudMiniMap(ContentManager content, int x, int y, int width, int height) : base(
            content,
            x,
            y,
            width,
            height,
            new string[0]) {}

        public void ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            var relativeClickPoint = inputLastMouseClickPosition - mBox.Location - (mBox.Size.ToVector2() / 2).ToPoint();
            var cameraPosition = ToolBox.Current.Camera.Position;
            var newPos = MiniMapToMap(relativeClickPoint);
            ToolBox.Current.Camera.Position = new Vector3(newPos.X, cameraPosition.Y, newPos.Y);
        }

        private Matrix MiniMapMatrix()
        {
            return 
                Matrix.CreateScale(1f / GameState.Current.MapSize.X, 1f / GameState.Current.MapSize.Y, 0) *
                Matrix.CreateScale(mBox.Width / 2f, mBox.Height / 2f, 0) *
                Matrix.CreateTranslation(mBox.X + mBox.Width / 2, mBox.Y + mBox.Height / 2, 0) *
                Matrix.CreateTranslation(UnitSize / -2f, UnitSize / -2f, 0);
        }

        private Point UnitMapSize()
        {
            return new Point(UnitSize * (GameState.Current.MapSize.X / (mBox.Width / 2)),
                UnitSize * (GameState.Current.MapSize.Y / (mBox.Height / 2)));
        }

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            spriteBatch.Draw(mBackground, mBox, Color.Black);
            spriteBatch.End();
            spriteBatch.Begin(transformMatrix: MiniMapMatrix());
            var unitSize = UnitMapSize();
            foreach (var units in GameState.Current.UnitsByPlayer)
            {
                foreach (var unit in units.Value)
                {
                    DrawUnit(spriteBatch, unit, unitSize);
                }
            }
            foreach (var buildings in GameState.Current.BuildingsByPlayer)
            {
                foreach (var building in buildings.Value)
                {
                    DrawUnit(spriteBatch, building, unitSize);
                }
            }
            DrawCamera(spriteBatch, unitSize);
            spriteBatch.End();
            spriteBatch.Begin();
        }

        private void DrawUnit(SpriteBatch spriteBatch, IUnit unit, Point unitSize)
        {
            spriteBatch.Draw(mBackground, new Rectangle(unit.Position.ToPoint(), unitSize), PlayerConstants.sPlayerColors[(int)unit.Owner]);
        }

        private void DrawCamera(SpriteBatch spriteBatch, Point unitSize)
        {
            var points = new[]
            {
                ToolBox.Current.ConvertScreenToGameFieldPosition(new Point(0, 0)),
                ToolBox.Current.ConvertScreenToGameFieldPosition(Globals.Resolution),
                ToolBox.Current.ConvertScreenToGameFieldPosition(new Point(0, Globals.Resolution.Y)),
                ToolBox.Current.ConvertScreenToGameFieldPosition(new Point(Globals.Resolution.X, 0))
            };
            foreach (var point in points)
            {
                spriteBatch.Draw(mBackground, new Rectangle(point, unitSize), Color.White);
            }
        }

        private Vector2 MiniMapToMap(Point position)
        {
            var mapSize = GameState.Current.MapSize;
            
            return new Vector2(position.X * mapSize.X * 2f / mBox.Width, position.Y * mapSize.Y * 2f / mBox.Height);
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            // do nothing
        }
    }
}
