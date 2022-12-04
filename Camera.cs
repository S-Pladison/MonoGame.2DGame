using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi
{
    public sealed class Camera
    {
        public Vector2 Position;
        public float Zoom;
        public Viewport Viewport;

        public Vector2 Center
        {
            get => new(Position.X + Viewport.Width / 2f, Position.Y + Viewport.Height / 2f);
            set => Position = new Vector2(value.X - Viewport.Width / 2f, value.Y - Viewport.Height / 2f);
        }

        public Matrix TransformMatrix =>
               Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0))
             * Matrix.CreateRotationZ(0f)
             * Matrix.CreateScale(Zoom);

        public Camera(Viewport viewport, float zoom = 1f)
        {
            Viewport = viewport;
            Zoom = zoom;
        }
    }
}