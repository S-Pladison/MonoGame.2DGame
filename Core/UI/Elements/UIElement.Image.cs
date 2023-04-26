using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Core.UI.Elements
{
    public class ImageUIElement : UIElement
    {
        public Texture2D Texture { get; private set; }

        // ...

        private Rectangle? textureFrame;
        private Color textureColor;

        // ...

        public ImageUIElement(Texture2D texture)
        {
            Texture = texture;

            this.textureColor = Color.White;

            Width.SetPixel(texture.Width);
            Height.SetPixel(texture.Height);
        }

        // ...

        public void SetFrame(float x, float y, float width, float height)
        {
            var frame = new Rectangle((int)x, (int)y, (int)width, (int)height);

            textureFrame = frame;

            Width.SetPixel(frame.Width);
            Height.SetPixel(frame.Height);
        }

        public void SetFrame(Rectangle frame)
        {
            textureFrame = frame;

            Width.SetPixel(frame.Width);
            Height.SetPixel(frame.Height);
        }

        public void SetColor(Color color)
            => textureColor = color;

        protected override void DrawThis(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, textureFrame, textureColor);
        }
    }
}