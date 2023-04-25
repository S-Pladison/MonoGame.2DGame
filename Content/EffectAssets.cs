using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Content
{
    public static class EffectAssets
    {
        public static Effect Background { get; private set; }
        public static Effect Collision { get; private set; }
        public static Effect TileEdgeShadow { get; private set; }
        public static Effect Light { get; private set; }
        public static Effect Shadow { get; private set; }

        public static Effect He { get; private set; }
        public static Effect Tile { get; private set; }

        // ...

        public static void Load(ContentManager content)
        {
            Background = content.Load<Effect>("Effects/Background");
            Collision = content.Load<Effect>("Effects/Collision");
            TileEdgeShadow = content.Load<Effect>("Effects/TileEdgeShadow");
            Light = content.Load<Effect>("Effects/Light");
            Shadow = content.Load<Effect>("Effects/Shadow");

            He = content.Load<Effect>("Effects/He");
            Tile = content.Load<Effect>("Effects/Tile");
        }

        public static void Unload()
        {
            Tile = null;
            He = null;

            Shadow = null;
            Light = null;
            TileEdgeShadow = null;
            Collision = null;
            Background = null;
        }
    }
}
