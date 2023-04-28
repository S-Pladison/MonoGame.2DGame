using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.UI.Events;
using System;

namespace Pladi.Core.UI.Elements
{
    public class SwitchUIElement : UIElement
    {
        public enum SwitchStyles
        {
            GreenRed,
            White
        }

        // ...

        public bool Value
        {
            get => value;
            set
            {
                this.value = value;
                var height = imageUIElement.Texture.Height / 2;

                imageUIElement.SetFrame(0, value ? 0 : height, imageUIElement.Width.Pixel, height);

                OnChangeValue?.Invoke(this);
            }
        }

        public Action<SwitchUIElement> OnChangeValue;

        // ...

        private ImageUIElement imageUIElement;
        private bool value;

        // ...

        public SwitchUIElement(string text, bool value = false, SwitchStyles style = SwitchStyles.GreenRed)
        {
            var textUIElement = new TextUIElement(text)
            {
                VerticalAlign = 0.5f
            };

            Append(textUIElement);

            imageUIElement = new ImageUIElement(GetTextureByStyle(style))
            {
                VerticalAlign = 0.5f,
                HorizontalAlign = 1f
            };

            imageUIElement.OnMouseClick += FlipValue;

            Append(imageUIElement);

            Width.SetPercent(1f);
            Height.SetPixel(32f);

            SetPadding(10f, 0f);

            Value = value;
        }

        // ...

        private void FlipValue(UIMouseEvent @event, UIElement element)
        {
            Value = !Value;
        }

        private Texture2D GetTextureByStyle(SwitchStyles style)
        {
            return style switch
            {
                SwitchStyles.White => TextureAssets.UI.Switch2,
                _ => TextureAssets.UI.Switch
            };
        }
    }
}
