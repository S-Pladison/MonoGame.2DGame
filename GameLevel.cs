using Microsoft.Xna.Framework.Graphics;
using Pladi.Tiles;
using System.IO;

namespace Pladi
{
    public class GameLevel
    {
        public Tilemap BackTilemap { get; private set; }
        public Tilemap CollisionTilemap { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float TileScale { get; private set; }

        // ...

        public GameLevel(int width, int height, float tileScale = 1f)
        {
            BackTilemap = new Tilemap(width, height, tileScale);
            CollisionTilemap = new Tilemap(width, height, tileScale);

            Width = width;
            Height = height;
            TileScale = tileScale;
        }

        // ...

        public void RecreateRenderTargets(GraphicsDevice device, int width, int height)
        {
            BackTilemap.RecreateRenderTarget(device, width, height);
            CollisionTilemap.RecreateRenderTarget(device, width, height);
        }

        public void Render(SpriteBatch spriteBatch, Camera camera)
        {
            BackTilemap.Render(spriteBatch, camera);
            CollisionTilemap.Render(spriteBatch, camera);
        }

        public void SaveToFile(string path)
        {
            using var writer = new BinaryWriter(File.Create(path));

            writer.Write(Width);
            writer.Write(Height);
            writer.Write(TileScale);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    writer.Write(BackTilemap.Tiles[i, j].Type);
                    writer.Write(CollisionTilemap.Tiles[i, j].Type);
                }
            }
        }

        // ...

        public static GameLevel LoadFromFile(string path)
        {
            using var reader = new BinaryReader(File.Open(path, FileMode.Open));

            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var tileScale = reader.ReadSingle();

            var level = new GameLevel(width, height, tileScale);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    level.BackTilemap.Tiles[i, j].Type = reader.ReadUInt16();
                    level.CollisionTilemap.Tiles[i, j].Type = reader.ReadUInt16();
                }
            }

            return level;
        }
    }
}