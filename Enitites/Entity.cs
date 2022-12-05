using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pladi.Enitites
{
    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public int Width;
        public int Height;

        // ...

        public Vector2 Center
        {
            get => new(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
            set => Position = new Vector2(value.X - (float)(Width / 2), value.Y - (float)(Height / 2));
        }

        public Vector2 Left
        {
            get => new(Position.X, Position.Y + (float)(Height / 2));
            set => Position = new Vector2(value.X, value.Y - (float)(Height / 2));
        }

        public Vector2 Right
        {
            get => new(Position.X + (float)Width, Position.Y + (float)(Height / 2));
            set => Position = new Vector2(value.X - (float)Width, value.Y - (float)(Height / 2));
        }

        public Vector2 Top
        {
            get => new(Position.X + (float)(Width / 2), Position.Y);
            set => Position = new Vector2(value.X - (float)(Width / 2), value.Y);
        }

        public Vector2 TopLeft
        {
            get => Position;
            set => Position = value;
        }

        public Vector2 TopRight
        {
            get => new(Position.X + (float)Width, Position.Y);
            set => Position = new Vector2(value.X - (float)Width, value.Y);
        }

        public Vector2 Bottom
        {
            get => new(Position.X + (float)(Width / 2), Position.Y + (float)Height);
            set => Position = new Vector2(value.X - (float)(Width / 2), value.Y - (float)Height);
        }

        public Vector2 BottomLeft
        {
            get => new(Position.X, Position.Y + (float)Height);
            set => Position = new Vector2(value.X, value.Y - (float)Height);
        }

        public Vector2 BottomRight
        {
            get => new(Position.X + (float)Width, Position.Y + (float)Height);
            set => Position = new Vector2(value.X - (float)Width, value.Y - (float)Height);
        }

        public Vector2 Size
        {
            get => new(Width, Height);
            set
            {
                Width = (int)value.X;
                Height = (int)value.Y;
            }
        }

        public Rectangle Hitbox
        {
            get => new((int)Position.X, (int)Position.Y, Width, Height);
            set
            {
                Position = new Vector2(value.X, value.Y);
                Width = value.Width;
                Height = value.Height;
            }
        }

        // ...

        public float AngleTo(Vector2 destination)
            => MathF.Atan2(destination.Y - Center.Y, destination.X - Center.X);

        public float AngleFrom(Vector2 source)
            => MathF.Atan2(Center.Y - source.Y, Center.X - source.X);

        public float Distance(Vector2 other)
            => Vector2.Distance(Center, other);

        public float DistanceSQ(Vector2 other)
            => Vector2.DistanceSquared(Center, other);

        public Vector2 DirectionTo(Vector2 destination)
            => Vector2.Normalize(destination - Center);

        public Vector2 DirectionFrom(Vector2 source)
            => Vector2.Normalize(Center - source);

        public bool WithinRange(Vector2 target, float range)
            => Vector2.DistanceSquared(Center, target) <= range * range;

        // ...

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}
