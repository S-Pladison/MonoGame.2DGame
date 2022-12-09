using Pladi.Core.UI.Elements;

namespace Pladi.Core.UI.Events
{
    public class UIResolutionChangeEvent : UIEvent
    {
        public readonly int Width;
        public readonly int Height;

        // ...

        public UIResolutionChangeEvent(UIElement target, int width, int height) : base(target)
        {
            Width = width;
            Height = height;
        }
    }
}