using System;

namespace Pladi.Enums
{
    [Flags]
    public enum CollisionSides : byte
    {
        None = 0,

        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Buttom = 1 << 3
    }
}
