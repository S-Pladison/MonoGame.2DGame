using Pladi.Core.UI.Elements;

namespace Pladi.Core.UI.Events
{
    public class UIEvent
    {
        public readonly UIElement Element;

        // ...

        public UIEvent(UIElement element)
        {
            Element = element;
        }
    }
}
