using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;
using Pladi.Utilities.DataStructures;

namespace Pladi.Scenes
{
    public class MenuScene : Scene
    {
        private GraphicalUI userInterface;

        // ...

        public override void OnActivate()
        {
            userInterface = new GraphicalUI();

            var panel = new PanelUIElement(new Color(0, 0, 0, 90));
            panel.ClippingOutsideRectangle = true;
            panel.BoundingRectangle = new RectangleF(5, 5, 500, Main.ScreenSize.Y - 10);
            panel.OnResolutionChanged += (evt, elem) => elem.BoundingRectangle = new RectangleF(5, 5, 500, evt.Height - 10);

            userInterface.CoreElement.Append(panel);

            var button = new TextUIElement(FontAssets.DefaultMedium, "Start", Color.White);
            button.Position = new Vector2(15, 10);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            panel.Append(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Editor", Color.White);
            button.Position = new Vector2(15, 40);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Editor);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            panel.Append(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Settings", Color.White);
            button.Position = new Vector2(15, 70);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Settings);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            panel.Append(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Exit", Color.White);
            button.Position = new Vector2(15, 100);
            button.OnMouseClick += (evt, elem) => Main.ExitFromGame();
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            panel.Append(button);
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