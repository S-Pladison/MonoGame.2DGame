using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Scenes
{
    public class SplashScene : Scene
    {
        private readonly float blackScreenTime = 2;
        private float splashCounter;

        // ...

        public override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            splashCounter += 1 * delta;

            if (splashCounter >= blackScreenTime)
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
        }
    }
}