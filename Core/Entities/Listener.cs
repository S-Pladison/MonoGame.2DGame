using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Collisions;
using Pladi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Entities
{
    public class Listener : Entity
    {
        bool flag = false;

        public override void Update()
        {
            //flag = false;
        }

        public override void OnCollision(CollisionEventArgs args)
        {
            switch (args.Other)
            {
                case Player player:
                    {
                        throw new Exception(":(");
                    }
                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, Hitbox, flag ? Color.Cyan : Color.Pink);
        }
    }
}
