﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Utilities;

namespace Pladi.UI
{
    public class TextUIElement : UIElement
    {
        private readonly SpriteFont font;

        private string text;
        private float scale;
        private float spread;

        private Vector2 textOrigin;
        private Vector2 textSize;
        private Color color;

        // ...

        public TextUIElement(SpriteFont font, string text, Color textColor, float scale = 1f, float spread = 2f)
        {
            this.font = font;
            this.color = textColor;
            this.textOrigin = new Vector2(0, 0.5f);

            SetText(text, scale, spread);
        }

        // ...

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawStringWithShadow(font, $"{text}", Position, color, 0, textOrigin, scale, spread);
        }

        // ...

        public void SetText(string text, float scale = 1f, float spread = 2f)
        {
            this.text = text;
            this.scale = scale;
            this.spread = spread;

            var vector = font.MeasureString(text);
            var vector2 = textSize = vector * scale;

            Width = (int)vector2.X;
            Height = (int)vector2.Y;
        }

        public void SetColor(Color color)
            => this.color = color;
    }
}