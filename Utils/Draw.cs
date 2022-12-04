using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Utils
{
    public static partial class PladiUtils
    {
        public static void DrawStringWithShadow(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float spread = 2f)
        {
            spriteBatch.DrawStringShadow(font, text, position, Color.Black, rotation, origin, scale, spread);
            spriteBatch.DrawString(font, text, position, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawStringShadow(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float spread = 2f)
        {
            for (int i = 0; i < ShadowDirections.Length; i++)
            {
                spriteBatch.DrawString(font, text, position + ShadowDirections[i] * spread, color, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        // ...

        public static readonly Vector2[] ShadowDirections = new Vector2[4] { -Vector2.UnitX, Vector2.UnitX, -Vector2.UnitY, Vector2.UnitY };
    }
}