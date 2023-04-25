using Pladi.Core.Entities;
using Pladi.Core.Tiles;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace Pladi.Core.Collisions
{
    public class LevelCollision
    {
        private readonly TileLayer tileLayer;
        private readonly SpatialHash<Entity> entitySpatialHash;

        // ...

        public LevelCollision(TileLayer tileLayer, SpatialHash<Entity> entitySpatialHash)
        {
            this.tileLayer = tileLayer;
            this.entitySpatialHash = entitySpatialHash;
        }

        // ...

        public bool IsRectCollideWithEntities(RectangleF rectangle, params Entity[] ignoreEntities)
            => IsRectCollideWithEntities(rectangle, ignoreEntities.AsEnumerable());

        public bool IsRectCollideWithEntities(RectangleF rectangle, IEnumerable<Entity> ignoreEntities = null)
        {
            if (tileLayer.IsCollideWithRect(rectangle))
                return true;

            var entities = entitySpatialHash.GetObjectsIntersectsWithRect(rectangle);

            for (int i = 0; i < entities.Count; i++)
            {
                if (!entities[i].IsTrigger
                    && entities[i].Hitbox.Intersects(rectangle)
                    && !(ignoreEntities?.Contains(entities[i]) ?? false))
                    return true;
            }

            return false;
        }

        public List<Entity> GetEntitiesIntersectsWithRect(RectangleF rectangle)
        {
            var entities = tileLayer.GetTileEntitiesIntersectsWithRect(rectangle);
            entities.AddRange(entitySpatialHash.GetObjectsIntersectsWithRect(rectangle));

            return entities;
        }
    }
}