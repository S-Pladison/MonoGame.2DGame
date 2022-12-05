using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.UI;

namespace Pladi.Scenes
{
    public class MenuScene : Scene
    {
        private UserInterface userInterface;

        // ...

        public override void Init()
        {
            userInterface = new UserInterface();

            var button = new TextUIElement(FontAssets.DefaultMedium, "Start", Color.White);
            button.Position = new Vector2(10, 10);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.AddElement(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Editor", Color.White);
            button.Position = new Vector2(10, 40);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Editor);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.AddElement(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Settings", Color.White);
            button.Position = new Vector2(10, 70);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Settings);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.AddElement(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Exit", Color.White);
            button.Position = new Vector2(10, 100);
            button.OnMouseClick += (evt, elem) => Main.ExitFromGame();
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.AddElement(button);
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
            spriteBatch.GraphicsDevice.Clear(new Color(36, 34, 41));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}