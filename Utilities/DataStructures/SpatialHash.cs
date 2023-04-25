using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pladi.Utilities.DataStructures
{
    public class SpatialHash<T>
    {
        private readonly int cellSizeX;
        private readonly int cellSizeY;
        private readonly Dictionary<int, List<T>> cells;
        private readonly Dictionary<T, int> objects;

        // ...

        public SpatialHash(int cellSizeX, int cellSizeY)
        {
            this.cellSizeX = cellSizeX;
            this.cellSizeY = cellSizeY;
            this.cells = new Dictionary<int, List<T>>();
            this.objects = new Dictionary<T, int>();
        }

        // ...

        public void Insert(Vector2 vector, T obj)
        {
            var key = GetCellAtPosition(vector);

            if (cells.ContainsKey(key))
                cells[key].Add(obj);
            else
                cells[key] = new() { obj };

            objects[obj] = key;
        }

        public void Update(Vector2 vector, T obj)
        {
            if (objects.ContainsKey(obj))
                cells[objects[obj]].Remove(obj);

            Insert(vector, obj);
        }

        public bool ContainsCell(Vector2 vector)
            => cells.ContainsKey(GetCellAtPosition(vector));

        public void Clear()
        {
            var keys = cells.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++)
                cells[keys[i]].Clear();
            objects.Clear();
        }

        public void Reset()
        {
            cells.Clear();
            objects.Clear();
        }

        public List<T> GetObjectsIntersectsWithRect(RectangleF rectangle)
        {
            if (rectangle.Width > cellSizeX || rectangle.Height > cellSizeY)
                throw new Exception("Стороны проверяемой зоны не должны превышать размеров одной клетки...");

            var x = rectangle.X - 0.005f;
            var y = rectangle.Y - 0.005f;
            var w = rectangle.Width + 0.01f;
            var h = rectangle.Height + 0.01f;

            var lt = GetCellAtPosition(new Vector2(x, y));
            var rt = GetCellAtPosition(new Vector2(x + w, y));
            var lb = GetCellAtPosition(new Vector2(x, y + h));
            var rb = GetCellAtPosition(new Vector2(x + w, y + h));

            var set = new HashSet<int>() { lt, rt, lb, rb };
            var result = new List<T>();

            foreach (var cell in set)
            {
                if (!cells.ContainsKey(cell)) continue;

                result.AddRange(cells[cell]);
            }

            return result;
        }

        public int GetCellAtPosition(Vector2 position)
            => ((int)(position.X / cellSizeX) * 92837111) ^ ((int)(position.Y / cellSizeY) * 68928749);
    }
}