using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Entities
{
    public class Player : Entity
    {
        public override void Update()
        {
            var input = Main.InputManager;

            if (input.IsPressed(Keys.A))
            {
                Position.X += -100 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.D))
            {
                Position.X += 100 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.W))
            {
                Position.Y += -100 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.S))
            {
                Position.Y += 100 * Main.DeltaTime;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, Hitbox, Color.Blue);
        }
    }
}