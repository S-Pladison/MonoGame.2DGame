using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.Collisions;
using Pladi.Core.Entities;
using Pladi.Core.Graphics;
using Pladi.Core.Graphics.Lighting;
using Pladi.Core.Input;
using Pladi.Core.Tiles;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;
using Pladi.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pladi.Core.Scenes
{
    public class LevelScene : Scene
    {
        // [private properties and fields]

        private GameLevel gameLevel;
        private string gameLevelName;

        private GraphicalUI userInterface;
        private KeyboardButtonSlotUIElement[] uiEntityElements;
        private MouseKeyboardButtonUIElement mouseEntityElement;

        private List<Entity> entities;
        private SpatialHash<Entity> entitySpatialHash;
        private PlayerEntity player;

        private LevelCollision levelCollision;

        private CameraComponent camera;
        private Vector2 cameraSmoothVelocity;

        private LightRenderer lightRendered;
        private WallRenderer wallRenderer;
        private TileRenderer tileRenderer;
        private EntityRenderer entityRenderer;

        private bool shouldDrawHitboxes;
        private bool pause;


        // [public methods]

        public override void OnActivate()
        {
            Main.Instance.OnPreDraw += RequestLightRenderTarget;

            camera = ILoadable.GetInstance<CameraComponent>();
            lightRendered = ILoadable.GetInstance<LightRenderer>();
            wallRenderer = ILoadable.GetInstance<WallRenderer>();
            tileRenderer = ILoadable.GetInstance<TileRenderer>();
            entityRenderer = ILoadable.GetInstance<EntityRenderer>();

            ActivateUserInterface();
            ActivateLevel();
            PrepareLightRenderer();
            ActivateEntities();
            ResetLevel();

            levelCollision = new(gameLevel.TileMap.TileLayer, entitySpatialHash);
        }

        public override void OnDeactivate()
        {
            levelCollision = null;

            DeactivateEntities();
            DeactivateUserInterface();

            lightRendered.PrepareStaticShadowVertices(null);

            camera = null;
            lightRendered = null;
            wallRenderer = null;
            tileRenderer = null;
            entityRenderer = null;

            Main.Instance.OnPreDraw -= RequestLightRenderTarget;
        }

        public override void Update()
        {
            var input = ILoadable.GetInstance<InputComponent>();

            if (input.JustPressed(Keys.Escape))
            {
                SceneComponent.SetActiveScene<LevelMenuScene>();
                return;
            }

            if (!pause)
            {
                UpdateInput();
                UpdateEntities();
                UpdateCamera();
                UpdateShadowEdges();
            }

            userInterface.Update();
        }

        public override void OnResolutionChanged(int width, int height)
        {
            userInterface.OnResolutionChanged(width, height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!lightRendered.TryGetTargetIfPrepared(out var lightTarget) ||
                !wallRenderer.TryGetTargetIfPrepared(out var wallTarget) ||
                !tileRenderer.TryGetTargetIfPrepared(out var tileTarget) ||
                !entityRenderer.TryGetTargetIfPrepared(out var entityTarget))
                throw new Exception("Одна из целей рендеринга не была подготовлена...");

            PrepareEffects();

            spriteBatch.GraphicsDevice.Clear(new Color(47, 54, 73) * 0.5f);

            Draw_Walls(spriteBatch);
            Draw_Entities(spriteBatch);
            Draw_Tiles(spriteBatch);
            Draw_HitboxesAndGrid(spriteBatch);
            Draw_UI(spriteBatch);
        }

        public void RequestLightRenderTarget(GameTime _)
        {
            lightRendered.Request((gameLevel.Lights, gameLevel.TileMap.TileLayer));
            wallRenderer.Request(gameLevel.TileMap.WallLayer);
            tileRenderer.Request(gameLevel.TileMap.TileLayer);
            entityRenderer.Request(entities);
        }

        public void ResetLevel()
        {
            pause = false;

            mouseEntityElement.SetEntity(null);

            uiEntityElements[0].SetEntity(new KeyboardButtonEntity.LeftKeyboardButtonEntity(this));
            uiEntityElements[1].SetEntity(new KeyboardButtonEntity.SpaceKeyboardButtonEntity(this));
            uiEntityElements[2].SetEntity(new KeyboardButtonEntity.RightKeyboardButtonEntity(this));

            entities.Clear();
            entitySpatialHash.Clear();

            foreach (var entityInfo in gameLevel.Entities)
            {
                var entity = Entity.CreateEntityByType(entityInfo.Item1, this);
                entity.Position = entityInfo.Item2;

                AddEntity(entity);
            }

            player = entities.FirstOrDefault(x => x is PlayerEntity, null) as PlayerEntity;

            if (player is null)
            {
                throw new Exception("Не удалось найти сущность игрока...");
            }
        }

        public void UpdateDeadZoneInfo()
        {
            var value = false;

            foreach (var plates in entities.Where(x => x is PressurePlateTrigger))
            {
                value |= (plates as Trigger).AnyEntitiesInHistory;
            }

            foreach (var deadZones in entities.Where(x => x is DeadZoneTrigger))
            {
                (deadZones as DeadZoneTrigger).IsDangerous = !value;
            }
        }

        public void SetGameLevelName(string name)
        {
            gameLevelName = name;
        }

        public void Pause()
        {
            pause = true;
        }

        public void Complite()
        {
            if (pause) return;

            Pause();

            static void InitButtonHoverEvents(UIElement button)
            {
                button.OnMouseOver += (_, e) =>
                {
                    var text = (e as TextUIElement);
                    text.FontScale = 1.1f;
                    text.FontColor = Color.Gold;
                };
                button.OnMouseOut += (_, e) =>
                {
                    var text = (e as TextUIElement);
                    text.FontScale = 1.0f;
                    text.FontColor = Color.White;
                };
            }

            var blackScreen = new PanelUIElement(Color.Black * 0.75f);
            blackScreen.Width.SetPercent(1f);
            blackScreen.Height.SetPercent(1f);
            userInterface.Append(blackScreen);

            var secondPanel = new PanelUIElement(Color.White, PanelUIElement.PanelStyles.Standard);
            secondPanel.HorizontalAlign = 0.5f;
            secondPanel.VerticalAlign = 0.5f;
            secondPanel.Width.SetPixel(220.0f);
            secondPanel.Height.SetPixel(110.0f);
            blackScreen.Append(secondPanel);

            var lablePanel = new PanelUIElement(Color.White, PanelUIElement.PanelStyles.Standard);
            lablePanel.HorizontalAlign = 0.5f;
            lablePanel.Width.SetPixel(150.0f);
            lablePanel.Height.SetPixel(40.0f);
            lablePanel.Top.SetPixel(-20.0f);
            secondPanel.Append(lablePanel);

            var labelText = new TextUIElement("Уровень пройден!");
            labelText.HorizontalAlign = 0.5f;
            labelText.VerticalAlign = 0.5f;
            lablePanel.Append(labelText);

            var button = new TextUIElement("Перезапустить");
            InitButtonHoverEvents(button);
            button.Top.SetPixel(40.0f);
            button.HorizontalAlign = 0.5f;
            button.OnMouseClick += (_, _) => SceneComponent.SetActiveScene<LevelScene>();
            secondPanel.Append(button);

            button = new TextUIElement("Вернуться");
            InitButtonHoverEvents(button);
            button.Top.SetPixel(71.0f);
            button.HorizontalAlign = 0.5f;
            button.OnMouseClick += (_, _) => SceneComponent.SetActiveScene<LevelMenuScene>();
            secondPanel.Append(button);
        }

        // [private methods]

        private void ActivateUserInterface()
        {
            userInterface = new();
            uiEntityElements = new KeyboardButtonSlotUIElement[3]
            {
                new(new KeyboardButtonEntity.LeftKeyboardButtonEntity(this)),
                new(new KeyboardButtonEntity.SpaceKeyboardButtonEntity(this)),
                new(new KeyboardButtonEntity.RightKeyboardButtonEntity(this))
            };

            var buttonPanel = new PanelUIElement(Color.White * 0.5f);
            buttonPanel.Width.SetPixel(274.0f);
            buttonPanel.Height.SetPixel(70.0f);
            buttonPanel.Top.SetPixel(-4.0f);
            buttonPanel.HorizontalAlign = 0.5f;
            buttonPanel.VerticalAlign = 1.0f;
            userInterface.Append(buttonPanel);

            var button = uiEntityElements[0];
            button.Width.SetPixel(48.0f);
            button.Height.SetPixel(48.0f);
            button.Left.SetPixel(11.0f);
            button.Top.SetPixel(11.0f);
            button.OnMouseClick += (e, o) =>
            {
                if (mouseEntityElement.ButtonEntity is not null) return;

                var instance = o as KeyboardButtonSlotUIElement;

                if (instance.ButtonEntity is not null)
                {
                    mouseEntityElement.SetEntity(instance.ButtonEntity);
                    instance.SetEntity(null);
                    player.Input &= ~PlayerEntity.InputFlags.Left;
                }
            };
            buttonPanel.Append(button);

            button = uiEntityElements[1];
            button.Width.SetPixel(144.0f);
            button.Height.SetPixel(48.0f);
            button.Left.SetPixel(65.0f);
            button.Top.SetPixel(11.0f);
            button.OnMouseClick += (e, o) =>
            {
                if (mouseEntityElement.ButtonEntity is not null) return;

                var instance = o as KeyboardButtonSlotUIElement;

                if (instance.ButtonEntity is not null)
                {
                    mouseEntityElement.SetEntity(instance.ButtonEntity);
                    instance.SetEntity(null);
                    player.Input &= ~PlayerEntity.InputFlags.Jump;
                }
            };
            buttonPanel.Append(button);

            button = uiEntityElements[2];
            button.Width.SetPixel(48.0f);
            button.Height.SetPixel(48.0f);
            button.Left.SetPixel(215.0f);
            button.Top.SetPixel(11.0f);
            button.OnMouseClick += (e, o) =>
            {
                if (mouseEntityElement.ButtonEntity is not null) return;

                var instance = o as KeyboardButtonSlotUIElement;

                if (instance.ButtonEntity is not null)
                {
                    mouseEntityElement.SetEntity(instance.ButtonEntity);
                    instance.SetEntity(null);
                    player.Input &= ~PlayerEntity.InputFlags.Right;
                }
            };
            buttonPanel.Append(button);

            mouseEntityElement = new MouseKeyboardButtonUIElement();
            userInterface.Append(mouseEntityElement);
        }

        private void DeactivateUserInterface()
        {
            userInterface = null;
            uiEntityElements = null;
            mouseEntityElement = null;
        }

        private void ActivateEntities()
        {
            entities = new();
            entitySpatialHash = new(48 * 5, 48 * 5);
        }

        private void DeactivateEntities()
        {
            entities = null;
            player = null;
            entitySpatialHash = null;
        }

        private void ActivateLevel()
        {
            if (!GameLevel.TryLoadLevelFromFile(gameLevelName, out GameLevel level))
            {
                throw new Exception($"Не удалось загрузить уровень \"{gameLevelName}\"...");
            }

            gameLevel = level;
            gameLevel.TileMap.WallLayer.SetPalette(new TilePalette(TextureAssets.WallPalette, 5, 1));
            gameLevel.TileMap.TileLayer.SetPalette(new TilePalette(TextureAssets.TilePalette, 4, 1));
        }

        private void PrepareLightRenderer()
        {
            var rectangle = new RectangleF(0, 0, gameLevel.TileMap.Width * 48 * gameLevel.TileMap.TileLayer.Palette.TileWidth, gameLevel.TileMap.Height * 48 * gameLevel.TileMap.TileLayer.Palette.TileHeight);
            var edges = gameLevel.TileMap.TileLayer.GetEdgesFromTiles(rectangle);

            lightRendered.PrepareStaticShadowVertices(edges);
            lightRendered.PrepareDynamicBuffers(1024);
        }

        private void AddEntity(Entity entity)
        {
            entity.OnChangePosition += () => entitySpatialHash.Update(entity.Hitbox, entity);
            entitySpatialHash.Insert(entity.Hitbox, entity);
            entities.Add(entity);
        }

        private void RemoveEntity(Entity entity)
        {
            if (!entities.Contains(entity)) return;

            entity.OnChangePosition = null;
            entitySpatialHash.Remove(entity.Hitbox, entity);
            entities.Remove(entity);
        }

        private void RestoreButtonEntity(Entity entity)
        {
            if (player is null) return;
            if (entity is not KeyboardButtonEntity buttonEntity) return;

            player.Input |= buttonEntity.InputType;

            switch (buttonEntity.InputType)
            {
                case PlayerEntity.InputFlags.Left:
                    uiEntityElements[0].SetEntity(buttonEntity);
                    break;
                case PlayerEntity.InputFlags.Jump:
                    uiEntityElements[1].SetEntity(buttonEntity);
                    break;
                case PlayerEntity.InputFlags.Right:
                    uiEntityElements[2].SetEntity(buttonEntity);
                    break;
                default:
                    break;
            }
        }

        private void UpdateInput()
        {
            var input = ILoadable.GetInstance<InputComponent>();

            if (input.JustPressed(Keys.R))
            {
                ResetLevel();
                return;
            }

            if (input.JustPressed(Keys.NumPad5))
                shouldDrawHitboxes = !shouldDrawHitboxes;

            var uiMousePosition = input.MousePosition;
            var worldMousePosition = Vector2.Transform(uiMousePosition, camera.InvertViewMatrix);

            if (mouseEntityElement.ButtonEntity is not null)
            {
                var mouseEntity = mouseEntityElement.ButtonEntity;
                var entityRect = mouseEntity.Hitbox;
                entityRect.Location = Vector2.Floor(worldMousePosition / 48) * 48 + new Vector2(24, 24) - entityRect.Size * 0.5f;

                mouseEntityElement.CanBePlaced =
                    new RectangleF(0, 0, gameLevel.Width * 48, gameLevel.Height * 48).Contains(entityRect)
                    && gameLevel.TileMap.WallLayer.IsRectInTiles(entityRect)
                    && !levelCollision.IsRectCollideWithEntities(entityRect);

                if (input.JustPressed(MouseInputTypes.RightButton))
                {
                    RestoreButtonEntity(mouseEntityElement.ButtonEntity);
                    mouseEntityElement.SetEntity(null);
                }
            }

            if (input.JustPressed(MouseInputTypes.LeftButton))
            {
                if (mouseEntityElement.ButtonEntity is null)
                {
                    var mouseRect = new RectangleF(worldMousePosition.X, worldMousePosition.Y, 1, 1);

                    foreach (var entity in entitySpatialHash.GetObjectsIntersectsWithRect(mouseRect).Where(e => e is KeyboardButtonEntity && e.Hitbox.Intersects(mouseRect)))
                    {
                        RestoreButtonEntity(entity);
                        RemoveEntity(entity);
                    }
                }
                else if (mouseEntityElement.CanBePlaced)
                {
                    AddEntity(mouseEntityElement.ButtonEntity);

                    mouseEntityElement.ButtonEntity.Position = Vector2.Floor(worldMousePosition / 48) * 48 + new Vector2(24, 24) - mouseEntityElement.ButtonEntity.Hitbox.Size * 0.5f;
                    mouseEntityElement.SetEntity(null);
                }
            }
        }

        private void UpdateEntities()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Update(levelCollision);
            }
        }

        private void UpdateCamera()
        {
            camera.Position = MathUtils.SmoothDamp(camera.Position, player.Hitbox.Center, ref cameraSmoothVelocity, 0.05f, Main.DeltaTime);
        }

        private void UpdateShadowEdges()
        {
            var shadowEdges = new List<EdgeF>();

            for (int i = 0; i < entities.Count; i++)
            {
                var e = entities[i].ShadowCastEdges;

                if (e is null) continue;

                shadowEdges.AddRange(e);
            }

            lightRendered.UpdateDynamicBuffers(shadowEdges);
        }

        private void PrepareEffects()
        {
            var effect = EffectAssets.TileEdgeShadow;
            effect.Parameters["TransformMatrix"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["Texture0Size"].SetValue(ILoadable.GetInstance<ScreenComponent>().Size.ToVector2());

            if (!lightRendered.TryGetTargetIfPrepared(out var lightTarget))
                throw new Exception("Цель рендеринга освещения не была подготовлена...");

            effect = EffectAssets.He;
            effect.Parameters["TransformMatrix"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["Texture1"].SetValue(lightTarget);

            effect = EffectAssets.Tile;
            effect.Parameters["TransformMatrix"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["Texture1"].SetValue(lightTarget);
            effect.Parameters["Resolution"].SetValue(ILoadable.GetInstance<ScreenComponent>().Size.ToVector2());
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

            Draw_OutlineEffect(spriteBatch, entityTarget);

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

        private void Draw_HitboxesAndGrid(SpriteBatch spriteBatch)
        {
            if (!shouldDrawHitboxes)
            {
                if (mouseEntityElement.ButtonEntity is null) return;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.ViewMatrix);
                spriteBatch.DrawGrid(camera.VisibleArea, Color.White * 0.05f, 48, 48, 1);
                spriteBatch.DrawGrid(camera.VisibleArea, Color.White * 0.1f, 48 * 5, 48 * 5, 2);
                spriteBatch.End();

                return;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.ViewMatrix);

            spriteBatch.DrawGrid(camera.VisibleArea, Color.White * 0.05f, 48, 48, 1);
            spriteBatch.DrawGrid(camera.VisibleArea, Color.White * 0.1f, 48 * 5, 48 * 5, 2);


            for (int i = 0; i < gameLevel.Lights.Count; i++)
            {
                spriteBatch.DrawRectangle(gameLevel.Lights[i].VisibleArea.ToRectangle(), Color.Yellow, 2);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                spriteBatch.DrawRectangle(entities[i].Hitbox.ToRectangle(), entities[i].IsTrigger ? Color.Blue : Color.Red, 2);
            }

            var mp = Vector2.Transform(ILoadable.GetInstance<InputComponent>().MousePosition, camera.InvertViewMatrix);
            var rect = new Rectangle((int)mp.X - 8, (int)mp.Y - 8, 16, 16);

            spriteBatch.DrawRectangle(rect, gameLevel.TileMap.WallLayer.IsRectInTiles(rect.ToRectangleF()) ? Color.Red : Color.Lime, 2);

            spriteBatch.End();
        }

        private void Draw_UI(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}