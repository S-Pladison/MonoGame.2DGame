using Microsoft.Xna.Framework;
using RectangleF = System.Drawing.RectangleF;

namespace Pladi.Utils
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
    }
}
