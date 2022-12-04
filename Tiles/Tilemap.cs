using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Enitites;
using System;

namespace Pladi.Tiles
{
    public class Tilemap
    {
        public Texture2D RenderedTexture => target;
        public int Width => x;
        public int Height => y;

        // ...

        private readonly int x;
        private readonly int y;
        private readonly float scale;
        private readonly Tile[,] tiles;

        private RenderTarget2D target;
        private Texture2D texture;

        private int textureFrameCountX;
        private int textureFrameCountY;
        private int tileSizeX;
        private int tileSizeY;

        // ...

        public Tilemap(int x, int y, float scale = 1f)
        {
            this.x = x;
            this.y = y;
            this.scale = scale;

            tiles = new Tile[x, y];
        }

        public void RecreateRenderTarget(GraphicsDevice device, int Width, int Height)
        {
            target = new RenderTarget2D(device, Width, Height);
        }

        public void SetTexture(Texture2D texture, int frameX, int frameY)
        {
            this.texture = texture;

            textureFrameCountX = frameX;
            textureFrameCountY = frameY;

            tileSizeX = texture.Width / textureFrameCountX;
            tileSizeY = texture.Height / textureFrameCountY;

            RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, Main.SpriteBatch.GraphicsDevice.Viewport.Width, Main.SpriteBatch.GraphicsDevice.Viewport.Height);
        }

        public void SetTile(int x, int y, Tile tile)
        {
            tiles[x, y] = tile;
        }

        public void Render(SpriteBatch spriteBatch, Camera camera)
        {
            if (texture is null || target is null) return;

            var device = spriteBatch.GraphicsDevice;

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);

            var cameraBounds = camera.Viewport.Bounds;
            var x4TileSizeX = tileSizeX * 4;
            var x4TileSizeY = tileSizeY * 4;
            var x8TileSizeX = tileSizeX * 8;
            var x8TileSizeY = tileSizeY * 8;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    var pos = new Vector2(i * tileSizeX, j * tileSizeY) * scale;
                    var screenRect = new Rectangle((int)(pos.X - camera.Position.X + x4TileSizeX), (int)(pos.Y - camera.Position.Y + x4TileSizeY), tileSizeX - x8TileSizeX, tileSizeY - x8TileSizeY);

                    if (!cameraBounds.Contains(screenRect)) continue;

                    var tile = tiles[i, j];
                    var rect = new Rectangle(tile.Type % textureFrameCountX * tileSizeX, tile.Type / textureFrameCountX * tileSizeY, tileSizeX, tileSizeY);

                    spriteBatch.Draw(texture, pos, rect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                }
            }

            spriteBatch.End();
            device.SetRenderTarget(null);
        }

        public bool IsTileCollision(Vector2 position, int width, int height)
        {
            var scaledTileSizeX = scale * tileSizeX;
            var scaledTileSizeY = scale * tileSizeY;

            var leftTile = (int)(position.X / scaledTileSizeX);
            var rightTile = (int)((position.X + width) / scaledTileSizeX) + 1;
            var topTile = (int)(position.Y / scaledTileSizeY);
            var bottomTile = (int)((position.Y + height) / scaledTileSizeY) + 1;

            leftTile = Math.Clamp(leftTile, 0, x - 1);
            rightTile = Math.Clamp(rightTile, 0, x - 1);
            topTile = Math.Clamp(topTile, 0, y - 1);
            bottomTile = Math.Clamp(bottomTile, 0, y - 1);

            for (int i = leftTile; i < rightTile; i++)
            {
                for (int j = topTile; j < bottomTile; j++)
                {
                    var tile = tiles[i, j];

                    if (tile.IsAir) continue;

                    var vector = new Vector2(i * scaledTileSizeX, j * scaledTileSizeY);

                    if (position.X + width > vector.X
                        && position.X < vector.X + scaledTileSizeX
                        && position.Y + height > vector.Y
                        && position.Y < vector.Y + scaledTileSizeY)
                        return true;
                }
            }

            return false;
        }

        public void TileCollisionWithEntity(Entity entity, out bool onFloor, int minIntersArea = 0, float offsetY = 1f)
        {
            onFloor = false;

            var position = entity.Position;
            var width = entity.Width;
            var height = entity.Height;
            var hitbox = entity.Hitbox;

            var scaledTileSizeX = scale * tileSizeX;
            var scaledTileSizeY = scale * tileSizeY;

            var leftTile = (int)(position.X / scaledTileSizeX) - 1;
            var rightTile = (int)((position.X + width) / scaledTileSizeX) + 1;
            var topTile = (int)(position.Y / scaledTileSizeY) - 1;
            var bottomTile = (int)((position.Y + height) / scaledTileSizeY) + 1;

            leftTile = Math.Clamp(leftTile, 0, x - 1);
            rightTile = Math.Clamp(rightTile, 0, x - 1);
            topTile = Math.Clamp(topTile, 0, y - 1);
            bottomTile = Math.Clamp(bottomTile, 0, y - 1);

            for (int i = leftTile; i < rightTile; i++)
            {
                for (int j = topTile; j < bottomTile; j++)
                {
                    ref var tile = ref tiles[i, j];

                    if (tile.IsAir) continue;

                    var tileRectangle = new Rectangle(i * (int)scaledTileSizeX, j * (int)scaledTileSizeY, (int)scaledTileSizeX, (int)scaledTileSizeY);
                    var intersection = Rectangle.Intersect(hitbox, tileRectangle);

                    if (intersection.Width * intersection.Height < minIntersArea) continue;

                    if (intersection.Width < intersection.Height)
                    {
                        if (entity.Position.X > tileRectangle.X)
                        {
                            // Ограничение слева
                            entity.Position.X = tileRectangle.X + tileRectangle.Width;
                        }
                        else
                        {
                            // Ограничение справа
                            entity.Position.X = tileRectangle.X - hitbox.Width;
                        }

                        entity.Velocity.X = 0;
                    }
                    else
                    {
                        if (entity.Position.Y > tileRectangle.Y)
                        {
                            // Ограничение свехру
                            entity.Position.Y = tileRectangle.Y + tileRectangle.Height;
                        }
                        else
                        {
                            // Ограничение под
                            entity.Position.Y = tileRectangle.Y - hitbox.Height + offsetY;
                            onFloor = true;
                        }

                        entity.Velocity.Y = 0;
                    }
                }
            }
        }
    }
}