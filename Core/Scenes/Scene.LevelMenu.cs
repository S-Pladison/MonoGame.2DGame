using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Core.Input;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;

namespace Pladi.Core.Scenes
{
    public class LevelMenuScene : Scene
    {
        // [private properties and fields]

        private GraphicalUI userInterface;

        // [public methods]

        public override void OnActivate()
        {
            userInterface = new GraphicalUI();

            InitUserInterface();
        }

        public override void OnDeactivate()
        {
            userInterface.Core.RemoveAllChildren();
            userInterface = null;
        }

        public override void Update()
        {
            if (ILoadable.GetInstance<InputComponent>().JustPressed(Keys.Escape))
            {
                SceneComponent.SetActiveScene<MenuScene>();
                return;
            }

            userInterface.Update();
        }

        public override void OnResolutionChanged(int width, int height)
        {
            userInterface.OnResolutionChanged(width, height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(new Color(47, 54, 73));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(spriteBatch);
            spriteBatch.End();
        }

        public void InitUserInterface()
        {
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

            var panel = new PanelUIElement(new Color(30, 32, 47), PanelUIElement.PanelStyles.WithoutTexture);
            panel.HorizontalAlign = 0.5f;
            panel.VerticalAlign = 0.5f;
            panel.Width.SetPixel(600.0f);
            panel.Height.SetPercent(1f);
            userInterface.Append(panel);

            var secondPanel = new PanelUIElement(Color.White, PanelUIElement.PanelStyles.Standard);
            secondPanel.HorizontalAlign = 0.5f;
            secondPanel.VerticalAlign = 0.5f;
            secondPanel.Width.SetPixel(640.0f);
            secondPanel.Height.SetPixel(340.0f);
            panel.Append(secondPanel);

            var lablePanel = new PanelUIElement(Color.White, PanelUIElement.PanelStyles.Standard);
            lablePanel.HorizontalAlign = 0.5f;
            lablePanel.Width.SetPixel(130.0f);
            lablePanel.Height.SetPixel(40.0f);
            lablePanel.Top.SetPixel(-20.0f);
            secondPanel.Append(lablePanel);

            var labelText = new TextUIElement("Выбор уровней");
            labelText.HorizontalAlign = 0.5f;
            labelText.VerticalAlign = 0.5f;
            lablePanel.Append(labelText);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var levelButton = new LevelButtonUIElement(j * 10 + i + 1);
                    levelButton.Left.SetPixel(20.0f + i * 60f);
                    levelButton.Top.SetPixel(50.0f + j * 60f);
                    secondPanel.Append(levelButton);
                }
            }

            /*var buttonPanel = new PanelUIElement(Color.Transparent);
            buttonPanel.VerticalAlign = 0.5f;
            buttonPanel.Width.SetPercent(1.0f);
            buttonPanel.Height.SetPixel(110f);
            panel.Append(buttonPanel);

            var button = new TextUIElement("Начать игру");
            InitButtonHoverEvents(button);
            button.HorizontalAlign = 0.55f;
            button.VerticalAlign = 0.15f;
            button.OnMouseClick += (_, _) => SceneComponent.SetActiveScene(SceneComponent.GameScenes.Game);
            buttonPanel.Append(button);

            button = new TextUIElement("Настройки");
            InitButtonHoverEvents(button);
            button.HorizontalAlign = 0.55f;
            button.VerticalAlign = 0.15f;
            button.Top.SetPixel(31.0f);
            button.OnMouseClick += (_, _) => SceneComponent.SetActiveScene(SceneComponent.GameScenes.Settings);
            buttonPanel.Append(button);

            button = new TextUIElement("Выход");
            InitButtonHoverEvents(button);
            button.HorizontalAlign = 0.55f;
            button.VerticalAlign = 0.15f;
            button.Top.SetPixel(62.0f);
            //button.OnMouseClick += (_, _) => Main.Instance.Exit();
            button.OnMouseClick += (_, _) => SceneComponent.SetActiveScene(SceneComponent.GameScenes.Editor);
            buttonPanel.Append(button);

            var version = new TextUIElement("v1.0.0.1");
            version.FontColor = Color.Gray;
            version.HorizontalAlign = 1.0f;
            version.VerticalAlign = 1.0f;
            version.Left.SetPixel(-4.0f);
            version.Top.SetPixel(-4.0f);
            userInterface.Append(version);*/
        }
    }
}