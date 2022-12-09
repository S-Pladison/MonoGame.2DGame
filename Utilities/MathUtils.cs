using Microsoft.Xna.Framework;
using Pladi.Utilities.DataStructures;
using System;

namespace Pladi.Utilities
{
    public static partial class PladiUtils
    {
        public static double SmoothDamp(double previousValue, double targetValue, ref double speed, double smoothTime, double dt)
        {
            double t1 = 0.36 * smoothTime;
            double t2 = 0.64 * smoothTime;
            double x = previousValue - targetValue;
            double newSpeed = speed + dt * (-1.0 / (t1 * t2) * x - (t1 + t2) / (t1 * t2) * speed);
            double newValue = x + dt * speed;
            speed = newSpeed;
            double result = targetValue + newValue;
            speed = double.IsNaN(speed) ? 0.0 : speed;
            return double.IsNaN(result) ? 0 : result;
        }

        public static Vector2 SmoothDamp(Vector2 previousValue, Vector2 targetValue, ref Vector2 speed, double smoothTime, double dt)
        {
            var speedX = (double)speed.X;
            var speedY = (double)speed.Y;

            var result = new Vector2((float)SmoothDamp(previousValue.X, targetValue.X, ref speedX, smoothTime, dt), (float)SmoothDamp(previousValue.Y, targetValue.Y, ref speedY, smoothTime, dt));

            speed.X = (float)speedX;
            speed.Y = (float)speedY;

            return result;
        }

        public static Rectangle ToRectangle(this RectangleF rectF)
            => new((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);

        public static RectangleF ToRectangleF(this Rectangle rect)
            => new(rect.X, rect.Y, rect.Width, rect.Height);

        public static float GetHorizontalIntersectionDepth(this RectangleF rectA, RectangleF rectB)
        {
            float halfWidthA = rectA.Width / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;

            float centerA = rectA.Left + halfWidthA;
            float centerB = rectB.Left + halfWidthB;

            float distanceX = centerA - centerB;
            float minDistanceX = halfWidthA + halfWidthB;

            if (Math.Abs(distanceX) >= minDistanceX) return 0f;
            return distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        }

        public static float GetVerticalIntersectionDepth(this RectangleF rectA, RectangleF rectB)
        {
            float halfHeightA = rectA.Height / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            float centerA = rectA.Top + halfHeightA;
            float centerB = rectB.Top + halfHeightB;

            float distanceY = centerA - centerB;
            float minDistanceY = halfHeightA + halfHeightB;

            if (Math.Abs(distanceY) >= minDistanceY) return 0f;
            return distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        }
    }
}