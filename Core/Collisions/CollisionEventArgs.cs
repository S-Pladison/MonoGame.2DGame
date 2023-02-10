using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Collisions
{
    public class CollisionEventArgs : EventArgs
    {
        public ICollidable Other { get; init; }
        public Vector2 ContactPoint { get; init; }
        public Vector2 ContactNormal { get; init; }
        public float ContactTime { get; init; }
    }
}