using Microsoft.Xna.Framework;

namespace Pladi.Core
{
    public abstract class BasicComponent : DrawableGameComponent, ILoadable
    {
        // [public properties and fields]

        public int LoadOrder { get; set; }

        // [constructors]

        public BasicComponent() : base(Main.Instance)
        {
            Main.Instance.Components.Add(this);
        }

        // [public methods]

        public virtual void Load() { }
    }
}