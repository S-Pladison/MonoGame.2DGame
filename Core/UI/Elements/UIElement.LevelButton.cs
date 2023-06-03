using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Scenes;
using Pladi.Core.UI.Events;
using Pladi.Utilities;

namespace Pladi.Core.UI.Elements
{
    public class LevelButtonUIElement : UIElement
    {
        public int Index { get; init; }
        public bool IsExists { get; init; }

        // ...

        public LevelButtonUIElement(int index)
        {
            Index = index;
            Width.SetPixel(48.0f);
            Height.SetPixel(48.0f);

            var text = new TextUIElement(Index.ToString())
            {
                HorizontalAlign = 0.5f,
                VerticalAlign = 0.5f
            };

            Append(text);

            IsExists = GameLevel.LevelFileExists($"lv_{Index}");

            if (!IsExists) return;

            OnMouseClick += Click;
            text.OnMouseClick += Click;
        }

        // ...

        protected override void DrawThis(SpriteBatch spriteBatch)
        {
            var rect = Dimensions.ToRectangle();

            spriteBatch.Draw(TextureAssets.Pixel, rect, new Color(30, 32, 47));

            rect.X -= 1;
            rect.Y -= 1;
            rect.Width += 2;
            rect.Height += 2;

            spriteBatch.DrawRectangle(rect, IsExists ? (IsMouseHovering ? Color.Gold : Color.Cyan * 0.45f) : Color.Red * 0.45f, 2);
        }

        private void Click(UIMouseEvent evt, UIElement element)
        {
            (ILoadable.GetInstance<SceneComponent>().GetSceneInstance<LevelScene>() as LevelScene).SetGameLevelName($"lv_{Index}");
            ILoadable.GetInstance<SceneComponent>().SetActiveScene<LevelScene>();
        }
    }
}