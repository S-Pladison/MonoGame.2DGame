using Microsoft.Xna.Framework;

namespace Pladi.Core
{
    public abstract class BaseComponent : DrawableGameComponent, ILoadable
    {
        // [public properties and fields]

        public int LoadOrder { get; set; }

        // [constructors]

        public BaseComponent() : base(Main.Instance)
        {
            Main.Instance.Components.Add(this);
        }
    }
}