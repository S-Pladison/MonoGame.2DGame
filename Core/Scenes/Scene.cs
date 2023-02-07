using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Core.Scenes
{
    public class Scene
    {
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        public virtual void Update() { }
        public virtual void OnResolutionChanged(int width, int height) { }
        public virtual void PreDraw(SpriteBatch spriteBatch) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}