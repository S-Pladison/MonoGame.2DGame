using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Content
{
    public static class AudioAssets
    {
        public static Song Default { get; private set; }

        // ...

        public static void Load(ContentManager content)
        {
            Default = content.Load<Song>("Audio/Default");
        }

        public static void Unload()
        {
            Default = null;
        }
    }
}
