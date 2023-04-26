using Microsoft.Xna.Framework;
using Pladi.Core.Graphics.Lighting;
using Pladi.Core.Graphics;
using Pladi.Core.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Input;
using Microsoft.Xna.Framework.Input;
using Pladi.Utilities;
using Pladi.Utilities.Enums;
using System.IO;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;
using System.Linq;

namespace Pladi.Core.Scenes
{
    public class EditorScene : Scene
    {
        private enum DrawLayerTypes
        {
            NotSelected,
            Walls,
            Tiles,
            Collisions
        }

        //

        private GraphicalUI graphicalUI;
        private TileMap tileMap;

        private bool isGUIMouseClick;
        private bool drawTileLayer = true;

        private SwitchUIElement wallSwitchUIElement;
        private SwitchUIElement tileSwitchUIElement;
        private SwitchUIElement collisionSwitchUIElement;

        private DrawLayerTypes drawLayerType = DrawLayerTypes.NotSelected;

        // ...

        public override void OnActivate()
        {
            tileMap = new TileMap(100, 50, 3);

            if (TryLoadLevelFromFile("Debug", out var level))
            {
                tileMap = level;
            }
            else
            {
                tileMap.TileLayer.Tiles[0, 0].Type = 1;
            }

            tileMap.WallLayer.SetPalette(new TilePalette(TextureAssets.WallPalette, 5, 1));
            tileMap.TileLayer.SetPalette(new TilePalette(TextureAssets.TilePalette, 3, 1));

            ILoadable.GetInstance<CameraComponent>().ResetPosition();

            // ...

            graphicalUI = new GraphicalUI();

            /*var panel = new PanelUIElement();
            panel.Width.SetPixel(163f);
            panel.Height.SetPixel(55f);
            panel.Left.SetPixel(10f);
            panel.Top.SetPixel(10f);
            graphicalUI.Append(panel);

            var text = new TextUIElement();
            text.OnPostUpdate += (elem) => (elem as TextUIElement).Text = $"" +
                $"FPS: {(int)ILoadable.GetInstance<FrameCounterComponent>().AverageFramesPerSecond}\n" +
                $"Current layer: {(isWallLayer ? "Walls" : "Tiles")}";
            text.Left.SetPixel(10f);
            text.Top.SetPixel(10f);
            panel.Append(text);*/

            //Hee();

            InitLayerPanel();
        }

        private void InitLayerPanel()
        {
            var screen = ILoadable.GetInstance<ScreenComponent>();

            var panel = new PanelUIElement();
            panel.HorizontalAlign = 1f;
            panel.Width.SetPixel(230f);
            panel.Height.SetPixel(screen.Height);
            panel.OnResolutionChanged += (@event, elem) => elem.Height.SetPixel(screen.Height);
            graphicalUI.Append(panel);
        }

