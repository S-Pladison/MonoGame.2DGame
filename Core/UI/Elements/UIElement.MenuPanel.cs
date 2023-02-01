using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.UI.Events;
using Pladi.Utilities;
using System.Linq;

namespace Pladi.Core.UI.Elements
{
    public class MenuPanelUIElement : PanelUIElement
    {
        private readonly SpriteFont font;

        // ...

        public MenuPanelUIElement(SpriteFont font) : base()
        {
            //ClippingOutsideRectangle = true;

            this.font = font;
        }

        // ...

        public void AddButton(string text, MouseEvent onClick)
        {
            var button = new MenuButtonUIElement(font, text);
            //button.SetRectangle(0, 0, width, 40);
            button.Width.SetValue(40, 0);
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

        public override void Recalculate()
        {
            var buttons = children.Where(x => x is MenuButtonUIElement).ToList();
            var buttonCount = buttons.Count;
            var offsetY = Height.Pixel / 2f - buttonCount / 2f * 40;

            for (int i = 0; i < buttonCount; i++)
            {
                //buttons[i].SetPosition(0, offsetY + i * 40);
            }

            base.Recalculate();
        }

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
                //TextUIElement.SetPosition((width - TextUIElement.Size.X) / 2f, (height - TextUIElement.Size.Y) / 2f);

                base.Recalculate();
            }

            protected override void DrawThis(GameTime gameTime, SpriteBatch spriteBatch)
            {
                if (!IsMouseHovering) return;

                //spriteBatch.Draw(TextureAssets.Pixel, boundingRectangle.ToRectangle(), new Color(0, 0, 0, 40));
            }
        }
    }
}