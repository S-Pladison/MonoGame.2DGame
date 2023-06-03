using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.Entities;
using Pladi.Core.Input;

namespace Pladi.Core.UI.Elements
{
    public class MouseKeyboardButtonUIElement : UIElement
    {
        public KeyboardButtonEntity ButtonEntity { get; private set; }
        public bool CanBePlaced { get; set; }

        // ...

        public MouseKeyboardButtonUIElement()
        {
            CanBePlaced = true;
        }

        // ...

        public void SetEntity(KeyboardButtonEntity entity)
        {
            ButtonEntity = entity;
        }

        protected override void UpdateThis()
        {
            var uiMousePosition = ILoadable.GetInstance<InputComponent>().MousePosition;
            var worldMousePosition = Vector2.Transform(uiMousePosition, ILoadable.GetInstance<CameraComponent>().InvertViewMatrix);
            var position = Vector2.Floor(worldMousePosition / 48.0f) * 48.0f;

            Left.SetPixel(position.X + 24f);
            Top.SetPixel(position.Y + 24f);

            Recalculate();
        }

        protected override void DrawThis(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, ILoadable.GetInstance<CameraComponent>().ViewMatrix);

            ButtonEntity?.DrawThis(spriteBatch, Position - ButtonEntity.Hitbox.Size * 0.5f, Color.White * (CanBePlaced ? 0.8f : 0.4f));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
        }
    }
}
