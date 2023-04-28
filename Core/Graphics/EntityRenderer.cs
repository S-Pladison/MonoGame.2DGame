using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.Entities;
using Pladi.Core.Graphics.Renderers;
using System.Collections.Generic;

namespace Pladi.Core.Graphics
{
    public class EntityRenderer : Renderer
    {
        public override int RenderOrder => int.MinValue;

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            if (extraData is not IList<Entity> entities) return;

            var camera = ILoadable.GetInstance<CameraComponent>();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.ViewMatrix);

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is PlayerEntity) continue;

                entities[i].Draw(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}