using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Scenes;
using Pladi.Utilities.DataStructures;

namespace Pladi.Core.Entities
{
    public abstract class KeyboardButtonEntity : Entity
    {
        // [...]

        public class LeftKeyboardButtonEntity : KeyboardButtonEntity
        {
            public LeftKeyboardButtonEntity(LevelScene scene) : base(scene, 0, 1, 1, PlayerEntity.InputFlags.Left) { }
        }

        public class SpaceKeyboardButtonEntity : KeyboardButtonEntity
        {
            public SpaceKeyboardButtonEntity(LevelScene scene) : base(scene, 16, 3, 1, PlayerEntity.InputFlags.Jump) { }
        }

        public class RightKeyboardButtonEntity : KeyboardButtonEntity
        {
            public RightKeyboardButtonEntity(LevelScene scene) : base(scene, 64, 1, 1, PlayerEntity.InputFlags.Right) { }
        }

        // [public properties and fields]

        public override EdgeF[] ShadowCastEdges
        {
            get
            {
                var hitbox = Hitbox;
                hitbox.X -= 0.5f;
                hitbox.Width += 1f;

                return RectangleF.GetEdges(hitbox);
            }
        }

        public PlayerEntity.InputFlags InputType { get; init; }

        // [private properties and fields]

        private readonly int textureOffsetX;
        private readonly int buttonWidth;
        private readonly int buttonHeight;

        // [constructors]

        public KeyboardButtonEntity(LevelScene scene, int textureOffsetX, int buttonWidth, int buttonHeight, PlayerEntity.InputFlags inputType) : base(scene)
        {
            this.textureOffsetX = textureOffsetX;
            this.buttonWidth = buttonWidth;
            this.buttonHeight = buttonHeight;

            InputType = inputType;
            Hitbox = new RectangleF(0, 0, buttonWidth * 48, buttonHeight * 48);

            SetMassAsImmovable();
        }

        // [public methods]

        public void DrawThis(SpriteBatch spriteBatch, Vector2? position = null, Color? color = null)
        {
            var texture = TextureAssets.KeyboardButtons;

            spriteBatch.Draw(texture, position ?? Hitbox.Location, new Rectangle(textureOffsetX, 0, buttonWidth * 16, buttonHeight * 16), color ?? Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
        }

        // [protected methods]

        protected override bool PreDraw(SpriteBatch spriteBatch)
        {
            DrawThis(spriteBatch);

            return false;
        }
    }
}