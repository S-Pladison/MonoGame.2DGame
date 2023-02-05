using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.UI.Events;
using Pladi.Utilities;
using Pladi.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.UI.Elements
{
    public class TextInputFieldUIElement : UIElement
    {
        private readonly SpriteFont font;

        // ...

        public bool IsFocused { get; private set; }
        public string Text { get; private set; }

        // ...

        public TextInputFieldUIElement()
        {
            font = FontAssets.DefaultMedium;

            OnMouseClick += Click;
            Text = string.Empty;
        }

        // ...

        protected override void UpdateThis(GameTime gameTime)
        {
            if (!IsFocused) return;

            var input = Main.InputManager;
            var mousePosition = input.GetMousePosition();

            if (!ContainsPoint(mousePosition) && input.JustPressed(MouseInputTypes.LeftButton))
            {
                IsFocused = false;
                return;
            }

            Text = input.GetInputText(Text);
        }

        private void Click(UIMouseEvent evt, UIElement elem)
        {
            IsFocused = true;
        }

        protected override void DrawThis(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawStringWithShadow(font, $"{Text ?? ""}", position, Color.White, 0, Vector2.Zero, 1f, 2f);
        }
    }
}