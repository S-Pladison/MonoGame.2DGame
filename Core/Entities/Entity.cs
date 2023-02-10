using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.Collisions;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Entities
{
    public abstract class Entity : ICollidable
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Position;
        public float Width;
        public float Height;

        // ...

        public RectangleF Hitbox
        {
            get => new(Position.X, Position.Y, Width, Height);
            set
            {
                Position = new(value.X, value.Y);
                Width = value.Width;
                Height = value.Height;
            }
        }

        // ...

        public Entity()
        {
            Position = Vector2.Zero;
        }

        // ...

        public virtual void Update() { }
        public virtual void OnCollision(CollisionEventArgs args) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        // ...

        public void CheckAndResolveCollision(ICollidable target)
        {
            if (Velocity == Vector2.Zero) return;

            var expandedTarget = target.Hitbox;
            expandedTarget.Location -= Hitbox.Size * 0.5f;
            expandedTarget.Size += Hitbox.Size;

            if (!CollisionUtils.CheckRayAabbCollision(Hitbox.Center, Velocity * Main.DeltaTime, expandedTarget, out Vector2 contactPoint, out Vector2 contactNormal, out float contactTime) || contactTime >= 1f || contactTime < 0f) return;

            OnCollision(new CollisionEventArgs()
            {
                Other = target,
                ContactPoint = contactPoint,
                ContactNormal = contactNormal,
                ContactTime = contactTime
            });
        }
    }
}