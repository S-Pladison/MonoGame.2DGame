using Pladi.Core.Collisions;
using System;
using System.Collections.Generic;

namespace Pladi.Core.Entities
{
    public abstract class Trigger : Entity
    {
        // [public properties and fields]

        public Action<Entity> OnTriggerEnter;
        public Action<Entity> OnTriggerExit;

        // [private properties and fields]

        private readonly List<Entity> entityHistory;

        // [constructors]

        public Trigger()
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
                if (entityHistory[i].Hitbox.Intersects(Hitbox)) continue;

                OnTriggerExit?.Invoke(entityHistory[i]);
                entityHistory.Remove(entityHistory[i]);

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