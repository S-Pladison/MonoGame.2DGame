using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.Collisions;
using Pladi.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            /*Velocity += Vector2.UnitY * 100 * Main.DeltaTime;
            Velocity *= new Vector2(0.95f, 1);*/

            if (input.IsPressed(Keys.A))
            {
                Velocity += Vector2.UnitX * -300 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.D))
            {
                Velocity += Vector2.UnitX * 300 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.W))
            {
                Velocity += Vector2.UnitY * -300 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.S))
            {
                Velocity += Vector2.UnitY * 300 * Main.DeltaTime;
            }
        }

        public override void OnCollision(CollisionEventArgs args)
        {
            switch (args.Other)
            {
                case Box box:
                    OnCollisionWithTile(box, args);
                    break;
                default:
                    break;
            }
        }

        private void OnCollisionWithTile(Box box, CollisionEventArgs args)
        {
            Velocity += args.ContactNormal * Velocity.Abs() * (1 - args.ContactTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, Hitbox, Color.Blue);
        }
    }
}