using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pladi.Core.Tiles
{
    public class TilePalette
    {
        public readonly Texture2D Texture;
        public readonly int CountX;
        public readonly int CountY;
        public readonly int TileWidth;
        public readonly int TileHeight;
        public readonly int Count;

        private readonly int tileWidthPlusTwo;
        private readonly int tileHeightPlusTwo;

        // ...

        public TilePalette(Texture2D texture, int countX, int countY)
        {
            Texture = texture;

            CountX = countX;
            CountY = countY;

            TileWidth = (tileWidthPlusTwo = texture.Width / countX) - 2;
            TileHeight = (tileHeightPlusTwo = texture.Height / countY) - 2;

            Count = countX * countY;
        }

        // ...

        public Rectangle GetRectByType(int type)
            => new(1 + type % CountX * tileWidthPlusTwo, 1 + type / CountX * tileHeightPlusTwo, TileWidth, TileHeight);
    }
}