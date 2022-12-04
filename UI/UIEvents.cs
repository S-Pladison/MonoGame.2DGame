using Microsoft.Xna.Framework;
using System;

namespace Pladi.UI
{
	public class UIEvent
	{
		public readonly UIElement Target;

		// ...

		public UIEvent(UIElement target)
		{
			Target = target;
		}
	}

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