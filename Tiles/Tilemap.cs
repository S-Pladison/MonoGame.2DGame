using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Enitites;
using Pladi.Enums;
using Pladi.Utils;
using System;
using RectangleF = System.Drawing.RectangleF;

namespace Pladi.Tiles
{
    public class Tilemap
    {
        public Tile[,] Tiles => tiles;

        public Texture2D RenderedTexture => target;
        public int Width => x;
        public int Height => y;
        public float Scale => scale;
        public int RenderedTileCount { get; private set; }

        private float ScaledTileSizeX => tileSizeX * scale;
        private float ScaledTileSizeY => tileSizeY * scale;

        // ...

        private int x;
        private int y;
        private float scale;
        private Tile[,] tiles;

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

            RenderedTileCount = 0;

            var device = spriteBatch.GraphicsDevice;
            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            GetTilesCoordsIntersectsWithRect(camera.VisibleArea, out Point leftTop, out Point rightBottom);

            for (int i = leftTop.X; i <= rightBottom.X; i++)
            {
                for (int j = leftTop.Y; j <= rightBottom.Y; j++)
                {
                    var position = new Vector2(i * tileSizeX, j * tileSizeY);
                    var tile = tiles[i, j];
                    var rectangle = new Rectangle(tile.Type % textureFrameCountX * tileSizeX, tile.Type / textureFrameCountX * tileSizeY, tileSizeX, tileSizeY);

                    spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                    RenderedTileCount++;
                }
            }

            spriteBatch.End();
            device.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
            => spriteBatch.Draw(target, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

        public bool IsTileCollision(int positionX, int positionY, int width, int height)
            => IsTileCollision(new Rectangle(positionX, positionY, width, height));

        public bool IsTileCollision(Vector2 position, int width, int height)
            => IsTileCollision(new Rectangle((int)position.X, (int)position.Y, width, height));

        public bool IsTileCollision(Rectangle rectangle)
        {
            GetTilesCoordsIntersectsWithRect(rectangle, out Point leftTop, out Point rightBottom);

            for (int i = leftTop.X; i <= rightBottom.X; i++)
            {
                for (int j = leftTop.Y; j <= rightBottom.Y; j++)
                {
                    var tile = tiles[i, j];

                    if (!tile.IsCollidable) continue;

                    var tilePosition = new Vector2(i * ScaledTileSizeX, j * ScaledTileSizeY);

                    if (rectangle.X + rectangle.Width > tilePosition.X
                        && rectangle.X < tilePosition.X + ScaledTileSizeX
                        && rectangle.Y + rectangle.Height > tilePosition.Y
                        && rectangle.Y < tilePosition.Y + ScaledTileSizeY)
                        return true;
                }
            }

            return false;
        }

        public void TileCollisionWithEntity(Entity entity, out CollisionSides collisionFlags, float minIntersectionArea = 0.01f)
        {
            collisionFlags = CollisionSides.None;

            var entityRectangle = new RectangleF(entity.Position.X, entity.Position.Y, entity.Width, entity.Height);

            GetTilesCoordsIntersectsWithRect(entityRectangle, out Point leftTop, out Point rightBottom);

            for (int i = leftTop.X; i <= rightBottom.X; i++)
            {
                for (int j = leftTop.Y; j <= rightBottom.Y; j++)
                {
                    ref var tile = ref tiles[i, j];

                    if (!tile.IsCollidable) continue;

                    var tileRectangle = new RectangleF(i * ScaledTileSizeX, j * ScaledTileSizeY, ScaledTileSizeX, ScaledTileSizeY);
                    var intersection = RectangleF.Intersect(entityRectangle, tileRectangle);

                    if (intersection.Width * intersection.Height < minIntersectionArea) continue;

                    if (intersection.Width > intersection.Height)
                    {
                        // Top
                        if (entity.Velocity.Y < 0 && entity.Position.Y > tileRectangle.Y)
                        {
                            entity.Position.Y = tileRectangle.Y + tileRectangle.Height;
                            entity.Velocity.Y = 0;
                            collisionFlags |= CollisionSides.Top;
                        }
                        // Buttom
                        else if (entity.Velocity.Y > 0 && entity.Position.Y < tileRectangle.Y)
                        {
                            entity.Position.Y = tileRectangle.Y - entity.Hitbox.Height;
                            entity.Velocity.Y = 0;
                            collisionFlags |= CollisionSides.Buttom;
                        }
                    }
                    else
                    {
                        // Left
                        if (entity.Velocity.X < 0 && entity.Position.X > tileRectangle.X)
                        {
                            entity.Position.X = tileRectangle.X + tileRectangle.Width;
                            entity.Velocity.X = 0;
                            collisionFlags |= CollisionSides.Left;
                        }
                        // Right
                        else if (entity.Velocity.X > 0 && entity.Position.X < tileRectangle.X)
                        {
                            entity.Position.X = tileRectangle.X - entity.Hitbox.Width;
                            entity.Velocity.X = 0;
                            collisionFlags |= CollisionSides.Right;
                        }
                    }
                }
            }
        }

        private void GetTilesCoordsIntersectsWithRect(Rectangle rectangle, out Point leftTop, out Point rightBottom)
            => GetTilesCoordsIntersectsWithRect(rectangle.ToRectangleF(), out leftTop, out rightBottom);

        private void GetTilesCoordsIntersectsWithRect(RectangleF rectangleF, out Point leftTop, out Point rightBottom)
        {
            var leftTile = (int)(rectangleF.X / ScaledTileSizeX) - 1;
            var rightTile = (int)((rectangleF.X + rectangleF.Width) / ScaledTileSizeX) + 1;
            var topTile = (int)(rectangleF.Y / ScaledTileSizeY) - 1;
            var bottomTile = (int)((rectangleF.Y + rectangleF.Height) / ScaledTileSizeY) + 1;

            var xMax = x - 1;
            var yMax = y - 1;

            leftTile = Math.Clamp(leftTile, 0, xMax);
            rightTile = Math.Clamp(rightTile, 0, xMax);
            topTile = Math.Clamp(topTile, 0, yMax);
            bottomTile = Math.Clamp(bottomTile, 0, yMax);

            leftTop = new Point(leftTile, topTile);
            rightBottom = new Point(rightTile, bottomTile);
        }
    }
}