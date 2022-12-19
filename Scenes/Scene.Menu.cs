using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;

namespace Pladi.Scenes
{
    public class MenuScene : Scene
    {
        private GraphicalUI userInterface;

        // ...

        public override void OnActivate()
        {
            userInterface = new GraphicalUI();

            var menuPanel = new MenuPanelUIElement(FontAssets.DefaultMedium);
            menuPanel.OnResolutionChanged += (evt, elem) => elem.SetRectangle(20, 0, 200, evt.Height);
            menuPanel.SetRectangle(20, 0, 200, Main.ScreenSize.Y);
            menuPanel.SetBackgroundColor(new Color(0, 0, 0, 90));
            menuPanel.AddButtons(
                ("Start", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game)),
                ("Editor", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Editor)),
                ("Settings", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Settings)),
                ("Exit", (_, _) => Main.ExitFromGame())
            );

            userInterface.Append(menuPanel);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            userInterface?.OnResolutionChanged(width, height);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game);
            }

            userInterface.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}