using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Core.Scenes
{
    public class SplashScene : Scene
    {
        private readonly float blackScreenTime = 2;
        private float splashCounter;

        // ...

        public override void Update()
        {
            splashCounter += 1 * Main.DeltaTime;

            if (splashCounter >= blackScreenTime)
            {
                SceneComponent.SetActiveScene<MenuScene>();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
        }
    }
}