using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Scenes;

namespace Pladi.Core.Entities
{
    public class DeadZoneTrigger : Trigger
    {
        // [public properties and fields]

        public bool IsDangerous { get; set; }

        // [constructors]

        public DeadZoneTrigger(LevelScene scene) : base(scene)
        {
            IsDangerous = true;
            Width = 48;
            Height = 48;
            OnTriggerEnter += OnTriggeredByPlayer;

            SetMassAsImmovable();
        }

        // [protected methods]

        protected override bool PreDraw(SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.DeadZone;
            var origin = new Vector2(texture.Width * 0.25f, texture.Height * 0.5f);
            var width = texture.Width / 2;
            var rectangle = new Rectangle(IsDangerous ? 0 : width, 0, width, texture.Height);
            var position = Hitbox.Center;

            spriteBatch.Draw(texture, position, rectangle, Color.White * (IsDangerous ? 1f : 0.5f), 0f, origin, 3f, SpriteEffects.None, 0);

            return false;
        }

        // [private methods]

        private void OnTriggeredByPlayer(Entity other)
        {
            if (!IsDangerous || other is not PlayerEntity player) return;

            player.Kill();
        }
    }
}