namespace Pladi.Core.Tiles
{
    public class TileMap
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public int Scale { get; init; }

        public TileLayer WallLayer { get; init; }
        public TileLayer TileLayer { get; init; }
        public TileLayer CollisionLayer { get; init; }

        //private QuadTree<IQuadTreeData> collisionQuadTree;

        // ...

        public TileMap(int width, int height, int scale)
        {
            Width = width;
            Height = height;
            Scale = scale;

            WallLayer = new TileLayer(this);
            TileLayer = new TileLayer(this);
            CollisionLayer = new TileLayer(this);

            //collisionQuadTree = new(new RectangleF(0, 0, Width * Scale, Height * Scale), 5);
        }
    }
}