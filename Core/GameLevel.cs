using Microsoft.Xna.Framework;
using Pladi.Core.Entities;
using Pladi.Core.Graphics.Lighting;
using Pladi.Core.Tiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pladi.Core
{
    public class GameLevel
    {
        // [public properties and fields]

        public int Width { get; init; }
        public int Height { get; init; }
        public TileMap TileMap { get; init; }
        public List<Light> Lights { get; init; }
        public List<Tuple<EntityTypes, Vector2>> Entities { get; init; }

        // [constructors]

        public GameLevel(int width, int height, int tileScale)
        {
            Width = width;
            Height = height;
            TileMap = new TileMap(width, height, tileScale);
            Lights = new List<Light>();
            Entities = new List<Tuple<EntityTypes, Vector2>>();
        }

        // [public static methods]

        public static bool TryLoadLevelFromFile(string name, out GameLevel gameLevel)
        {
            CreateMapDirectoryIfDontExists();

            var path = $"Maps/{name}.pl";

            if (!File.Exists(path))
            {
                gameLevel = null;
                return false;
            }

            using var reader = new BinaryReader(File.Open(path, FileMode.Open));

            try
            {
                var scale = reader.ReadInt32();
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();

                gameLevel = new GameLevel(width, height, scale);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        gameLevel.TileMap.WallLayer.Tiles[i, j].Type = reader.ReadUInt16();
                        gameLevel.TileMap.TileLayer.Tiles[i, j].Type = reader.ReadUInt16();
                    }
                }

                var lightCount = reader.ReadInt32();

                for (int i = 0; i < lightCount; i++)
                {
                    var lightPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    var lightRadius = reader.ReadSingle();
                    var lightColor = new Color(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    var light = new Light(lightColor, lightPosition, lightRadius);

                    gameLevel.Lights.Add(light);
                }

                var entityCount = reader.ReadInt32();

                for (int i = 0; i < entityCount; i++)
                {
                    var entityType = (EntityTypes)reader.ReadInt32();
                    var entityPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                    gameLevel.Entities.Add(new(entityType, entityPosition));
                }

                return true;
            }
            catch
            {
                reader?.Close();
                gameLevel = null;
            }

            return false;
        }

        public static bool LevelFileExists(string name)
        {
            var path = $"Maps/{name}.pl";
            return File.Exists(path);
        }

        // [private static methods]

        private static void CreateMapDirectoryIfDontExists()
        {
            if (Directory.Exists("Maps")) return;

            Directory.CreateDirectory("Maps");
        }

        // [public methods]

        public void SaveToFile(string name)
        {
            CreateMapDirectoryIfDontExists();

            var path = $"Maps/{name}.pl";

            using var writer = new BinaryWriter(File.Create(path));

            writer.Write(TileMap.Scale);
            writer.Write(TileMap.Width);
            writer.Write(TileMap.Height);

            for (int i = 0; i < TileMap.Width; i++)
            {
                for (int j = 0; j < TileMap.Height; j++)
                {
                    writer.Write(TileMap.WallLayer.Tiles[i, j].Type);
                    writer.Write(TileMap.TileLayer.Tiles[i, j].Type);
                }
            }

            writer.Write(Lights.Count);

            for (int i = 0; i < Lights.Count; i++)
            {
                writer.Write(Lights[i].Position.X);
                writer.Write(Lights[i].Position.Y);
                writer.Write(Lights[i].Radius);
                writer.Write((int)Lights[i].Color.R);
                writer.Write((int)Lights[i].Color.G);
                writer.Write((int)Lights[i].Color.B);
                writer.Write((int)Lights[i].Color.A);
            }

            writer.Write(Entities.Count);

            for (int i = 0; i < Entities.Count; i++)
            {
                writer.Write((int)Entities[i].Item1);
                writer.Write(Entities[i].Item2.X);
                writer.Write(Entities[i].Item2.Y);
            }
        }
    }
}