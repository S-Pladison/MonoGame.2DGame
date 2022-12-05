using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Content
{
    public static class TextureAssets
    {
        public static Texture2D Cursor { get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Texture2D Player { get; private set; }
        public static Texture2D Tilemap { get; private set; }

        // ...

        public static void Load(ContentManager content)
        {
            Texture2D Load(string name) => content.Load<Texture2D>("Textures/" + name);

            Cursor = Load("Cursor");
            Pixel = Load("Pixel");
            Player = Load("Player");
            Tilemap = Load("Tilemap");
        }

        public static void Unload()
        {
            Tilemap = null;
            Player = null;
            Pixel = null;
            Cursor = null;
        }
    }
}
