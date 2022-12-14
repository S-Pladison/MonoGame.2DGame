using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Core.Scenes
{
    public class Scene
    {
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void OnResolutionChanged(int width, int height) { }
        public virtual void PreDraw(GameTime gameTime, SpriteBatch spriteBatch) { }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}