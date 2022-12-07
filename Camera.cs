using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi
{
    public sealed class Camera
    {
        public Vector2 Location { get; set; }
        public float Zoom { get; set; }
        public float Rotation { get; set; }
        public Viewport Viewport { get; set; }

        public Matrix TransformMatrix
        {
            get => Matrix.CreateTranslation(new Vector3(-Location.X, -Location.Y, 0))
                 * Matrix.CreateRotationZ(0f)
                 * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1f))
                 * Matrix.CreateTranslation(new Vector3(Viewport.Width * 0.5f, Viewport.Height * 0.5f, 0));
        }

        public Rectangle VisibleArea
        {
            get => new
            (
                (int)(Location.X - Viewport.Width / 2 / Zoom),
                (int)(Location.Y - Viewport.Height / 2 / Zoom),
                (int)(Viewport.Width / Zoom),
                (int)(Viewport.Height / Zoom)
            );
        }

        private Vector2 location;

        // ...

        public Camera(Viewport viewport, float zoom = 1f, float rotation = 0f)
        {
            Viewport = viewport;
            Zoom = zoom;
            Rotation = rotation;
        }

        // ...

        public Vector2 ScreenToWorldSpace(in Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(TransformMatrix);
            return Vector2.Transform(point, invertedMatrix);
        }
    }
}