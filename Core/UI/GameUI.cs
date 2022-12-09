using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI.Elements;
using Pladi.Core.UI.Events;
using Pladi.Utilities.Enums;

namespace Pladi.Core.UI
{
    public class GameUI
    {
        private CoreUIElement coreElement;
        private UIElement lastElementHover;

        // ...

        public GameUI(CoreUIElement element)
        {
            SetCoreElement(element);
        }

        // ...

        public void SetCoreElement(CoreUIElement element)
        {
            coreElement = element;
        }

        public void Update(GameTime gameTime)
        {
            var input = Main.InputManager;
            var mousePosition = input.GetMousePosition();
            var mouseLeft = input.JustPressed(MouseInputTypes.LeftButton);
            var mouseElement = coreElement.GetElementAt(mousePosition);

            UpdateMouseHover(mouseElement, mousePosition);
            UpdateMouseClick(mouseElement, mousePosition, mouseLeft);

            coreElement.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            coreElement.Draw(gameTime, spriteBatch);
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