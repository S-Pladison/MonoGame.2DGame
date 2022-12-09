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
        private GameUI userInterface;
        private CoreUIElement coreUIElement;

        // ...

        public override void Init()
        {
            coreUIElement = new CoreUIElement();
            userInterface = new GameUI(coreUIElement);

            var button = new TextUIElement(FontAssets.DefaultMedium, "Start", Color.White);
            button.Position = new Vector2(10, 10);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            coreUIElement.Append(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Editor", Color.White);
            button.Position = new Vector2(10, 40);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Editor);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            coreUIElement.Append(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Settings", Color.White);
            button.Position = new Vector2(10, 70);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Settings);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            coreUIElement.Append(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Exit", Color.White);
            button.Position = new Vector2(10, 100);
            button.OnMouseClick += (evt, elem) => Main.ExitFromGame();
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            coreUIElement.Append(button);
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