using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pladi.Content
{
    public static class EffectAssets
    {
        public static Effect Background { get; private set; }

        // ...

        public static void Load(ContentManager content)
        {
            Background = content.Load<Effect>("Effects/Background");
        }

        public static void Unload()
        {
            Background = null;
        }
    }
}