        private void Hee()
        {
            var panel = new PanelUIElement();
            panel.HorizontalAlign = 1f;
            panel.Width.SetPixel(230f);
            panel.Height.SetPixel(400f);
            panel.Left.SetPixel(-10f);
            panel.Top.SetPixel(10f);
            panel.SetPadding(0f, 5f);
            graphicalUI.Append(panel);

            void SetToNextPosition(UIElement e1, UIElement o)
            {
                var e2 = o.Children.Last();
                e1.Top.SetPixel(e2.Top.Pixel + e2.Height.Pixel);
            }

            var text = new TextUIElement("Выбор текущего слоя:");
            text.Top.SetPixel(3f);
            text.Left.SetPixel(10f);
            panel.Append(text);

            var panel2 = new PanelUIElement(Color.Black * 0.5f, PanelUIElement.PanelStyles.WithoutTexture);
            panel2.Width.SetPixel(222f);
            panel2.Left.SetPixel(4f);
            panel2.Height.SetPixel(106f);
            panel2.SetPadding(0f, 4f);
            SetToNextPosition(panel2, panel);
            panel2.Top.SetPixel(panel2.Top.Pixel + 4f);
            panel.Append(panel2);

            wallSwitchUIElement = new SwitchUIElement("1. Задние плитки", false, SwitchUIElement.SwitchStyles.White);
            wallSwitchUIElement.OnMouseClick += (@event, elem) =>
            {  
                isGUIMouseClick = true;

                if (!wallSwitchUIElement.Value)
                {
                    drawLayerType = DrawLayerTypes.NotSelected;
                    return;
                }

                drawLayerType = DrawLayerTypes.Walls;
                tileSwitchUIElement.Value = false;
                collisionSwitchUIElement.Value = false;
            };
            panel2.Append(wallSwitchUIElement);

            tileSwitchUIElement = new SwitchUIElement("2. Передние плитки", false, SwitchUIElement.SwitchStyles.White);
            tileSwitchUIElement.OnMouseClick += (@event, elem) =>
            {
                isGUIMouseClick = true;

                if (!tileSwitchUIElement.Value)
                {
                    drawLayerType = DrawLayerTypes.NotSelected;
                    return;
                }

                drawLayerType = DrawLayerTypes.Tiles;
                wallSwitchUIElement.Value = false;
                collisionSwitchUIElement.Value = false;
            };
            SetToNextPosition(tileSwitchUIElement, panel2);
            panel2.Append(tileSwitchUIElement);

            collisionSwitchUIElement = new SwitchUIElement("3. Коллизия", false, SwitchUIElement.SwitchStyles.White);
            collisionSwitchUIElement.OnMouseClick += (@event, elem) =>
            {
                isGUIMouseClick = true;

                if (!collisionSwitchUIElement.Value)
                {
                    drawLayerType = DrawLayerTypes.NotSelected;
                    return;
                }

                drawLayerType = DrawLayerTypes.Collisions;
                wallSwitchUIElement.Value = false;
                tileSwitchUIElement.Value = false;
            };
            SetToNextPosition(collisionSwitchUIElement, panel2);
            panel2.Append(collisionSwitchUIElement);

            /*var swtch = new SwitchUIElement("Draw \"Wall Layer\"", true);
            swtch.OnChangeValue += (elem) =>
            {
                isGUIMouseClick = true;
                drawTileLayer = elem.Value;
            };
            panel.Append(swtch);

            var prevSwtch = swtch;

            swtch = new SwitchUIElement("Draw \"Tile Layer\"", true, SwitchUIElement.SwitchStyle.White);
            swtch.Top.SetPixel(prevSwtch.Top.Pixel + prevSwtch.Height.Pixel);
            swtch.OnChangeValue += (elem) =>
            {
                isGUIMouseClick = true;
                drawTileLayer = elem.Value;
            };
            panel.Append(swtch);

            prevSwtch = swtch;

            swtch = new SwitchUIElement("Draw \"Collision Layer\"", true);
            swtch.Top.SetPixel(prevSwtch.Top.Pixel + prevSwtch.Height.Pixel);
            swtch.OnChangeValue += (elem) =>
            {
                isGUIMouseClick = true;
                drawTileLayer = elem.Value;
            };
            panel.Append(swtch);*/
        }

        public override void OnDeactivate()
        {
            SaveLevelToFile("Debug", tileMap);

            tileMap = null;
        }

