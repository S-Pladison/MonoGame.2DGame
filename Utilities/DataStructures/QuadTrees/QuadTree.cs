using System.Collections.Generic;
using System.Numerics;

namespace Pladi.Utilities.DataStructures.QuadTrees
{
    public class QuadTree<T> where T : IQuadTreeData
    {
        private readonly int objectsPerNode;

        // ...

        public RectangleF Rectangle { get; protected set; }
        public QuadTree<T> TopLeftChild { get; protected set; }
        public QuadTree<T> TopRightChild { get; protected set; }
        public QuadTree<T> BottomLeftChild { get; protected set; }
        public QuadTree<T> BottomRightChild { get; protected set; }
        public List<T> Objects { get; protected set; }
        public int Count { get => ObjectCount(); }

        // ...

        public QuadTree(RectangleF rect, int objectsPerNode = 8)
        {
            Rectangle = rect;

            this.objectsPerNode = objectsPerNode;
        }

        public QuadTree(int x, int y, int width, int height, int objectsPerNode = 8)
        {
            Rectangle = new(x, y, width, height);

            this.objectsPerNode = objectsPerNode;
        }

        // ...

        private void Add(T item)
        {
            Objects ??= new();
            Objects.Add(item);
        }

        private void Remove(T item)
        {
            if (Objects is not null && Objects.Contains(item))
            {
                Objects.Remove(item);
            }
        }

        private int ObjectCount()
        {
            int count = 0;

            if (Objects != null)
            {
                count += Objects.Count;
            }

            if (TopLeftChild != null)
            {
                count += TopLeftChild.ObjectCount();
                count += TopRightChild.ObjectCount();
                count += BottomLeftChild.ObjectCount();
                count += BottomRightChild.ObjectCount();
            }

            return count;
        }

        private void Subdivide()
        {
            var size = new Vector2(Rectangle.Width / 2, Rectangle.Height / 2);
            var mid = new Vector2(Rectangle.X + size.X, Rectangle.Y + size.Y);

            TopLeftChild = new QuadTree<T>(new(Rectangle.Left, Rectangle.Top, size.X, size.Y));
            TopRightChild = new QuadTree<T>(new(mid.X, Rectangle.Top, size.X, size.Y));
            BottomLeftChild = new QuadTree<T>(new(Rectangle.Left, mid.Y, size.X, size.Y));
            BottomRightChild = new QuadTree<T>(new(mid.X, mid.Y, size.X, size.Y));

            for (int i = 0; i < Objects.Count; i++)
            {
                var destTree = GetDestinationTree(Objects[i]);

                if (destTree != this)
                { 
                    destTree.Insert(Objects[i]);
                    Remove(Objects[i]);
                    i--;
                }
            }
        }

        private QuadTree<T> GetDestinationTree(T item)
        {
            var destTree = this;

            if (TopLeftChild.Rectangle.Contains(item.Rectangle))
            {
                destTree = TopLeftChild;
            }
            else if (TopRightChild.Rectangle.Contains(item.Rectangle))
            {
                destTree = TopRightChild;
            }
            else if (BottomLeftChild.Rectangle.Contains(item.Rectangle))
            {
                destTree = BottomLeftChild;
            }
            else if (BottomRightChild.Rectangle.Contains(item.Rectangle))
            {
                destTree = BottomRightChild;
            }

            return destTree;
        }

        public void Clear()
        {
            if (TopLeftChild != null)
            {
                TopLeftChild.Clear();
                TopRightChild.Clear();
                BottomLeftChild.Clear();
                BottomRightChild.Clear();
            }

            if (Objects != null)
            {
                Objects.Clear();
                Objects = null;
            }

            TopLeftChild = null;
            TopRightChild = null;
            BottomLeftChild = null;
            BottomRightChild = null;
        }

        public void Delete(T item)
        {
            bool objectRemoved = false;

            if (Objects is not null && Objects.Contains(item))
            {
                Remove(item);
                objectRemoved = true;
            }

            if (TopLeftChild is not null && !objectRemoved)
            {
                TopLeftChild.Delete(item);
                TopRightChild.Delete(item);
                BottomLeftChild.Delete(item);
                BottomRightChild.Delete(item);
            }

            if (TopLeftChild is not null && TopLeftChild.Count == 0 && TopRightChild.Count == 0 && BottomLeftChild.Count == 0 && BottomRightChild.Count == 0)
            {
                TopLeftChild = null;
                TopRightChild = null;
                BottomLeftChild = null;
                BottomRightChild = null;
            }
        }

        public void Insert(T item)
        {
            if (!Rectangle.Intersects(item.Rectangle)) return;

            if (Objects is null || (TopLeftChild is null && Objects.Count + 1 <= objectsPerNode))
            {
                Add(item);
                return;
            }

            if (TopLeftChild is null)
            {
                Subdivide();
            }

            var destTree = GetDestinationTree(item);

            if (destTree == this) Add(item);
            else destTree.Insert(item);
        }

        public List<T> GetObjects(RectangleF rect)
        {
            var results = new List<T>();

            GetObjects(rect, ref results);

            return results;
        }

        public void GetObjects(RectangleF rect, ref List<T> results)
        {
            if (results is null) return;

            if (rect.Contains(Rectangle))
            {
                GetAllObjects(ref results);
                return;
            }

            if (rect.Intersects(Rectangle))
            {
                if (Objects is not null)
                {
                    for (int i = 0; i < Objects.Count; i++)
                    {
                        if (rect.Intersects(Objects[i].Rectangle))
                        {
                            results.Add(Objects[i]);
                        }
                    }
                }

                if (TopLeftChild is not null)
                {
                    TopLeftChild.GetObjects(rect, ref results);
                    TopRightChild.GetObjects(rect, ref results);
                    BottomLeftChild.GetObjects(rect, ref results);
                    BottomRightChild.GetObjects(rect, ref results);
                }
            }
        }

        public List<T> GetAllObjects()
        {
            var results = new List<T>();

            GetAllObjects(ref results);

            return results;
        }

        public void GetAllObjects(ref List<T> results)
        {
            if (Objects is not null)
            {
                results.AddRange(Objects);
            }

            if (TopLeftChild is not null)
            {
                TopLeftChild.GetAllObjects(ref results);
                TopRightChild.GetAllObjects(ref results);
                BottomLeftChild.GetAllObjects(ref results);
                BottomRightChild.GetAllObjects(ref results);
            }
        }
    }
}
