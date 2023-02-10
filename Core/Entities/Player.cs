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

            if (input.IsPressed(Keys.A))
            {
                Velocity.X += -100 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.D))
            {
                Velocity.X += 100 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.W))
            {
                Velocity.Y += -100 * Main.DeltaTime;
            }

            if (input.IsPressed(Keys.S))
            {
                Velocity.Y += 100 * Main.DeltaTime;
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
            //Velocity += Velocity * Vector2.Dot(args.ContactNormal, Velocity) * (1 - args.ContactTime) * Main.DeltaTime;
            Velocity += args.ContactNormal * new Vector2(Math.Abs(Velocity.X), Math.Abs(Velocity.Y)) * (1 - args.ContactTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, Hitbox, Color.Blue);
        }
    }
}