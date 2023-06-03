using Pladi.Core.Collisions;
using Pladi.Core.Scenes;
using System;
using System.Collections.Generic;

namespace Pladi.Core.Entities
{
    public abstract class Trigger : Entity
    {
        // [public properties and fields]

        public Action<Entity> OnTriggerEnter;
        public Action<Entity> OnTriggerExit;

        public bool AnyEntitiesInHistory
        {
            get => entityHistory.Count > 0;
        }

        // [private properties and fields]

        private readonly List<Entity> entityHistory;

        // [constructors]

        public Trigger(LevelScene scene) : base(scene)
        {
            entityHistory = new();

            SetMassAsImmovable();
        }

        // [protected methods]

        protected virtual bool PreUpdateTrigger(LevelCollision levelCollision) => true;

        protected sealed override bool PreUpdate(LevelCollision levelCollision)
        {
            for (int i = 0; i < entityHistory.Count; i++)
            {
                var other = entityHistory[i];

                if (other.Hitbox.Intersects(Hitbox)) continue;

                entityHistory.Remove(other);
                OnTriggerExit?.Invoke(other);

                i--;
            }

            bool flag = PreUpdateTrigger(levelCollision);

            if (flag && IsImmovable)
            {
                ResolveCollisionBetweenEntities(levelCollision);
            }

            return flag;
        }

        protected override void OnCollide(CollisionInfo info)
        {
            if (entityHistory.Contains(info.Other)) return;

            entityHistory.Add(info.Other);
            OnTriggerEnter?.Invoke(info.Other);
        }
    }
}