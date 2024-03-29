﻿using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Pladi.Utilities.DataStructures
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        // [public static properties and fields]

        public static RectangleF Empty { get; }

        // [public properties and fields]

        public float Left
        {
            get => X;
        }

        public float Right
        {
            get => X + Width;
        }

        public float Top
        {
            get => Y;
        }

        public float Bottom
        {
            get => Y + Height;
        }

        public bool IsEmpty
        {
            get
            {
                if (Width == 0 && Height == 0 && X == 0) return Y == 0;
                return false;
            }
        }

        public Vector2 Location
        {
            get => new(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Size
        {
            get => new(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Vector2 Center
        {
            get => new(X + Width / 2f, Y + Height / 2f);
        }

        [DataMember]
        public float X;

        [DataMember]
        public float Y;

        [DataMember]
        public float Width;

        [DataMember]
        public float Height;

        // [constructors]

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(Vector2 location, Vector2 size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.X;
            Height = size.Y;
        }

        // [operators]

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            if (a.X == b.X && a.Y == b.Y && a.Width == b.Width) return a.Height == b.Height;
            return false;
        }

        public static bool operator !=(RectangleF a, RectangleF b)
            => !(a == b);

        // [public static methods]

        public static RectangleF Intersect(RectangleF value1, RectangleF value2)
        {
            Intersect(ref value1, ref value2, out RectangleF result);
            return result;
        }

        public static void Intersect(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            if (value1.Intersects(value2))
            {
                float num = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
                float num2 = Math.Max(value1.X, value2.X);
                float num3 = Math.Max(value1.Y, value2.Y);
                float num4 = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
                result = new RectangleF(num2, num3, num - num2, num4 - num3);
            }
            else
            {
                result = new RectangleF(0, 0, 0, 0);
            }
        }

        public static RectangleF Union(RectangleF value1, RectangleF value2)
        {
            float num = Math.Min(value1.X, value2.X);
            float num2 = Math.Min(value1.Y, value2.Y);
            return new RectangleF(num, num2, Math.Max(value1.Right, value2.Right) - num, Math.Max(value1.Bottom, value2.Bottom) - num2);
        }

        public static void Union(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            result.X = Math.Min(value1.X, value2.X);
            result.Y = Math.Min(value1.Y, value2.Y);
            result.Width = Math.Max(value1.Right, value2.Right) - result.X;
            result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
        }

        public static EdgeF[] GetEdges(RectangleF rectangle)
        {
            var points = new Vector2[]
            {
                rectangle.Location,
                rectangle.Location + new Vector2(rectangle.Width, 0),
                rectangle.Location + new Vector2(rectangle.Width, rectangle.Height),
                rectangle.Location + new Vector2(0, rectangle.Height)
            };

            var edges = new EdgeF[4];

            for (int i = 0; i < edges.Length; i++)
            {
                edges[i] = new EdgeF(points[i], points[(i + 1) % 4]);
            }

            return edges;
        }

        public static Vector2 Penetration(RectangleF value1, RectangleF value2)
            => Penetration(value1, value2, out _);

        public static Vector2 Penetration(RectangleF value1, RectangleF value2, out RectangleF intersect)
        {
            intersect = Intersect(value1, value2);

            if (intersect.Width < intersect.Height)
                return new(value1.Center.X < value2.Center.X ? intersect.Width : -intersect.Width, 0);
            else
                return new(0, value1.Center.Y < value2.Center.Y ? intersect.Height : -intersect.Height);
        }

        // [public methods]

        public override string ToString()
            => "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";

        public override bool Equals(object obj)
        {
            if (obj is RectangleF rect) return this == rect;
            return false;
        }

        public override int GetHashCode()
            => (((17 * 25 + X.GetHashCode()) * 25 + Y.GetHashCode()) * 25 + Width.GetHashCode()) * 25 + Height.GetHashCode();

        public bool Contains(int x, int y)
        {
            if (X <= x && x < X + Width && Y <= y) return y < Y + Height;
            return false;
        }

        public bool Contains(float x, float y)
        {
            if (X <= x && x < (float)(X + Width) && Y <= y) return y < (float)(Y + Height);
            return false;
        }

        public bool Contains(Vector2 value)
        {
            if (X <= value.X && value.X < X + Width && Y <= value.Y) return value.Y < Y + Height;
            return false;
        }

        public void Contains(ref Vector2 value, out bool result)
            => result = (X <= value.X && value.X < X + Width && Y <= value.Y && value.Y < Y + Height);

        public bool Contains(RectangleF value)
        {
            if (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y) return value.Y + value.Height <= Y + Height;
            return false;
        }

        public void Contains(ref RectangleF value, out bool result)
            => result = (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y && value.Y + value.Height <= Y + Height);

        public bool IsInside(RectangleF value)
            => value.Contains(this);

        public bool Equals(RectangleF other)
            => this == other;

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        public bool Intersects(RectangleF value)
        {
            if (value.Left < Right && Left < value.Right && value.Top < Bottom) return Top < value.Bottom;
            return false;
        }

        public void Intersects(ref RectangleF value, out bool result)
            => result = (value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom);

        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        public void Deconstruct(out float x, out float y, out float width, out float height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }
    }
}
