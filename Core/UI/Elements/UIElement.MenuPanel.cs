using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI.Events;

namespace Pladi.Core.UI.Elements
{
    public class MenuPanelUIElement : PanelUIElement
    {
        private readonly SpriteFont font;

        // ...

        public MenuPanelUIElement(SpriteFont font) : base(Color.White)
        {
            ClippingOutsideRectangle = true;

            this.font = font;
        }

        // ...

        public void AddButton(string text, MouseEvent onClick)
        {
            var button = new MenuButtonUIElement(font, text);
            button.SetRectangle(0, 10 + 40 * children.Count, width, 40);
            button.OnMouseClick += onClick;

            Append(button);
        }

        public void AddButtons(params (string text, MouseEvent onClick)[] buttons)
        {
            foreach (var button in buttons)
            {
                AddButton(button.text, button.onClick);
            }
        }

        public void SetBackgroundColor(Color color)
            => backgroundColor = color;

        // ...

        private class MenuButtonUIElement : UIElement
        {
            public TextUIElement TextUIElement;
            public Color DefaultColor;
            public Color HoverColor;

            // ...

            public MenuButtonUIElement(SpriteFont font, string text)
            {
                TextUIElement = new TextUIElement(font, text, Color.White);
                DefaultColor = new Color(250, 250, 250);
                HoverColor = Color.Gold;

                Append(TextUIElement);
            }

            public override void MouseOver(UIMouseEvent evt)
            {
                TextUIElement.SetColor(HoverColor);

                base.MouseOver(evt);
            }

            public override void MouseOut(UIMouseEvent evt)
            {
                TextUIElement.SetColor(DefaultColor);

                base.MouseOut(evt);
            }

            public override void Recalculate()
            {
                TextUIElement.SetPosition((width - TextUIElement.Size.X) / 2f, (height - TextUIElement.Size.Y) / 2f);

                base.Recalculate();
            }
        }
    }
}