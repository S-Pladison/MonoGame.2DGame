using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Core.Scenes
{
    public class Scene
    {
        public SceneComponent SceneComponent { get; private set; }

        // ...

        public Scene()
        {
            SceneComponent = ILoadable.GetInstance<SceneComponent>();
        }

        // ...

        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
        public virtual void Update() { }
        public virtual void OnResolutionChanged(int width, int height) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}