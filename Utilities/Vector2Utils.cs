using Microsoft.Xna.Framework;
using System;

namespace Pladi.Utilities
{
    public static class Vector2Utils
    {
        public static Vector2 Abs(this Vector2 vector)
            => new(Math.Abs(vector.X), Math.Abs(vector.Y));

        public static Vector2 RotateBy(this Vector2 vector, float angle)
            => new(vector.X * MathF.Cos(angle) - vector.Y * MathF.Sin(angle), vector.X * MathF.Sin(angle) + vector.Y * MathF.Cos(angle));

        public static float ToRotation(this Vector2 vector)
            => (float)Math.Atan2(vector.Y, vector.X);
    }
}