using Microsoft.Xna.Framework;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Collisions
{
    public interface ICollidable
    {
        RectangleF Hitbox { get; }
        Vector2 Velocity { get; }

        // ...

        void OnCollision(CollisionEventArgs args);
    }
}