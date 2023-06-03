using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Content
{
    public static class TextureAssets
    {
        public static Texture2D Pixel { get; private set; }

        // ...

        public static Texture2D Cursor { get; private set; }
        public static Texture2D Logo { get; private set; }

        // ...

        public static Texture2D Collision { get; private set; }
        public static Texture2D CollisionTilemap { get; private set; }
        public static Texture2D Shadow { get; private set; }

        // ...

        public static Texture2D Player { get; private set; }
        public static Texture2D Crate { get; private set; }
        public static Texture2D Spikes { get; private set; }
        public static Texture2D Flagpole { get; private set; }
        public static Texture2D Flag { get; private set; }
        public static Texture2D PressurePlate { get; private set; }
        public static Texture2D DeadZone { get; private set; }

        // ...

        public static Texture2D WallPalette { get; private set; }
        public static Texture2D TilePalette { get; private set; }

        // ...

        public static Texture2D KeyboardButtons { get; private set; }

        // ...

        public static Texture2D UIPanel { get; set; }
        public static Texture2D UISwitch { get; set; }
        public static Texture2D UISwitch2 { get; set; }

        // ...

        public static void Load(ContentManager content)
        {
            Texture2D Load(string name) => content.Load<Texture2D>("Textures/" + name);

            Pixel = Load("Pixel");

            Cursor = Load("Cursor");
            Logo = Load("Logo");

            Collision = Load("Collision");
            CollisionTilemap = Load("CollisionTilemap");
            Shadow = Load("Shadow");

            Player = Load("Player");
            Crate = Load("Crate");
            Spikes = Load("Spikes");
            Flagpole = Load("Flagpole");
            Flag = Load("Flag");
            PressurePlate = Load("PressurePlate");
            DeadZone = Load("DeadZone");

            WallPalette = Load("WallPalette");
            TilePalette = Load("TilePalette");

            KeyboardButtons = Load("ButtonKey");

            UIPanel = Load("Panel");
            UISwitch = Load("UI/SwitchUIElement");
            UISwitch2 = Load("UI/Switch2UIElement");
        }

        public static void Unload()
        {
            Pixel = null;

            Cursor = null;
            Logo = null;

            Collision = null;
            CollisionTilemap = null;
            Shadow = null;

            Player = null;
            Crate = null;
            Spikes = null;
            Flagpole = null;
            Flag = null;
            PressurePlate = null;
            DeadZone = null;

            WallPalette = null;
            TilePalette = null;

            KeyboardButtons = null;

            UIPanel = null;
            UISwitch = null;
            UISwitch2 = null;
        }
    }
}