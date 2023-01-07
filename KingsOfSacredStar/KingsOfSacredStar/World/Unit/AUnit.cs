using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit
{
    internal abstract class AUnit : IUnit
    {
        public virtual ModelManager.Model ModelType { get; }
        protected int GridSize { get; }
        public UnitTypes UnitType { get; }
        public Vector2 Position { get; protected set; }
        protected float Rotation { get; set; }
        public Players Owner { get; protected set; }
        private readonly Matrix mModelOffset;
        private readonly Matrix mReverseOffset;

        protected AUnit(ModelManager.Model model, UnitTypes unitType, Players owner, Vector2 pos, int gridSize, float rotation)
        {
            ModelType = model;
            GridSize = gridSize;
            UnitType = unitType;
            Position = pos * gridSize;
            Rotation = rotation;
            Owner = owner;
            mModelOffset = Matrix.CreateTranslation(gridSize / 2f, 0, gridSize / 2f);
            mReverseOffset = Matrix.CreateTranslation(gridSize / -2f, 0, gridSize / -2f);
        }

        public abstract void Update(GameTime gameTime);

        public virtual void IsPaused(GameTime gameTime)
        {
            // No pause action
        }

        public virtual string Serialize()
        {
            return (int)Owner + " " + (int)UnitType + " " + Position.X / GridSize + " " + Position.Y / GridSize + " " + Rotation;
        }

        public virtual Matrix RenderPosition =>
            mModelOffset *
            Matrix.CreateRotationY(Rotation) *
            mReverseOffset *
            Matrix.CreateTranslation(Position.X, 0, Position.Y);
    }
}
