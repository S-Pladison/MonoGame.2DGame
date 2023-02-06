using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities;

namespace Pladi.Core.UI.Elements
{
    public class TextUIElement : UIElement
    {
        private readonly SpriteFont font;
        private string text;
        private float scale;

        // ...

        public string Text
        {
            get => text;
            set
            {
                text = value;
                RecreateText();
            }
        }

        public float FontScale
        {
            get => scale;
            set
            {
                scale = value;
                RecreateText();
            }
        }

        public Color FontColor { get; set; }
        public float ShadowSpread { get; set; }

        // ...

        public TextUIElement() : this(string.Empty) { }

        public TextUIElement(string text)
        {
            this.font = FontAssets.DefaultSmall;
            this.text = text;
            this.scale = 1f;

            FontColor = Color.White;
            ShadowSpread = 2f;

            RecreateText();
        }

        // ...

        protected override void DrawThis(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawStringWithShadow(font, $"{text}", Position, FontColor, 0, Vector2.Zero, FontScale, ShadowSpread);
        }

        private void RecreateText()
        {
            var vector = font.MeasureString(text);
            var vector2 = vector * FontScale;

            Width.SetPixel(vector2.X);
            Height.SetPixel(vector2.Y);

            Recalculate();
        }
    }
}