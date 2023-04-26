using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;

namespace Pladi.Core.Input
{
    public class MouseDrawerComponent : BasicComponent
    {
        // [public methods]

        public override void Initialize()
        {
            Game.IsMouseVisible = false;
            DrawOrder = int.MaxValue;
            Enabled = false;
        }

        public override void Draw(GameTime gameTime)
        {
            Main.SpriteBatch.Begin();
            Main.SpriteBatch.Draw(TextureAssets.Cursor, ILoadable.GetInstance<InputComponent>().MousePosition, null, Color.White, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0);
            Main.SpriteBatch.End();
        }
    }
}
