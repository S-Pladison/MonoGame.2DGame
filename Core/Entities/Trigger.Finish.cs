using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Scenes;

namespace Pladi.Core.Entities
{
    public class FinishTrigger : Trigger
    {
        // [constructors]

        public FinishTrigger(LevelScene scene) : base(scene)
        {
            Width = 48;
            Height = 48 * 3;

            SetMassAsImmovable();

            OnTriggerEnter += (other) =>
            {
                if (other is not PlayerEntity player) return;

                player.Scene.Complite();
            };
        }

        // [protected methods]

        protected override bool PreDraw(SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.Flagpole;
            var origin = new Vector2(texture.Width * 0.5f, texture.Height);
            var rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            var position = Position + new Vector2(Width * 0.5f, Height);

            spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 3f, SpriteEffects.None, 0);

            var frame = (int)(Main.GlobalTimeWrappedHourly * 10f) % 6;
            texture = TextureAssets.Flag;
            rectangle.X = (int)(rectangle.Width * frame);

            spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 3f, SpriteEffects.None, 0);

            return true;
        }
    }
}