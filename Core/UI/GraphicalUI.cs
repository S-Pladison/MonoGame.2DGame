using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI.Elements;
using Pladi.Core.UI.Events;
using Pladi.Utilities.Enums;

namespace Pladi.Core.UI
{
    public class GraphicalUI
    {
        public UIElement Core { get; private set; }

        private UIElement lastElementHover;

        // ...

        public GraphicalUI()
        {
            Core = new UIElement();
            Core.SetRectangle(0, 0, Main.ScreenSize.X, Main.ScreenSize.Y);
        }

        // ...

        public void Append(UIElement element)
            => Core.Append(element);

        public void Update(GameTime gameTime)
        {
            var input = Main.InputManager;
            var mousePosition = input.GetMousePosition();
            var mouseLeft = input.JustPressed(MouseInputTypes.LeftButton);
            var mouseElement = Core.GetElementAt(mousePosition);

            UpdateMouseHover(mouseElement, mousePosition);
            UpdateMouseClick(mouseElement, mousePosition, mouseLeft);

            Core.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Core.Draw(gameTime, spriteBatch);
        }

        public void OnResolutionChanged(int width, int height)
        {
            Core.ResolutionChanged(new UIResolutionChangeEvent(Core, width, height));
        }

        private void UpdateMouseHover(UIElement mouseElement, Vector2 mousePosition)
        {
            if (mouseElement == lastElementHover) return;

            lastElementHover?.MouseOut(new UIMouseEvent(lastElementHover, mousePosition));
            mouseElement?.MouseOver(new UIMouseEvent(mouseElement, mousePosition));
            lastElementHover = mouseElement;
        }

        private void UpdateMouseClick(UIElement mouseElement, Vector2 mousePosition, bool mouseLeft)
        {
            if (!mouseLeft || !(mouseElement?.ContainsPoint(mousePosition) ?? false)) return;

            mouseElement.Click(new UIMouseEvent(mouseElement, mousePosition));
        }
    }
}