        public override void Update()
        {
            isGUIMouseClick = false;

            var input = ILoadable.GetInstance<InputComponent>();
            var camera = ILoadable.GetInstance<CameraComponent>();

            GetInputMoveVector(input, out var inputVector);

            camera.Position += inputVector * 48f * 10f * Main.DeltaTime;

            if (input.JustPressed(Keys.Escape))
                SceneComponent.SetActiveScene(SceneComponent.GameScenes.Menu);

            graphicalUI.Update();

            if (!isGUIMouseClick && input.JustPressed(MouseInputTypes.LeftButton))
            {
                var mousePosition = Vector2.Transform(input.MousePosition, camera.InvertViewMatrix);

                if (mousePosition.X > 0 && mousePosition.Y > 0)
                {
                    var tilePosition = (mousePosition / 48).ToPoint();

                    if (tilePosition.X.Between(0, tileMap.Width) && tilePosition.Y.Between(0, tileMap.Height))
                    {
                        switch (drawLayerType)
                        {
                            case DrawLayerTypes.Walls:
                                tileMap.WallLayer.Tiles[tilePosition.X, tilePosition.Y].Type = (ushort)((tileMap.WallLayer.Tiles[tilePosition.X, tilePosition.Y].Type > 0) ? 0 : 1);
                                break;
                            case DrawLayerTypes.Tiles:
                                tileMap.TileLayer.Tiles[tilePosition.X, tilePosition.Y].Type = (ushort)((tileMap.TileLayer.Tiles[tilePosition.X, tilePosition.Y].Type > 0) ? 0 : 1);
                                break;
                            case DrawLayerTypes.Collisions:
                                tileMap.CollisionLayer.Tiles[tilePosition.X, tilePosition.Y].Type = (ushort)((tileMap.CollisionLayer.Tiles[tilePosition.X, tilePosition.Y].Type > 0) ? 0 : 1);
                                break;
                            case DrawLayerTypes.NotSelected:
                                default:
                                break;
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var camera = ILoadable.GetInstance<CameraComponent>();

            // ...

            spriteBatch.GraphicsDevice.Clear(Colors.Main * 0.5f);

            // ...

            tileMap.WallLayer.Draw(spriteBatch, Color.DarkGray, camera);
            if (drawTileLayer) tileMap.TileLayer.Draw(spriteBatch, Color.White, camera);

            var tileMapRect = new Rectangle(0, 0, tileMap.Width * 48, tileMap.Height * 48);
            var cameraRect = camera.VisibleArea;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.ViewMatrix);
            spriteBatch.DrawGrid(Rectangle.Intersect(tileMapRect, cameraRect), Color.White * 0.1f, 48, 48, 2);
            spriteBatch.DrawRectangle(tileMapRect, Color.White * 0.25f, 2);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            graphicalUI.Draw(spriteBatch);
            spriteBatch.End();
        }

        // ...

        private bool TryLoadLevelFromFile(string name, out TileMap level)
        {
            CreateMapDirectoryIfDontExists();

            var path = $"Maps/{name}.pl";

            if (!File.Exists(path))
            {
                level = null;
                return false;
            }

            using var reader = new BinaryReader(File.Open(path, FileMode.Open));

            try
            {
                var scale = reader.ReadInt32();
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();

                level = new TileMap(width, height, scale);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        level.WallLayer.Tiles[i, j].Type = reader.ReadUInt16();
                        level.TileLayer.Tiles[i, j].Type = reader.ReadUInt16();
                    }
                }

                return true;
            }
            catch
            {
                reader?.Close();
                level = null;
            }

            return false;
        }

        private void SaveLevelToFile(string name, TileMap level)
        {
            CreateMapDirectoryIfDontExists();

            var path = $"Maps/{name}.pl";

            using var writer = new BinaryWriter(File.Create(path));

            writer.Write(tileMap.Scale);
            writer.Write(tileMap.Width);
            writer.Write(tileMap.Height);

            for (int i = 0; i < tileMap.Width; i++)
            {
                for (int j = 0; j < tileMap.Height; j++)
                {
                    writer.Write(tileMap.WallLayer.Tiles[i, j].Type);
                    writer.Write(tileMap.TileLayer.Tiles[i, j].Type);
                }
            }
        }

        private void CreateMapDirectoryIfDontExists()
        {
            if (Directory.Exists("Maps")) return;

            Directory.CreateDirectory("Maps");
        }

        private void GetInputMoveVector(InputComponent input, out Vector2 vector)
        {
            vector = Vector2.Zero;

            if (input.IsPressed(Keys.A))
                vector -= Vector2.UnitX;

            if (input.IsPressed(Keys.D))
                vector += Vector2.UnitX;

            if (input.IsPressed(Keys.W))
                vector -= Vector2.UnitY;

            if (input.IsPressed(Keys.S))
                vector += Vector2.UnitY;
        }


        /*private GameLevel level;
        private Grid grid;
        private Camera camera;

        private int currentTileType;
        private bool collideLayer;

        // ...

        public override void OnActivate()
        {
            level = new GameLevel(width: 100, height: 100, tileScale: 4f);

            try
            {
                level = GameLevel.LoadFromFile("Editor.pgm");
                level.BackTilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
                level.CollisionTilemap.SetTexture(TextureAssets.CollisionTilemap, 2, 1);
            }
            catch
            {
                level = new GameLevel(100, 100, 4f);
            }

            grid = new Grid(32, 32, 2);

            camera = new Camera(Main.SpriteBatch.GraphicsDevice.Viewport, 1f);
        }

        public override void OnDeactivate()
        {
            level.SaveToFile("Editor.pgm");
        }

        public override void Update(GameTime gameTime)
        {
            var input = Main.InputManager;
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (input.JustPressed(Keys.Escape))
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            }

            float cameraSpeed = 32 * 15 * delta;

            if (input.IsPressed(Keys.W))
            {
                camera.Location -= Vector2.UnitY * cameraSpeed;
            }

            if (input.IsPressed(Keys.S))
            {
                camera.Location += Vector2.UnitY * cameraSpeed;
            }

            if (input.IsPressed(Keys.D))
            {
                camera.Location += Vector2.UnitX * cameraSpeed;
            }

            if (input.IsPressed(Keys.A))
            {
                camera.Location -= Vector2.UnitX * cameraSpeed;
            }

            if (input.JustPressed(Keys.NumPad7))
            {
                currentTileType++;
            }

            if (input.JustPressed(Keys.NumPad4))
            {
                currentTileType--;
            }

            if (input.JustPressed(Keys.NumPad1))
            {
                collideLayer = !collideLayer;
                currentTileType = 0;
            }

            var scroll = Main.InputManager.GetMouseScroll();

            if (scroll != 0)
            {
                camera.Zoom += Math.Sign(scroll) * 5f * delta;
            }

            if (!input.JustPressed(MouseInputTypes.LeftButton)) return;

            var mousePosition = camera.ScreenToWorldSpace(Main.InputManager.GetMousePosition());
            var tileCoordsX = (int)(mousePosition.X / 32);
            var tileCoordsY = (int)(mousePosition.Y / 32);

            if (tileCoordsX < 0 || tileCoordsX >= level.Width || tileCoordsY < 0 || tileCoordsY >= level.Height) return;

            //tilemap.SetTile(tileCoordsX, tileCoordsY, new Tile() { Type = (ushort)currentTileType });

            if (collideLayer)
            {
                level.CollisionTilemap.SetTile(tileCoordsX, tileCoordsY, new Tile() { Type = (ushort)currentTileType });
            }
            else
            {
                level.BackTilemap.SetTile(tileCoordsX, tileCoordsY, new Tile() { Type = (ushort)currentTileType });
            }
        }

        public override void OnResolutionChanged(int width, int height)
        {
            var device = Main.SpriteBatch.GraphicsDevice;

            camera.Viewport = Main.SpriteBatch.GraphicsDevice.Viewport;
            /*tilemap.RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, width, height);
            collisionTilemap.RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, width, height);
            level.RecreateRenderTargets(device, width, height);
            grid.RecreateRenderTarget(device, width, height);
        }

        public override void PreDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            level.Render(spriteBatch, camera);
            grid.Render(spriteBatch, camera);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var device = spriteBatch.GraphicsDevice;
            device.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);
            level.BackTilemap.Draw(spriteBatch, Vector2.Zero);
            spriteBatch.End();

            DrawCollisionAreas(spriteBatch);
            DrawGrid(spriteBatch);
            DrawTempInfo(spriteBatch);
        }

        // ...

        private void DrawCollisionAreas(SpriteBatch spriteBatch)
        {
            var effect = EffectAssets.Collision;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, EffectAssets.Collision, camera.TransformMatrix);

            var texture = level.CollisionTilemap.RenderedTexture;
            var thickness = 2 / level.CollisionTilemap.Scale;

            effect.Parameters["Texture1"].SetValue(TextureAssets.Collision);
            effect.Parameters["OutlineColor"].SetValue(new Color(57, 184, 255).ToVector4());
            effect.Parameters["OutlineWidth"].SetValue(thickness / (float)texture.Width);
            effect.Parameters["OutlineHeight"].SetValue(thickness / (float)texture.Height);
            effect.Parameters["Texture1UvMult"].SetValue(new Vector2(texture.Width, texture.Height) / 32 * 4);
            effect.Parameters["Offset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.25f));

            level.CollisionTilemap.Draw(spriteBatch, Vector2.Zero);

            spriteBatch.End();
        }

        private void DrawGrid(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(grid.RenderedTexture, Vector2.Zero, null, new Color(240, 240, 240, 5), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private void DrawTempInfo(SpriteBatch spriteBatch)
        {
            var mousePosition = camera.ScreenToWorldSpace(Main.InputManager.GetMousePosition());
            var tileCoordsX = (int)(mousePosition.X / 32);
            var tileCoordsY = (int)(mousePosition.Y / 32);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Rendered tiles: {level.BackTilemap.RenderedTileCount}", new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Tile coords [Mouse]: x:{tileCoordsX} y:{tileCoordsY}", new Vector2(5, 20), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Tile type: {currentTileType}", new Vector2(5, 35), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Layer type: {(collideLayer ? "Collider" : "Back")}", new Vector2(5, 50), Color.White, 0, Vector2.Zero, 1f, 1f);

            spriteBatch.End();
        }*/
    }
}