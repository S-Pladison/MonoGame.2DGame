using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Input;
using System.Collections.Generic;

namespace Pladi.UI
{
    public class UserInterface
    {
        private List<UIElement> elements;

        // ...

        public UserInterface()
        {
            elements = new();
        }

        // ...

        public void AddElement(UIElement element)
        {
            if (elements.Contains(element)) return;

            elements.Add(element);
        }

        public void Update(GameTime gameTime)
        {
            var mousePosition = Main.InputManager.GetMousePosition();
            var mouseLeft = Main.InputManager.JustPressed(MouseInputTypes.LeftButton);

            foreach (var elem in elements)
            {
                elem.IsMouseHovering = false;

                if (elem.ContainsPoint(mousePosition))
                {
                    elem.MouseOver(new UIMouseEvent(elem, mousePosition));

                    if (mouseLeft)
                    {
                        elem.Click(new UIMouseEvent(elem, mousePosition));
                    }
                }

                elem.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var elem in elements)
            {
                elem.Draw(gameTime, spriteBatch);
            }
        }
    }
}