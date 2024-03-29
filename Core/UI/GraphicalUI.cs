﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.Input;
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
            var screen = ILoadable.GetInstance<ScreenComponent>();

            Core = new UIElement();

            Core.Left.SetValue(0, 0);
            Core.Top.SetValue(0, 0);
            Core.Width.SetValue(screen.Width, 0);
            Core.Height.SetValue(screen.Height, 0);

            Core.Recalculate();

            screen.OnResolutionChanged += OnResolutionChanged;
        }

        ~GraphicalUI()
        {
            ILoadable.GetInstance<ScreenComponent>().OnResolutionChanged -= OnResolutionChanged;
        }

        // ...

        public void Append(UIElement element)
            => Core.Append(element);

        public void Update()
        {
            var input = ILoadable.GetInstance<InputComponent>();
            var mousePosition = input.MousePosition;
            var mouseLeft = input.JustPressed(MouseInputTypes.LeftButton);
            var mouseElement = Core.GetElementAt(mousePosition);

            Core.Update();

            UpdateMouseHover(mouseElement, mousePosition);
            UpdateMouseClick(mouseElement, mousePosition, mouseLeft);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Core.Draw(spriteBatch);
        }

        public void OnResolutionChanged(int width, int height)
        {
            Core.Left.SetValue(0, 0);
            Core.Top.SetValue(0, 0);
            Core.Width.SetValue(width, 0);
            Core.Height.SetValue(height, 0);

            Core.ResolutionChanged(new UIResolutionChangeEvent(Core, width, height));
            Core.Recalculate();
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