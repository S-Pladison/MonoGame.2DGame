using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Entities;
using Pladi.Utilities;

namespace Pladi.Core.UI.Elements
{
    public class KeyboardButtonSlotUIElement : UIElement
    {
        public KeyboardButtonEntity ButtonEntity { get; private set; }

        // ...

        public KeyboardButtonSlotUIElement(KeyboardButtonEntity buttonEntity)
        {
            ButtonEntity = buttonEntity;
        }

        // ...

        public void SetEntity(KeyboardButtonEntity entity)
        {
            ButtonEntity = entity;
        }

        protected override void DrawThis(SpriteBatch spriteBatch)
        {
            if (ButtonEntity != null)
            {
                ButtonEntity.DrawThis(spriteBatch, Position);
            }
            else
            {
                spriteBatch.Draw(TextureAssets.Pixel, Dimensions.ToRectangle(), new Color(22, 24, 35));
            }
        }
    }
}