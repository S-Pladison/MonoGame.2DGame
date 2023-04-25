using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Collisions;
using Pladi.Utilities.DataStructures;
using System;

namespace Pladi.Core.Entities
{
    public class CrateEntity : Entity
    {
        // [public properties and fields]

        public override EdgeF[] ShadowCastEdges
        {
            get
            {
                var hitbox = Hitbox;
                hitbox.X -= 0.5f;
                hitbox.Width += 1f;

                return RectangleF.GetEdges(hitbox);
            }
        }

        // [constructors]

        public CrateEntity()
        {
            Mass = 20f;

            Width = 47f;
            Height = 48f;
        }

        // [protected methods]

        protected override bool PreUpdate(LevelCollision levelCollision)
        {
            var movement = MathF.Pow(MathF.Abs(Velocity.X) * 48 * 0.25f, 0.9f) * Math.Sign(Velocity.X);
            Velocity -= movement * Vector2.UnitX * Main.DeltaTime;

            return true;
        }

        protected override bool PreDraw(SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.Crate;
            var origin = new Vector2(texture.Width, texture.Height) * 0.5f;

            spriteBatch.Draw(texture, Hitbox.Center, null, Color.White, 0f, origin, 3f, SpriteEffects.None, 0);

            return true;
        }
    }
}