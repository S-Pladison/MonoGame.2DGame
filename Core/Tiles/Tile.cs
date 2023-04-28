namespace Pladi.Core.Tiles
{
    public struct Tile
    {
        // [public properties and fields]

        public ushort Type { get; set; }

        public bool HasTile => Type != 0;
    }
}