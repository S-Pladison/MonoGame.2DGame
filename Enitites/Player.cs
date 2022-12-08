using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Enums;
using Pladi.Input;
using Pladi.Tiles;
using System;

namespace Pladi.Enitites
{
    public class Player : Entity
    {
        public const float MoveSpeed = 32 * 5;
        public const float JumpSpeed = 32 * 10;
        public const float GravitySpeed = 32 * 0.3f;
        public const float MaxFallSpeed = 32 * 32;
        public const float Scale = 2f;

        // ...

        private bool canJump;
        private Texture2D texture;
        private Direction2Types direction;

        // ...

        public Player(Texture2D texture)
        {
            this.texture = texture;

            Size = new Vector2(texture.Width, texture.Height) * Scale;
        }

        // ...

        public void Update(float delta, Tilemap tilemap)
        {
            var input = Main.InputManager;

            UpdateMoveVertical(input);
            UpdateMoveHorizontal(input);
            UpdateGravity();
            UpdatePosition(delta);

            tilemap.TileCollisionWithEntity(this, out CollisionSides collisionFlags);

            canJump = collisionFlags.HasFlag(CollisionSides.Buttom);
        }

        public void UpdateMoveVertical(InputManager input)
        {
            if (canJump && input.JustPressed(Keys.Space))
            {
                canJump = false;
                Velocity.Y = -JumpSpeed;
            }
        }

        public void UpdateMoveHorizontal(InputManager input)
        {
            Velocity.X *= 0.92f;

            if (input.IsPressed(Keys.A))
            {
                Velocity.X = -MoveSpeed;
            }

            if (input.IsPressed(Keys.D))
            {
                Velocity.X = MoveSpeed;
            }

            direction = Velocity.X >= 0 ? Direction2Types.Right : Direction2Types.Left;
        }

        public void UpdateGravity()
        {
            Velocity.Y += GravitySpeed;
            Velocity.Y = MathF.Min(MaxFallSpeed, Velocity.Y);
        }

        public void UpdatePosition(float delta)
        {
            Position += Velocity * delta;
        }

        // ...

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var spriteEffects = direction.Equals(Direction2Types.Left) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, Position, null, canJump ? Color.SkyBlue : Color.Red, 0f, Vector2.Zero, Scale, spriteEffects, 0);
        }
    }
}