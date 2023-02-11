﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Entities
{
    public class Box : Entity
    {
        public Color Color { get; set; } = Color.White;

        // ...

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.Pixel, Hitbox, Color);
        }
    }
}