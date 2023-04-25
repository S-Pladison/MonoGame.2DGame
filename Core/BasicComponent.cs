using Microsoft.Xna.Framework;

namespace Pladi.Core
{
    public abstract class BasicComponent : DrawableGameComponent, ILoadable
    {
        public int LoadOrder { get; set; }

        // ...

        public BasicComponent() : base(Main.Instance)
        {
            Main.Instance.Components.Add(this);
        }

        // ...

        public virtual void Load() { }
    }
}