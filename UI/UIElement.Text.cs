using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Utils;

namespace Pladi.UI
{
    public class TextUIElement : UIElement
    {
        private readonly SpriteFont font;

        private string text;
        private float scale;

        private Vector2 textOrigin;
        private Vector2 textSize;
        private Color color;

        // ...

        public TextUIElement(SpriteFont font, string text, Color textColor, float scale = 1f)
        {
            this.font = font;
            this.color = textColor;
            this.textOrigin = new Vector2(0, 0.5f);

            SetText(text, scale);
        }

        // ...

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawStringWithShadow(font, $"{text}", Position, color, 0, textOrigin, scale, 1f);
        }

        // ...

        public void SetText(string text, float scale)
        {
            this.text = text;
            this.scale = scale;

            var vector = font.MeasureString(text);
            var vector2 = textSize = vector * scale;

            Width = (int)vector2.X;
            Height = (int)vector2.Y;
        }

        public void SetColor(Color color)
            => this.color = color;
    }
}