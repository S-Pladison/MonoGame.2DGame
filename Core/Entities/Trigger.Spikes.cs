using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;

namespace Pladi.Core.Entities
{
    public class SpikesTrigger : Trigger
    {
        // [constructors]

        public SpikesTrigger()
        {
            Width = 32;
            Height = 32;
            OnTriggerEnter += OnTriggeredByPlayer;

            SetMassAsImmovable();
        }

        // [protected methods]

        protected override bool PreDraw(SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.Spikes;
            var origin = new Vector2(texture.Width, texture.Height) * 0.5f;

            spriteBatch.Draw(texture, Hitbox.Center, null, Color.White, 0f, origin, 3f, SpriteEffects.None, 0);

            return true;
        }

        // [private methods]

        private void OnTriggeredByPlayer(Entity other)
        {
            if (other is PlayerEntity player)
            {
                player.Kill();
            }
        }
    }
}