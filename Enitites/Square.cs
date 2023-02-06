using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Enitites
{
    public class Square : Entity
    {
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var indexes = new short[4];
            var effect = new SpriteEffect(spriteBatch.GraphicsDevice);

            var vp = new VertexPositionColor[4]
            {
                new(new(0, 0, 0), Color.Red),
                new(new(10, 10, 0), Color.Red),
                new(new(0, 0, 0), Color.Red),
                new(new(0, 0, 0), Color.Red)
            };

            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vp, 0, vp.Length / 2);
        }
    }
}
