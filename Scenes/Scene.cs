using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Scenes
{
    public abstract class Scene
    {
        public virtual void Init() { }
        public virtual void LoadContent(ContentManager content) { }
        public virtual void OnActivate() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void OnResolutionChanged(int width, int height) { }
        public virtual void PreDraw(GameTime gameTime, SpriteBatch spriteBatch) { }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}