namespace Pladi.Tiles
{
    public struct Tile
    {
        public ushort Type { get; set; }
        public bool IsCollidable { get => Type is not 0; }

        public Tile(ushort type)
        {
            Type = type;
        }
    }
}