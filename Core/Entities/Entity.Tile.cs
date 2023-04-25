using Pladi.Utilities.DataStructures;

namespace Pladi.Core.Entities
{
    public class TileEntity : Entity
    {
        // [constructors]

        public TileEntity(RectangleF hitbox)
        {
            Hitbox = hitbox;

            SetMassAsImmovable();
        }
    }
}