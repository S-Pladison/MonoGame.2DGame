using Pladi.Core.Scenes;
using Pladi.Utilities.DataStructures;

namespace Pladi.Core.Entities
{
    public class TileEntity : Entity
    {
        // [constructors]

        public TileEntity(LevelScene scene, RectangleF hitbox) : base(scene)
        {
            Hitbox = hitbox;

            SetMassAsImmovable();
        }
    }
}