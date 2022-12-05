namespace Pladi.Tiles
{
    public struct Tile
    {
        public ushort Type { get; set; }
        public bool IsAir { get => Type is 0; }

        public Tile(ushort type)
        {
            Type = type;
        }
    }
}