using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Scenes;

namespace Pladi.Core.Entities
{
    public class PressurePlateTrigger : Trigger
    {
        // [constructors]

        public PressurePlateTrigger(LevelScene scene) : base(scene)
        {
            Width = 32;
            Height = 16;

            OnTriggerEnter += _ => Scene.UpdateDeadZoneInfo();
            OnTriggerExit += _ => Scene.UpdateDeadZoneInfo();

            SetMassAsImmovable();
        }

        // [protected methods]

        protected override bool PreDraw(SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.PressurePlate;
            var origin = new Vector2(texture.Width * 0.25f, texture.Height);
            var width = texture.Width / 2;
            var rectangle = new Rectangle(AnyEntitiesInHistory ? width : 0, 0, width, texture.Height);
            var position = Position + new Vector2(Width * 0.5f, Height);

            spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 3f, SpriteEffects.None, 0);

            return true;
        }
    }
}