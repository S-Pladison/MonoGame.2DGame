using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;

namespace Pladi.Core.UI.Elements
{
    public class PanelUIElement : UIElement
    {
        protected Color backgroundColor;

        // ...

        public PanelUIElement()
        {
            backgroundColor = Color.White;
        }

        // ...

        public void SetBackgroundColor(Color color)
            => backgroundColor = color;

        protected override void DrawThis(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, HitboxRectangle.ToRectangle(), backgroundColor);
        }
    }
}