using Microsoft.Xna.Framework;

namespace Pladi.Core.Graphics.Particles
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Rectangle Frame;
        public Color Color;
        public int Alpha;
        public float Rotation;
        public float Scale;
        public float Timer;

        public readonly float InitTimer;

        // ...

        public Particle(Vector2 position, Vector2 velocity, Color color, float rotation, float scale, float timer)
        : this(position, velocity, default, color, 255, rotation, scale, timer)
        {
            // ...
        }

        public Particle(Vector2 position, Vector2 velocity, Rectangle frame, Color color, int alpha, float rotation, float scale, float timer)
        {
            Position = position;
            Velocity = velocity;
            Frame = frame;
            Color = color;
            Alpha = alpha;
            Rotation = rotation;
            Scale = scale;
            Timer = InitTimer = timer;
        }
    }
}