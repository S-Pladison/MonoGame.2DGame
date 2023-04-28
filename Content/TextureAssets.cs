using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Content
{
    public static class TextureAssets
    {
        public static Texture2D Collision { get; private set; }
        public static Texture2D CollisionTilemap { get; private set; }
        public static Texture2D Cursor { get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Texture2D Player { get; private set; }
        public static Texture2D Tilemap { get; private set; }
        public static Texture2D Shadow { get; private set; }

        public static Texture2D Crate { get; private set; }

        public static Texture2D Cloud1 { get; private set; }
        public static Texture2D Cloud2 { get; private set; }
        public static Texture2D Cloud3 { get; private set; }
        public static Texture2D Cloud4 { get; private set; }
        public static Texture2D Cloud5 { get; private set; }
        public static Texture2D Cloud6 { get; private set; }

        public static Texture2D Spikes { get; private set; }

        public static Texture2D WallPalette { get; private set; }
        public static Texture2D TilePalette { get; private set; }

        public static Texture2D Normal { get; private set; }

        // ...

        public static Texture2D UIPanel { get; set; }

        public static class UI
        {
            public static Texture2D Switch { get; set; }
            public static Texture2D Switch2 { get; set; }
        }

        // ...

        public static void Load(ContentManager content)
        {
            Texture2D Load(string name) => content.Load<Texture2D>("Textures/" + name);

            Collision = Load("Collision");
            CollisionTilemap = Load("CollisionTilemap");
            Cursor = Load("Cursor");
            Pixel = Load("Pixel");
            Player = Load("Player");
            Tilemap = Load("Tilemap");
            Shadow = Load("Shadow");

            Crate = Load("Crate");

            Cloud1 = Load("Clouds/1");
            Cloud2 = Load("Clouds/2");
            Cloud3 = Load("Clouds/3");
            Cloud4 = Load("Clouds/4");
            Cloud5 = Load("Clouds/5");
            Cloud6 = Load("Clouds/6");

            Spikes = Load("Spikes");

            WallPalette = Load("WallPalette");
            TilePalette = Load("TilePalette");

            Normal = Load("Normal");

            UIPanel = Load("Panel");

            UI.Switch = Load("UI/SwitchUIElement");
            UI.Switch2 = Load("UI/Switch2UIElement");
        }

        public static void Unload()
        {
            Shadow = null;
            Tilemap = null;
            Player = null;
            Pixel = null;
            Cursor = null;
            CollisionTilemap = null;
            Collision = null;

            Crate = null;

            Cloud6 = null;
            Cloud5 = null;
            Cloud4 = null;
            Cloud3 = null;
            Cloud2 = null;
            Cloud1 = null;

            Spikes = null;

            TilePalette = null;

            Normal = null;

            UIPanel = null;

            UI.Switch = null;
            UI.Switch2 = null;
        }
    }
}
