using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.Collisions;
using Pladi.Utilities.DataStructures;
using System;

namespace Pladi.Core.Entities
{
    public abstract class Entity
    {
        // [public static properties and fields]

        public static readonly float Gravity;

        // [static constructors]

        static Entity()
        {
            Gravity = 48f * 16f;
        }

        // [public properties and fields]

        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                OnChangePosition?.Invoke();
            }
        }

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

        public float Mass
        {
            get => mass;
            set => mass = Math.Max(value, 0);
        }

        public bool IsImmovable
        {
            get => Mass < 0.0001f;
        }

        public bool IsTrigger
        {
            get => isTrigger;
        }

        public virtual EdgeF[] ShadowCastEdges
        {
            get => null;
        }

        public Action OnChangePosition;
        public Vector2 Velocity;
        public float Width;
        public float Height;

        // [private properties and fields]

        private readonly bool isTrigger;

        private Vector2 position;
        private float mass;

        // [constructors]

        public Entity()
        {
            mass = 1f;
            isTrigger = this is Trigger;
        }

        // [public methods]

        public void SetMassAsImmovable()
            => mass = 0.00001f;

        public void Update(LevelCollision levelCollision)
        {
            if (PreUpdate(levelCollision) && !IsImmovable)
            {
                var gravityScale = 1f;

                ModifyGravityScale(ref gravityScale);
                ApplyGravity(gravityScale);

                Position += Velocity * Main.DeltaTime;

                ResolveCollisionBetweenEntities(levelCollision);
            }

            PostUpdate(levelCollision);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (PreDraw(spriteBatch))
            {
                // ...
            }

            PostDraw(spriteBatch);
        }

        // [protected methods]

        protected virtual bool PreUpdate(LevelCollision levelCollision) => true;
        protected virtual void PostUpdate(LevelCollision levelCollision) { }
        protected virtual void ModifyGravityScale(ref float scale) { }
        protected virtual bool PreDraw(SpriteBatch spriteBatch) => true;
        protected virtual void PostDraw(SpriteBatch spriteBatch) { }
        protected virtual void OnCollide(CollisionInfo info) { }

        protected void ResolveCollisionBetweenEntities(LevelCollision levelCollision)
        {
            var entities = levelCollision.GetEntitiesIntersectsWithRect(Hitbox);

            for (int j = 0; j < entities.Count; j++)
            {
                if (entities[j].IsTrigger || entities[j] == this) continue;

                ResolveCollisionBetweenEntity(entities[j]);
            }
        }

        // [private methods]

        private void ApplyGravity(float gravityScale)
        {
            Velocity.Y += Gravity * gravityScale * Main.DeltaTime;
        }

        private void ResolveCollisionBetweenEntity(Entity other)
        {
            Vector2 penetration;

            // ...

            if (IsTrigger)
            {
                if (other.IsImmovable) return;

                penetration = RectangleF.Penetration(Hitbox, other.Hitbox);

                if (penetration == Vector2.Zero) return;

                OnCollide(new CollisionInfo(other, penetration));

                return;
            }

            // ...

            penetration = RectangleF.Penetration(Hitbox, other.Hitbox);

            if (penetration == Vector2.Zero) return;

            if (other.IsImmovable)
            {
                Position -= penetration;
            }
            else if (IsImmovable)
            {
                other.Position += penetration;
            }
            else
            {
                var halfPenetration = penetration * 0.5f;

                other.Position += halfPenetration;
                Position -= halfPenetration;
            }

            // ...

            var vector = (Velocity - other.Velocity);
            var normal = Vector2.Normalize(-penetration);
            var dot = Vector2.Dot(vector, normal);

            vector = ((dot > 0.01f) ? Vector2.Zero : -(normal * dot));

            var invMass = !IsImmovable ? (1f / Mass) : 0f;
            var otherInvMass = !other.IsImmovable ? (1f / other.Mass) : 0f;
            var totalInvMass = invMass + otherInvMass;
            var responseFraction = invMass / totalInvMass;
            var otherResponseFraction = otherInvMass / totalInvMass;

            Velocity += vector * responseFraction;
            other.Velocity -= vector * otherResponseFraction;

            OnCollide(new CollisionInfo(other, penetration));
            other.OnCollide(new CollisionInfo(this, -penetration));
        }
    }
}