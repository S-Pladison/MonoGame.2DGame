using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pladi.Utilities
{
    public static partial class PladiUtils
    {
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, RectangleF destinationRectangle, Color color)
            => spriteBatch.Draw(texture, destinationRectangle.ToRectangle(), color);

        public static void DrawStringWithShadow(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float spread = 2f)
        {
            spriteBatch.DrawStringShadow(font, text, position, Color.Black, rotation, origin, scale, spread);
            spriteBatch.DrawString(font, text, position, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawStringShadow(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float spread = 2f)
        {
            for (int i = 0; i < shadowDirections.Length; i++)
            {
                spriteBatch.DrawString(font, text, position + shadowDirections[i] * spread, color, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            spriteBatch.Draw(TextureAssets.Pixel, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(TextureAssets.Pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(TextureAssets.Pixel, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(TextureAssets.Pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        // ...

        private static readonly Vector2[] shadowDirections = new Vector2[] { Vector2.UnitX, Vector2.UnitY, -Vector2.UnitX, -Vector2.UnitY };
    }
}