using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pladi.Core.Tiles
{
    public class TileLayer
    {
        public TileMap Map { get; init; }
        public Tile[,] Tiles { get; init; }
        public TilePalette Palette { get; private set; }

        public int Width => Map.Width;
        public int Height => Map.Height;

        // ...

        public TileLayer(TileMap map)
        {
            Map = map;
            Tiles = new Tile[map.Width, map.Height];
        }

        // ...

        public void SetPalette(TilePalette palette)
            => Palette = palette;

        public int Draw(SpriteBatch spriteBatch, Color color, CameraComponent camera)
            => InnerDraw(t => true, spriteBatch, color, camera);

        public int DrawOnlyTypes(SpriteBatch spriteBatch, Color color, CameraComponent camera, IEnumerable<ushort> types)
            => InnerDraw(t => types.Contains(t.Type), spriteBatch, color, camera);

        public int DrawWithoutTypes(SpriteBatch spriteBatch, Color color, CameraComponent camera, IEnumerable<ushort> types)
            => InnerDraw(t => !types.Contains(t.Type), spriteBatch, color, camera);

        private int InnerDraw(Predicate<Tile> predicate, SpriteBatch spriteBatch, Color color, CameraComponent camera)
        {
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Matrix.CreateScale(Map.Scale) * camera.ViewMatrix);

            int counter = 0;

            GetTileCoordsIntersectsWithRect(camera.VisibleArea, out Point leftTop, out Point rightBottom);

            var tileWidth = Palette.TileWidth;
            var tileHeight = Palette.TileHeight;

            for (int i = leftTop.X; i <= rightBottom.X; i++)
            {
                for (int j = leftTop.Y; j <= rightBottom.Y; j++)
                {
                    ref var tile = ref Tiles[i, j];

                    if (!tile.HasTile || !predicate.Invoke(tile)) continue;

                    var position = new Vector2(i * tileWidth, j * tileHeight);
                    var rect = Palette.GetRectByType(tile.Type);

                    spriteBatch.Draw(Palette.Texture, position, rect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                    counter++;
                }
            }

            spriteBatch.End();

            return counter;
        }

        public void GetTileCoordsIntersectsWithRect(Rectangle rectangle, out Point leftTop, out Point rightBottom)
            => GetTileCoordsIntersectsWithRect(rectangle.ToRectangleF(), out leftTop, out rightBottom);

        public void GetTileCoordsIntersectsWithRect(RectangleF rectangleF, out Point leftTop, out Point rightBottom)
        {
            var tileWidth = Palette.TileWidth * Map.Scale;
            var tileHeight = Palette.TileHeight * Map.Scale;

            rectangleF.X /= tileWidth;
            rectangleF.Y /= tileHeight;
            rectangleF.Width /= tileWidth;
            rectangleF.Height /= tileHeight;

            var rectangle = rectangleF.ToRectangle();

            var leftTile = rectangle.X - 1;
            var rightTile = rectangle.X + rectangle.Width + 1;
            var topTile = rectangle.Y - 1;
            var bottomTile = rectangle.Y + rectangle.Height + 1;

            var xMax = Width - 1;
            var yMax = Height - 1;

            leftTile = Math.Clamp(leftTile, 0, xMax);
            rightTile = Math.Clamp(rightTile, 0, xMax);
            topTile = Math.Clamp(topTile, 0, yMax);
            bottomTile = Math.Clamp(bottomTile, 0, yMax);

            leftTop = new Point(leftTile, topTile);
            rightBottom = new Point(rightTile, bottomTile);
        }
    }
}