using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Input;
using Pladi.Core.UI.Events;
using Pladi.Utilities;
using Pladi.Utilities.Enums;

namespace Pladi.Core.UI.Elements
{
    public class TextInputFieldUIElement : UIElement
    {
        private readonly int blinkingCursorStateTime;
        private readonly SpriteFont font;
        private float blinkingCursorTime;

        // ...

        public bool IsFocused { get; private set; }
        public string Text { get; private set; }
        public string HintText { get; set; }

        // ...

        public TextInputFieldUIElement()
        {
            blinkingCursorStateTime = 60;
            font = FontAssets.DefaultMedium;

            OnMouseClick += Click;
            Text = string.Empty;
        }

        // ...

        protected override void UpdateThis()
        {
            if (!IsFocused) return;

            var input = ILoadable.GetInstance<InputComponent>();
            var mousePosition = input.MousePosition;

            if (!ContainsPoint(mousePosition) && input.JustPressed(MouseInputTypes.LeftButton))
            {
                IsFocused = false;
                blinkingCursorTime = 0;
                return;
            }

            Text = input.GetInputText(Text);

            UpdateBlinkingCursor();
        }

        private void UpdateBlinkingCursor()
        {
            blinkingCursorTime -= Main.DeltaTime * 60;

            if (blinkingCursorTime <= 0)
            {
                blinkingCursorTime = blinkingCursorStateTime;
            }
        }

        private void Click(UIMouseEvent evt, UIElement elem)
        {
            IsFocused = true;
        }

        protected override void DrawThis(SpriteBatch spriteBatch)
        {
            var cursor = (blinkingCursorTime > (blinkingCursorStateTime / 2) ? "|" : "");

            if (Text is null || Text is "")
            {
                spriteBatch.DrawStringWithShadow(font, $"{HintText ?? ""}", Position + Vector2.UnitX * 4, Color.Gray, 0, Vector2.Zero, 1f, 2f);
                spriteBatch.DrawStringWithShadow(font, $"{cursor}", Position, Color.White, 0, Vector2.Zero, 1f, 2f);
            }
            else
            {
                spriteBatch.DrawStringWithShadow(font, $"{Text + cursor}", Position, Color.White, 0, Vector2.Zero, 1f, 2f);
            }
        }
    }
}