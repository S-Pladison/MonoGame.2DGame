using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Tiles;
using Pladi.Utilities;
using Pladi.Utilities.Enums;
using System;

namespace Pladi.Core.Scenes
{
    public class EditorScene : Scene
    {
        private GameLevel level;
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
            collisionTilemap.RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, width, height);*/
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
        }
    }
}