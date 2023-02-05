using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities;

namespace Pladi.Core.UI.Elements
{
    public class PanelUIElement : UIElement
    {
        public Color BackgroundColor { get; set; }

        // ...

        public PanelUIElement() : this(Color.White) { }

        public PanelUIElement(Color backgroundColor)
        {
            BackgroundColor = backgroundColor;
        }

        // ...

        protected override void DrawThis(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, Dimensions.ToRectangle(), BackgroundColor);
        }
    }
}