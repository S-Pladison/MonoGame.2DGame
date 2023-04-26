using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities;

namespace Pladi.Core.UI.Elements
{
    public class PanelUIElement : UIElement
    {
        public enum PanelStyles
        {
            Standard,
            WithoutTexture
        }

        // ...

        // ...

        public Color BackgroundColor { get; set; }

        private Texture2D texture;

        // ...

        public PanelUIElement() : this(Color.White) { }

        public PanelUIElement(Color backgroundColor, PanelStyles style = PanelStyles.Standard)
        {
            BackgroundColor = backgroundColor;

            texture = GetTextureByStyle(style);
        }

        // ...

        protected override void DrawThis(SpriteBatch spriteBatch)
        {
            var textureRect = texture.Bounds;
            var width = textureRect.Width / 3;
            var height = textureRect.Height / 3;
            var panelRect = Dimensions.ToRectangle();
            var wx = panelRect.Width - width * 2;
            var wy = panelRect.Height - height * 2;

            // Left Top
            var sourceRect = new Rectangle(0, 0, width, height);
            var distRect = new Rectangle(panelRect.X, panelRect.Y, width, height);
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Top
            sourceRect.X += width;
            distRect.X += width;
            distRect.Width = wx;
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Right Top
            sourceRect.X += width;
            distRect.X += wx;
            distRect.Width = width;
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Left Center
            sourceRect = new Rectangle(0, height, width, height);
            distRect = new Rectangle(panelRect.X, panelRect.Y + height, width, wy);
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Center
            sourceRect.X += width;
            distRect.X += width;
            distRect.Width = wx;
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Right Center
            sourceRect.X += width;
            distRect.X += wx;
            distRect.Width = width;
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Left Bottom
            sourceRect = new Rectangle(0, height * 2, width, height);
            distRect = new Rectangle(panelRect.X, panelRect.Y + wy + height, width, height);
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Bottom
            sourceRect.X += width;
            distRect.X += width;
            distRect.Width = wx;
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Right Bottom
            sourceRect.X += width;
            distRect.X += wx;
            distRect.Width = width;
            spriteBatch.Draw(texture, distRect, sourceRect, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }

        // ...

        private Texture2D GetTextureByStyle(PanelStyles style)
        {
            return style switch
            {
                PanelStyles.WithoutTexture => TextureAssets.Pixel,
                _ => TextureAssets.UIPanel
            };
        }
    }
}