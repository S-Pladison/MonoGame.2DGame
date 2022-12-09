using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities;

namespace Pladi.Core.UI.Elements
{
    public class PanelUIElement : UIElement
    {
        private Color backgroundColor;

        // ...

        public PanelUIElement(Color backgroundColor)
        {
            this.backgroundColor = backgroundColor;
        }

        // ...

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, BoundingRectangle.ToRectangle(), backgroundColor);
        }
    }
}