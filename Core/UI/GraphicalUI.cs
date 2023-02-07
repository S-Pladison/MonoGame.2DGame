﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI.Elements;
using Pladi.Core.UI.Events;
using Pladi.Utilities.Enums;

namespace Pladi.Core.UI
{
    public class GraphicalUI
    {
        private UIElement lastElementHover;

        // ...

        public UIElement Core { get; private set; }

        // ...

        public GraphicalUI()
        {
            Core = new UIElement();

            Core.Left.SetValue(0, 0);
            Core.Top.SetValue(0, 0);
            Core.Width.SetValue(Main.ScreenSize.X, 0);
            Core.Height.SetValue(Main.ScreenSize.Y, 0);

            Core.Recalculate();
        }

        // ...

        public void Append(UIElement element)
            => Core.Append(element);

        public void Update()
        {
            var input = Main.InputManager;
            var mousePosition = input.GetMousePosition();
            var mouseLeft = input.JustPressed(MouseInputTypes.LeftButton);
            var mouseElement = Core.GetElementAt(mousePosition);

            UpdateMouseHover(mouseElement, mousePosition);
            UpdateMouseClick(mouseElement, mousePosition, mouseLeft);

            Core.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Core.Draw(spriteBatch);
        }

        public void OnResolutionChanged(int width, int height)
        {
            Core.Left.SetValue(0, 0);
            Core.Top.SetValue(0, 0);
            Core.Width.SetValue(Main.ScreenSize.X, 0);
            Core.Height.SetValue(Main.ScreenSize.Y, 0);

            Core.Recalculate();
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