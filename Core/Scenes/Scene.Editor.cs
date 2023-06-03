using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.Entities;
using Pladi.Core.Graphics.Lighting;
using Pladi.Core.Input;
using Pladi.Core.Tiles;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;
using Pladi.Utilities;
using Pladi.Utilities.Enums;
using System;
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
        private GameLevel gameLevel;

        private bool isGUIMouseClick;
        private bool drawTileLayer = true;

        private SwitchUIElement wallSwitchUIElement;
        private SwitchUIElement tileSwitchUIElement;
        private SwitchUIElement collisionSwitchUIElement;

        private DrawLayerTypes drawLayerType = DrawLayerTypes.NotSelected;

        // ...

        public override void OnActivate()
        {
            gameLevel = new GameLevel(100, 50, 3);

            if (GameLevel.TryLoadLevelFromFile("lv_1", out var level))
            {
                gameLevel = level;
            }

            gameLevel.TileMap.WallLayer.SetPalette(new TilePalette(TextureAssets.WallPalette, 5, 1));
            gameLevel.TileMap.TileLayer.SetPalette(new TilePalette(TextureAssets.TilePalette, 4, 1));

            gameLevel.Entities.Clear();
            gameLevel.Lights.Clear();

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

            Hee();

            //InitLayerPanel();
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
            // ...

            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Player, new Vector2(3 * 48, 20 * 48)));

            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(6 * 48, 23 * 48) + new Vector2(8)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(7 * 48, 23 * 48) + new Vector2(8)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(8 * 48, 23 * 48) + new Vector2(8)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(9 * 48, 23 * 48) + new Vector2(8)));

            gameLevel.Lights.Add(new Light(Color.Cyan * 0.35f, new Vector2(2 * 48, 18 * 48) + new Vector2(24), 15 * 48));

            // ...

            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(18 * 48, 20 * 48) + new Vector2(8)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(19 * 48, 20 * 48) + new Vector2(8)));

            gameLevel.Lights.Add(new Light(Color.Red * 0.35f, new Vector2(21 * 48, 18 * 48) + new Vector2(24), 15 * 48));

            // ...

            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Crate, new Vector2(3 * 48, 12 * 48)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(5 * 48, 14 * 48) + new Vector2(8)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(6 * 48, 14 * 48) + new Vector2(8)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Spikes, new Vector2(7 * 48, 14 * 48) + new Vector2(8)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.PressurePlate, new Vector2(9 * 48, 12 * 48) + new Vector2(8, 32)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.DeadZone, new Vector2(1 * 48, 9 * 48)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.DeadZone, new Vector2(2 * 48, 9 * 48)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.DeadZone, new Vector2(3 * 48, 9 * 48)));
            gameLevel.Entities.Add(new Tuple<EntityTypes, Vector2>(EntityTypes.Finish, new Vector2(6 * 48, 6 * 48)));

            gameLevel.Lights.Add(new Light(Color.Lime * 0.35f, new Vector2(9 * 48, 11 * 48) + new Vector2(24), 12 * 48));
            gameLevel.Lights.Add(new Light(Color.CadetBlue * 0.35f, new Vector2(6 * 48, 3 * 48) + new Vector2(24), 12 * 48));

            // ...

            gameLevel.Entities.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            // ...

            gameLevel.SaveToFile("lv_1");
            gameLevel = null;
        }

        public override void Update()
        {
            isGUIMouseClick = false;

            var input = ILoadable.GetInstance<InputComponent>();
            var camera = ILoadable.GetInstance<CameraComponent>();

            GetInputMoveVector(input, out var inputVector);

            camera.Position += inputVector * 48f * 10f * Main.DeltaTime;

            if (input.JustPressed(Keys.Escape))
                SceneComponent.SetActiveScene<MenuScene>();

            graphicalUI.Update();

            if (!isGUIMouseClick && input.JustPressed(MouseInputTypes.LeftButton))
            {
                var mousePosition = Vector2.Transform(input.MousePosition, camera.InvertViewMatrix);

                if (mousePosition.X > 0 && mousePosition.Y > 0)
                {
                    var tilePosition = (mousePosition / 48).ToPoint();

                    if (tilePosition.X.Between(0, gameLevel.TileMap.Width) && tilePosition.Y.Between(0, gameLevel.TileMap.Height))
                    {
                        switch (drawLayerType)
                        {
                            case DrawLayerTypes.Walls:
                                gameLevel.TileMap.WallLayer.Tiles[tilePosition.X, tilePosition.Y].Type = (ushort)((gameLevel.TileMap.WallLayer.Tiles[tilePosition.X, tilePosition.Y].Type + 1) % gameLevel.TileMap.WallLayer.Palette.CountX);
                                break;
                            case DrawLayerTypes.Tiles:
                                gameLevel.TileMap.TileLayer.Tiles[tilePosition.X, tilePosition.Y].Type = (ushort)((gameLevel.TileMap.TileLayer.Tiles[tilePosition.X, tilePosition.Y].Type + 1) % gameLevel.TileMap.TileLayer.Palette.CountX);
                                break;
                            case DrawLayerTypes.Collisions:
                                gameLevel.TileMap.CollisionLayer.Tiles[tilePosition.X, tilePosition.Y].Type = (ushort)((gameLevel.TileMap.TileLayer.Tiles[tilePosition.X, tilePosition.Y].Type + 1) % gameLevel.TileMap.TileLayer.Palette.CountX);
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

            spriteBatch.GraphicsDevice.Clear(new Color(47, 54, 73));

            // ...

            gameLevel.TileMap.WallLayer.Draw(spriteBatch, Color.DarkGray, camera);
            if (drawTileLayer) gameLevel.TileMap.TileLayer.Draw(spriteBatch, Color.White, camera);

            var tileMapRect = new Rectangle(0, 0, gameLevel.TileMap.Width * 48, gameLevel.TileMap.Height * 48);
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
    }
}