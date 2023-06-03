using Microsoft.Xna.Framework;
using Pladi.Utilities.DataStructures;

namespace Pladi.Core.Graphics.Lighting
{
    public class Light
    {
        // [public properties and fields]

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;

                UpdateVisibleArea();
            }
        }

        public float Radius
        {
            get => radius;
            set
            {
                radius = value;

                UpdateVisibleArea();
            }
        }

        public RectangleF VisibleArea
        {
            get => area;
            set
            {
                radius = value.Width / 2;
                position = value.Center - Vector2.One * radius;

                UpdateVisibleArea();
            }
        }

        // [private properties and fields]

        private Color color;
        private Vector2 position;
        private float radius;
        private RectangleF area;

        // [constructors]

        public Light(Color color, Vector2 position, float radius)
        {
            this.color = color;
            this.position = position;
            this.radius = radius;

            UpdateVisibleArea();
        }

        // [private methods]

        private void UpdateVisibleArea()
        {
            area = new RectangleF(position.X - radius, position.Y - radius, radius * 2, radius * 2);
        }
    }
}