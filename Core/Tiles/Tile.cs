using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Tiles
{
    public struct Tile
    {
        public ushort Type { get; set; }

        public bool HasTile => Type != 0;
    }
}