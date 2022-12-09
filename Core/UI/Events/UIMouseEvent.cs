using Microsoft.Xna.Framework;
using Pladi.Core.UI.Elements;

namespace Pladi.Core.UI.Events
{
    public class UIMouseEvent : UIEvent
    {
        public readonly Vector2 MousePosition;

        // ...

        public UIMouseEvent(UIElement target, Vector2 mousePosition) : base(target)
        {
            MousePosition = mousePosition;
        }
    }
}