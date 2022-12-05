using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Content
{
    public static class FontAssets
    {
        public static SpriteFont DefaultSmall { get; private set; }
        public static SpriteFont DefaultMedium { get; private set; }

        // ...

        public static void Load(ContentManager content)
        {
            DefaultSmall = content.Load<SpriteFont>("Fonts/Main");
            DefaultMedium = content.Load<SpriteFont>("Fonts/MainMedium");
        }

        public static void Unload()
        {
            DefaultSmall = null;
            DefaultMedium = null;
        }
    }
}
