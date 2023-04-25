using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Pladi.Core.Graphics.Renderers
{
    public class RendererHandler : ILoadable
    {
        public int LoadOrder { get; set; }

        private readonly List<Renderer> renderers;

        // ...

        public RendererHandler()
        {
            renderers = new List<Renderer>();
        }

        // ...

        public void Add(Renderer renderer)
        {
            if (renderers.Contains(renderer)) return;

            renderers.Add(renderer);
            renderers.Sort((x, y) => x.CompareTo(y));
        }

        public void Render(GameTime gameTime)
        {
            foreach (var renderer in renderers)
            {
                renderer.Render(gameTime);
            }
        }
    }
}