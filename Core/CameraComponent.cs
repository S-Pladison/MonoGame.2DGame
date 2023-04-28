using Microsoft.Xna.Framework;

namespace Pladi.Core
{
    public class CameraComponent : BasicComponent
    {
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;

                UpdateMatrices();
            }
        }

        public Matrix ViewMatrix { get; private set; }
        public Matrix InvertViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix TransformMatrix { get; private set; }
        public Rectangle VisibleArea { get; private set; }

        private Vector2 position;

        // ...

        public CameraComponent()
        {
            UpdateOrder = int.MinValue;
            Visible = false;
        }

        // ...

        public override void Update(GameTime gameTime)
        {
            //UpdateMatrices();
        }

        public void ResetPosition()
            => Position = Vector2.Zero;

        // ...

        private void UpdateMatrices()
        {
            var viewport = Main.SpriteBatch.GraphicsDevice.Viewport;
            //var k = ((float)viewport.Width / Main.MinScreenWidth + (float)viewport.Height / Main.MinScreenHeight) / 2f;

            ViewMatrix = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0))
                       * Matrix.CreateTranslation(viewport.Width * 0.5f, viewport.Height * 0.5f, 0);
            InvertViewMatrix = Matrix.Invert(ViewMatrix);
            ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0f, -1f);
            TransformMatrix = ViewMatrix * ProjectionMatrix;
            VisibleArea = new
            (
                (int)(position.X - viewport.Width / 2),
                (int)(position.Y - viewport.Height / 2),
                (int)(viewport.Width),
                (int)(viewport.Height)
            );
        }
    }
}
