using Microsoft.Xna.Framework;
using Pladi.Core.Entities;

namespace Pladi.Core.Collisions
{
    public class CollisionInfo
    {
        public readonly Entity Other;
        public readonly Vector2 Penetration;

        // ...

        public CollisionInfo(Entity other, Vector2 penetration)
        {
            Other = other;
            Penetration = penetration;
        }
    }
}