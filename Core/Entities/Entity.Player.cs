using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.Collisions;
using Pladi.Core.Graphics.Particles;
using Pladi.Core.Input;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;
using System;

namespace Pladi.Core.Entities
{
    public class PlayerEntity : Entity
    {
        // [private properties and fields]

        private bool IsJumping { get => jumpTimer >= 0; }

        private readonly float moveSpeed;
        private readonly float acceleration;
        private readonly float decceleration;
        private readonly float velPower;

        private readonly float fallGravityMult;

        private readonly float jumpPower;
        private readonly float heldJumpTime;
        private readonly float heldJumpPowerMult;

        private readonly Texture2D texture;

        private bool onGround;
        private float jumpTimer;

        private bool direction;
        private float frameCounter;

        private ParticleSystem particleSystem;
        private float runParticleTimer;

        // [constructors]

        public PlayerEntity()
        {
            Width = 12 * 3;
            Height = 15 * 3;

            Mass = 5f;

            // ...

            moveSpeed = 48 * 5f;
            acceleration = 48 * 1f;
            decceleration = 48 * 0.25f;
            velPower = 0.9f;

            fallGravityMult = 2f;

            jumpPower = 48 * 6f;
            heldJumpTime = 0.33f;
            heldJumpPowerMult = 1.66f;

            texture = TextureAssets.Player;

            onGround = false;
            jumpTimer = -1;

            // ...

            particleSystem = new ParticleSystem(TextureAssets.Pixel, particle =>
            {
                var progress = 1.0f - particle.Timer / particle.InitTimer;

                particle.Alpha = (int)MathHelper.Lerp(255, 0, MathF.Pow(progress, 5));
                particle.Scale = MathHelper.Lerp(2, 8, progress);
            });
        }

        // [protected methods]

        protected override bool PreUpdate(LevelCollision levelCollision)
        {
            var input = ILoadable.GetInstance<InputComponent>();

            GetInputMoveVector(input, out Vector2 inputVector);

            // Горизонтальное перемещение

            var inputXSign = Math.Sign(inputVector.X);

            if (inputXSign is not 0)
                direction = inputXSign > 0;

            float targetSpeed = inputVector.X * moveSpeed;
            float speedDif = targetSpeed - Velocity.X;
            float accelRate = (MathF.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
            float movement = MathF.Pow(MathF.Abs(speedDif) * accelRate, velPower) * Math.Sign(speedDif);

            Velocity += movement * Vector2.UnitX * Main.DeltaTime;

            // Вертикальное перемещение

            Update_CheckOnGrount(levelCollision);
            Update_Jump(input, inputVector);

            // Остальное

            if (!onGround)
            {
                frameCounter = 0;
                runParticleTimer = 0;
            }
            else
            {
                frameCounter += Main.DeltaTime * 8f;

                Update_OnRunParticles();
            }

            particleSystem.Update();

            return true;
        }

        protected override void ModifyGravityScale(ref float scale)
        {
            if (onGround || Velocity.Y < 0) return;

            scale = fallGravityMult;
        }

        protected override bool PreDraw(SpriteBatch spriteBatch)
        {
            particleSystem.Draw(spriteBatch);

            GetCurrentSpriteFrameIndex(out int frame);

            var spriteEffect = direction ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var drawPosition = new Vector2(Hitbox.Center.X, Hitbox.Bottom);
            var frameRect = new Rectangle(frame * 24, 0, 24, 24);
            var origin = new Vector2(12, 21);

            spriteBatch.Draw(texture, drawPosition, frameRect, Color.White, 0f, origin, 3f, spriteEffect, 0f);

            return true;
        }

        // [private methods]

        private void Update_CheckOnGrount(LevelCollision levelCollision)
        {
            onGround = levelCollision.IsRectCollideWithEntities(new RectangleF(Position.X, Hitbox.Bottom - 2, Width, 6), this);
        }

        private void Update_Jump(InputComponent input, Vector2 inputVector)
        {
            if (IsJumping
                && jumpTimer < heldJumpTime
                && inputVector.Y < 0)
            {
                Velocity.Y -= jumpPower * heldJumpPowerMult * Main.DeltaTime;
                jumpTimer += Main.DeltaTime;
            }

            if (!IsJumping && onGround)
            {
                if (input.JustPressed(Keys.W) || input.JustPressed(Keys.Space))
                {
                    Velocity.Y = -jumpPower;

                    Update_OnJumpParticles();

                    onGround = false;
                    jumpTimer = 0;
                }
                else
                {
                    jumpTimer = -1;
                }
            }

            if (IsJumping
                && (input.JustReleased(Keys.W) || input.JustReleased(Keys.Space)))
            {
                jumpTimer = -1;
            }
        }

        private void Update_OnJumpParticles()
        {
            var position = new Vector2(Hitbox.Center.X, Hitbox.Bottom - 1);

            for (int i = 0; i < 16; i++)
            {
                var velocity = new Vector2(Main.Rand.NextFloat(-48f, 48f), 0).RotateBy(Main.Rand.NextFloat(-0.3f, 0.3f));

                particleSystem.Add(new Particle(position, velocity, Colors.Main, 0f, 1f, Main.Rand.NextFloat(0.4f, 0.8f)));
            }
        }

        private void Update_OnRunParticles()
        {
            if (Math.Abs(Velocity.X) < 48f) return;

            runParticleTimer += Main.DeltaTime * 120f;

            if ((int)runParticleTimer < 1) return;

            runParticleTimer = 0;

            if (Main.Rand.Next(2) != 0) return;

            var position = new Vector2(Hitbox.Center.X, Hitbox.Bottom - 1);
            var velocity = new Vector2(-MathF.Sign(Velocity.X) * 0.35f, 0).RotateBy(Main.Rand.NextFloat(-0.2f, 0.2f));

            particleSystem.Add(new Particle(position, velocity, Colors.Main, 0f, 1f, Main.Rand.NextFloat(0.2f, 0.4f)));
        }

        private void GetInputMoveVector(InputComponent input, out Vector2 vector)
        {
            vector = Vector2.Zero;

            if (input.IsPressed(Keys.A))
                vector -= Vector2.UnitX;

            if (input.IsPressed(Keys.D))
                vector += Vector2.UnitX;

            if (input.IsPressed(Keys.W) || input.IsPressed(Keys.Space))
                vector -= Vector2.UnitY;

            if (input.IsPressed(Keys.S))
                vector += Vector2.UnitY;
        }

        private void GetCurrentSpriteFrameIndex(out int frame)
        {
            int frameCount;
            int frameOffset;

            if (!onGround)
            {
                frameCount = 1;
                frameOffset = 12;
            }
            else if (Math.Abs(Velocity.X) > 16f)
            {
                frameCount = 6;
                frameOffset = 4;
            }
            else
            {
                frameCount = 4;
                frameOffset = 0;
            }

            frame = frameOffset + (int)(frameCounter % frameCount);
        }
    }
}