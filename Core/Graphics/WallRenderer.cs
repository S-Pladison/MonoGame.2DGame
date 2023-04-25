using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Entities;
using Pladi.Core.Graphics.Renderers;
using Pladi.Core.Tiles;
using Pladi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Graphics
{
    public class WallRenderer : Renderer
    {
        protected override void OnRender(SpriteBatch spriteBatch)
        {
            if (extraData is not TileLayer wallLayer) return;

            var camera = ILoadable.GetInstance<CameraComponent>();

            wallLayer.Draw(spriteBatch, Color.White, camera);
        }
    }
}