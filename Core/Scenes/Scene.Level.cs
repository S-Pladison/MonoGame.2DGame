using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pladi.Content;
using Pladi.Core.Collisions;
using Pladi.Core.Entities;
using Pladi.Core.Graphics;
using Pladi.Core.Graphics.Lighting;
using Pladi.Core.Graphics.Renderers;
using Pladi.Core.Input;
using Pladi.Core.Tiles;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pladi.Core.Scenes
{
    public class LevelScene : Scene
    {
        private GraphicalUI graphicalUI;

        private List<Entity> entities;
        private SpatialHash<Entity> entitySpatialHash;

        private LevelCollision levelCollision;

        private List<Light> lights;

        private PlayerEntity player;

        private TileMap tileMap;

        private CameraComponent camera;
        private Vector2 cameraSmoothVelocity;

        private LightRenderer lightRendered;
        private WallRenderer wallRenderer;
        private TileRenderer tileRenderer;
        private EntityRenderer entityRenderer;

        private bool shouldDrawHitboxes;

        // ...

        public LevelScene()
        {
            graphicalUI = new();

            entities = new();
            entitySpatialHash = new(48 * 5, 48 * 5);

            lights = new();

            InitTileMap();
            InitPlayer();
            InitEntities();
            InitUI();

            levelCollision = new(tileMap.TileLayer, entitySpatialHash);
        }

        // ...

        public override void OnActivate()
        {
            Main.Instance.OnPreDraw += RequestLightRenderTarget;

            /*MediaPlayer.Play(AudioAssets.Default);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.75f;*/

            camera = ILoadable.GetInstance<CameraComponent>();
            lightRendered = ILoadable.GetInstance<LightRenderer>();
            wallRenderer = ILoadable.GetInstance<WallRenderer>();
            tileRenderer = ILoadable.GetInstance<TileRenderer>();
            entityRenderer = ILoadable.GetInstance<EntityRenderer>();

            PrepareLightRenderer();

            var e = new ZoneTrigger() { Height = 48 * 3, Position = new Vector2(48 * 15, 48 * 8) };
            e.OnTriggerEnter += (other) =>
            {
                lights.Remove(lights.Last());
                lights.Add(new Light(Rainbow(Main.Rand.NextFloat(0, 1)) * 0.3f, new Vector2(400, 100), 48 * 7));
            };

            entities.Add(e);
        }

        public override void OnDeactivate()
        {
            lightRendered.PrepareStaticShadowVertices(null);
            camera.ResetPosition();

            MediaPlayer.Stop();

            Main.Instance.OnPreDraw -= RequestLightRenderTarget;
        }

        private void PrepareLightRenderer()
        {
            var rectangle = new RectangleF(0, 0, tileMap.Width * tileMap.TileLayer.Palette.TileWidth, tileMap.Height * tileMap.TileLayer.Palette.TileHeight);
            var edges = tileMap.TileLayer.GetEdgesFromTiles(rectangle);

            lightRendered.PrepareStaticShadowVertices(edges);
            lightRendered.PrepareDynamicBuffers(1024);
        }

        private void InitTileMap()
        {
            tileMap = new TileMap(300, 100, 3);

            tileMap.WallLayer.SetPalette(new TilePalette(TextureAssets.WallPalette, 5, 1));
            tileMap.TileLayer.SetPalette(new TilePalette(TextureAssets.TilePalette, 3, 1));

            for (int i = 0; i < 300; i++)
            {
                tileMap.TileLayer.Tiles[i, 5 + 0].Type = 1;

                for (int j = 0; j < 10; j++)
                {
                    tileMap.TileLayer.Tiles[i, 6 + 5 + j].Type = 1;
                }
            }

            void Aa(int j, byte type = 1)
            {
                tileMap.WallLayer.Tiles[j, 5 + 1].Type = type;
                tileMap.WallLayer.Tiles[j, 5 + 2].Type = type;
                tileMap.WallLayer.Tiles[j, 5 + 3].Type = type;
                tileMap.WallLayer.Tiles[j, 5 + 4].Type = type;
                tileMap.WallLayer.Tiles[j, 5 + 5].Type = type;
            }

            tileMap.TileLayer.Tiles[0, 5 + 4].Type = 1;
            tileMap.TileLayer.Tiles[0, 5 + 5].Type = 1;

            tileMap.TileLayer.Tiles[5, 5 + 5].Type = 1;
            tileMap.TileLayer.Tiles[6, 5 + 5].Type = 1;
            tileMap.TileLayer.Tiles[7, 5 + 5].Type = 1;
            tileMap.TileLayer.Tiles[8, 5 + 5].Type = 1;
            tileMap.TileLayer.Tiles[9, 5 + 5].Type = 1;

            Aa(2);
            Aa(3);

            Aa(7, 3);
            Aa(8, 2);
            Aa(9, 2);
            Aa(10, 2);
            Aa(11, 2);
            Aa(12, 4);

            Aa(15);
            Aa(16);

            tileMap.TileLayer.Tiles[5, 5 + 5].Type = 2;
            tileMap.TileLayer.Tiles[6, 5 + 5].Type = 2;
            tileMap.TileLayer.Tiles[7, 5 + 5].Type = 2;
            tileMap.TileLayer.Tiles[8, 5 + 5].Type = 2;
            tileMap.TileLayer.Tiles[9, 5 + 5].Type = 2;

            tileMap.TileLayer.Tiles[11, 5 + 5].Type = 2;

            tileMap.TileLayer.Tiles[12, 5 + 1].Type = 1;
            tileMap.TileLayer.Tiles[13, 5 + 1].Type = 1;
            tileMap.TileLayer.Tiles[14, 5 + 1].Type = 1;
            tileMap.TileLayer.Tiles[15, 5 + 1].Type = 1;
            tileMap.TileLayer.Tiles[16, 5 + 1].Type = 1;
            tileMap.TileLayer.Tiles[17, 5 + 1].Type = 1;

            tileMap.TileLayer.Tiles[13, 5 + 2].Type = 1;
            tileMap.TileLayer.Tiles[14, 5 + 2].Type = 1;
            tileMap.TileLayer.Tiles[15, 5 + 2].Type = 1;
            tileMap.TileLayer.Tiles[16, 5 + 2].Type = 1;
            tileMap.TileLayer.Tiles[17, 5 + 2].Type = 1;
        }

        private void InitPlayer()
        {
            player = new()
            {
                Position = new Vector2(50, 48 * 6)
            };
        }

        public static Color Rainbow(float progress)
        {
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;

            return (int)div switch
            {
                0 => new(255, ascending, 0),
                1 => new(descending, 255, 0),
                2 => new(0, 255, ascending),
                3 => new(0, descending, 255),
                4 => new(ascending, 0, 255),
                // case 5:
                _ => new(255, 0, descending),
            };
        }

        private void InitEntities()
        {
            entities.Add(new CrateEntity() { Position = new Vector2(48 * 8, 48 * 6) });


            entities.Add(player);

            //lights.Add(new Light(Color.Red * 0.3f, new Vector2(48 * 3, 48 * 7), 48 * 13));
            //lights.Add(new Light(Color.Yellow * 0.3f, new Vector2(48 * 21, 48 * 7), 48 * 12));

            for (int i = 0; i < 100; i++)
            {
                lights.Add(new Light(Color.Red * 0.3f, new Vector2(400, 100), 48 * 7));
            }

            lights.Add(new Light(Color.Red * 0.3f, new Vector2(400, 100), 48 * 7));

            foreach (var entity in entities)
            {
                entity.OnChangePosition += () => entitySpatialHash.Update(entity.Position, entity);
                entitySpatialHash.Insert(entity.Position, entity);
            }
        }

        private void InitUI()
        {
            var panel = new PanelUIElement();
            panel.Width.SetPixel(230f);
            panel.Height.SetPixel(84f);
            panel.Left.SetPixel(10f);
            panel.Top.SetPixel(10f);
            graphicalUI.Append(panel);

            var text = new TextUIElement();
            text.OnPostUpdate += (elem) => (elem as TextUIElement).Text = $"" +
                $"FPS: {(int)ILoadable.GetInstance<FrameCounterComponent>().AverageFramesPerSecond}\n" +
                $"Entities: {entities.Count}\n" +
                $"Lights: {lights.Count}\n" +
                $"Location: [{Math.Round(player.Position.X, 2)}; {Math.Round(player.Position.Y, 2)}]";
            text.Left.SetPixel(10f);
            text.Top.SetPixel(10f);
            panel.Append(text);
        }

        public override void Update()
        {
            if (ILoadable.GetInstance<InputComponent>().JustPressed(Keys.NumPad5))
                shouldDrawHitboxes = !shouldDrawHitboxes;

            if (ILoadable.GetInstance<InputComponent>().JustPressed(Keys.Escape))
                SceneComponent.SetActiveScene(SceneComponent.GameScenes.Menu);

            UpdateEntities();

            graphicalUI.Update();

            List<EdgeF> edges = new List<EdgeF>();

            for (int i = 0; i < entities.Count; i++)
            {
                var e = entities[i].ShadowCastEdges;

                if (e is null) continue;

                edges.AddRange(e);
            }

            lightRendered.Seeee(edges);

            var camera = ILoadable.GetInstance<CameraComponent>();
            camera.Position = PladiUtils.SmoothDamp(camera.Position, player.Hitbox.Center, ref cameraSmoothVelocity, 0.05f, Main.DeltaTime);
        }

        private void UpdateEntities()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Update(levelCollision);
            }

            //lights.Last().Position = Vector2.Transform(ILoadable.GetInstance<InputComponent>().MousePosition, Matrix.Invert(ILoadable.GetInstance<CameraComponent>().ViewMatrix));
            lights.Last().Position = player.Hitbox.Center;
        }

        public void RequestLightRenderTarget(GameTime _)
        {
            lightRendered.Request((lights, tileMap.TileLayer));
            wallRenderer.Request(tileMap.WallLayer);
            tileRenderer.Request(tileMap.TileLayer);
            entityRenderer.Request(entities);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!lightRendered.TryGetTargetIfPrepared(out var lightTarget) ||
                !wallRenderer.TryGetTargetIfPrepared(out var wallTarget) ||
                !tileRenderer.TryGetTargetIfPrepared(out var tileTarget) ||
                !entityRenderer.TryGetTargetIfPrepared(out var entityTarget))
                throw new Exception("Одна из целей рендеринга не была подготовлена...");

            PrepareEffects();

            spriteBatch.GraphicsDevice.Clear(Colors.Main * 0.75f);

            Draw_Walls(spriteBatch);
            Draw_Entities(spriteBatch);
            Draw_Tiles(spriteBatch);
            Draw_Hitboxes(spriteBatch);
            Draw_UI(spriteBatch);
        }

        private void PrepareEffects()
        {
            var effect = EffectAssets.TileEdgeShadow;
            effect.Parameters["TransformMatrix"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["Texture0Size"].SetValue(new Vector2(Main.ScreenSize.X, Main.ScreenSize.Y));

            if (!lightRendered.TryGetTargetIfPrepared(out var lightTarget))
                throw new Exception("Цель рендеринга освещения не была подготовлена...");

            effect = EffectAssets.He;
            effect.Parameters["TransformMatrix"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["Texture1"].SetValue(lightTarget);

            effect = EffectAssets.Tile;
            effect.Parameters["TransformMatrix"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["Texture1"].SetValue(lightTarget);
            effect.Parameters["Resolution"].SetValue(Main.ScreenSize.ToVector2());
        }

        private void Draw_OutlineEffect(SpriteBatch spriteBatch, RenderTarget2D target)
        {
            var effect = EffectAssets.TileEdgeShadow;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, effect, Matrix.Identity);
            spriteBatch.Draw(target, Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private void Draw_Walls(SpriteBatch spriteBatch)
        {
            if (!wallRenderer.TryGetTargetIfPrepared(out var wallTarget))
                throw new Exception("Цель рендеринга стен не была подготовлена...");

            Draw_OutlineEffect(spriteBatch, wallTarget);

            var effect = EffectAssets.He;
            effect.Parameters["Power"].SetValue(1f);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, effect, Matrix.Identity);
            spriteBatch.Draw(wallTarget, Vector2.Zero, null, Color.LightGray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }


        private void Draw_Entities(SpriteBatch spriteBatch)
        {
            if (!entityRenderer.TryGetTargetIfPrepared(out var entityTarget))
                throw new Exception("Цель рендеринга тайлов не была подготовлена...");

            //Draw_OutlineEffect(spriteBatch, entityTarget);

            var effect = EffectAssets.He;
            effect.Parameters["Power"].SetValue(0.0f);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, effect, Matrix.Identity);
            spriteBatch.Draw(entityTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.ViewMatrix);
            player.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void Draw_Tiles(SpriteBatch spriteBatch)
        {
            if (!tileRenderer.TryGetTargetIfPrepared(out var tileTarget))
                throw new Exception("Цель рендеринга тайлов не была подготовлена...");

            Draw_OutlineEffect(spriteBatch, tileTarget);

            var effect = EffectAssets.Tile;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, effect, Matrix.Identity);
            spriteBatch.Draw(tileTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private void Draw_Hitboxes(SpriteBatch spriteBatch)
        {
            if (!shouldDrawHitboxes) return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.ViewMatrix);

            foreach (var e in Eee(tileMap.TileLayer))
            {
                spriteBatch.DrawRectangle(e.ToRectangle(), Color.Cyan * 0.35f, 1);
            }


            for (int i = 0; i < lights.Count; i++)
            {
                spriteBatch.DrawRectangle(lights[i].VisibleArea.ToRectangle(), Color.Yellow, 1);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                spriteBatch.DrawRectangle(entities[i].Hitbox.ToRectangle(), entities[i].IsTrigger ? Color.Orange : Color.Red, 1);
            }

            // Проверка коллизии
            {
                var mousePosition = ILoadable.GetInstance<InputComponent>().MousePosition;
                mousePosition = Vector2.Transform(mousePosition, Matrix.Invert(ILoadable.GetInstance<CameraComponent>().ViewMatrix));

                var checkRectangle = new RectangleF(mousePosition.X - 8, mousePosition.Y - 8, 16, 16);

                spriteBatch.DrawRectangle(checkRectangle.ToRectangle(), levelCollision.IsRectCollideWithEntities(checkRectangle) ? Color.Green : Color.Red, 1);
            }

            // ...

            //var t = new TilesFromStrings().Create(tileMap.TileLayer);


            // ...

            spriteBatch.End();
        }

        private void Draw_UI(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            graphicalUI.Draw(spriteBatch);
            spriteBatch.End();
        }

        private List<RectangleF> Eee(TileLayer layer)
        {
            var result = new List<RectangleF>();

            for (int x = 0; x < layer.Width; x++)
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    ref var tile = ref layer.Tiles[x, y];

                    if (!tile.HasTile) continue;

                    var tileHitbox = new RectangleF(x * layer.Palette.TileWidth * layer.Map.Scale, y * layer.Palette.TileHeight * layer.Map.Scale, layer.Palette.TileWidth * layer.Map.Scale, layer.Palette.TileHeight * layer.Map.Scale);

                    result.Add(tileHitbox);
                }
            }

            return result;
        }

        public class TilesFromStrings
        {
            private List<Rectangle> Result = new List<Rectangle>();

            public IEnumerable<Rectangle> Create(TileLayer tileLayer)
            {
                Result.Clear();

                CreateMergedHorizontalTiles(tileLayer);
                MergeVerticallyTilesWithSameWidth();

                return Result.Where(f => f.Height > 0);
            }

            private void MergeVerticallyTilesWithSameWidth()
            {
                /*foreach (var current in Result)
                {
                    foreach (var other in Result)
                    {
                        if (other.Y + other.Height == current.Y
                                && other.X == current.X
                                && other.Height > 0
                                && current.Height > 0)
                        {
                            if (other.Type == current.Type)
                            {
                                if (other.Width == current.Width)
                                {
                                    current.Height--;
                                    current.Y++;
                                    other.Height++;
                                    break;
                                }
                            }
                        }
                    }
                }*/
            }

            private void CreateMergedHorizontalTiles(TileLayer tileLayer)
            {
                Rectangle currentRect = default;
                var lastColumnIndex = tileLayer.Height - 1;

                for (int rowIndex = 0; rowIndex < tileLayer.Width; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < tileLayer.Height; columnIndex++)
                    {
                        var tile = tileLayer.Tiles[rowIndex, columnIndex];

                        if (columnIndex == 0)
                        {
                            currentRect = new Rectangle
                            {
                                X = columnIndex + 1,
                                Y = rowIndex + 1,
                                Width = 1,
                                Height = 1
                            };

                            continue;
                        }

                        if (columnIndex == lastColumnIndex)
                        {
                            if (tile.HasTile)
                            {
                                Result.Add(currentRect);
                                currentRect.Width++;
                            }
                            else
                            {
                                Result.Add(currentRect);
                                currentRect = new Rectangle
                                {
                                    X = columnIndex + 1,
                                    Y = rowIndex + 1,
                                    Width = 1,
                                    Height = 1
                                };
                                Result.Add(currentRect);
                            }

                            continue;
                        }

                        if (tile.HasTile)
                        {
                            currentRect.Width++;
                        }
                        else
                        {
                            Result.Add(currentRect);
                            currentRect = new Rectangle
                            {
                                X = columnIndex + 1,
                                Y = rowIndex + 1,
                                Width = 1,
                                Height = 1
                            };
                        }
                    }
                }
            }
        }
    }
}