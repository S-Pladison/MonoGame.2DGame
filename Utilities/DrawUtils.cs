﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities.DataStructures;

namespace Pladi.Utilities
{
    public static class DrawUtils
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

        public static void DrawGrid(this SpriteBatch spriteBatch, Rectangle bounds, Color color, int cellSizeX, int cellSizeY, int lineWidth)
        {
            var offset = new Point(bounds.X / cellSizeX, bounds.Y / cellSizeY);
            var xMax = bounds.Width / cellSizeX + offset.X;
            var yMax = bounds.Height / cellSizeY + offset.Y;
            var halfWidth = lineWidth / 2;

            for (float x = offset.X; x <= xMax; x++)
            {
                var position = new Point((int)(x * cellSizeX) - halfWidth, bounds.Y);
                var rectangle = new Rectangle(position.X, position.Y, lineWidth, bounds.Height);
                spriteBatch.Draw(TextureAssets.Pixel, rectangle, color);
            }

            for (float y = offset.Y; y <= yMax; y++)
            {
                var position = new Point(bounds.X, (int)(y * cellSizeY) - halfWidth);
                var rectangle = new Rectangle(position.X, position.Y, bounds.Width, lineWidth);
                spriteBatch.Draw(TextureAssets.Pixel, rectangle, color);
            }
        }

        // [...]

        private static readonly Vector2[] shadowDirections = new Vector2[] { Vector2.UnitX, Vector2.UnitY, -Vector2.UnitX, -Vector2.UnitY };
    }
}