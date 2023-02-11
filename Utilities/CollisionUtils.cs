using Microsoft.Xna.Framework;
using Pladi.Utilities.DataStructures;
using System;

namespace Pladi.Utilities
{
    public static class CollisionUtils
    {
        public static bool CheckRayAabbCollision(Vector2 rayPosition, Vector2 rayDirection, RectangleF aabbDimensions, out Vector2 contactPoint, out Vector2 contactNormal, out float contactTime)
        {
            contactNormal = Vector2.Zero;
            contactPoint = Vector2.Zero;
            contactTime = 0;

            var vector = aabbDimensions.Location - rayPosition;
            var tNear = vector / rayDirection;

            if (float.IsNaN(tNear.X) || float.IsNaN(tNear.Y)) return false;

            var tFar = (vector + aabbDimensions.Size) / rayDirection;

            if (float.IsNaN(tFar.X) || float.IsNaN(tFar.Y)) return false;
            if (tNear.X > tFar.X) (tNear.X, tFar.X) = (tFar.X, tNear.X);
            if (tNear.Y > tFar.Y) (tNear.Y, tFar.Y) = (tFar.Y, tNear.Y);
            if (tNear.X > tFar.Y || tNear.Y > tFar.X) return false;

            contactTime = Math.Max(tNear.X, tNear.Y);
            float tHitFar = Math.Min(tFar.X, tFar.Y);

            if (tHitFar < 0) return false;

            contactPoint = rayPosition + contactTime * rayDirection;

            if (tNear.X > tNear.Y) contactNormal = new Vector2(rayDirection.X < 0 ? 1 : -1, 0);
            else if (tNear.X < tNear.Y) contactNormal = new Vector2(0, rayDirection.Y < 0 ? 1 : -1);

            return true;
        }
    }
}