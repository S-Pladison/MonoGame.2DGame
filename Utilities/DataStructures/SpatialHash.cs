using Microsoft.Xna.Framework;
using Pladi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pladi.Utilities.DataStructures
{
    public class SpatialHash<T>
    {
        // [private properties and fields]

        private readonly int cellSizeX;
        private readonly int cellSizeY;
        private readonly Dictionary<int, HashSet<T>> cells;
        private readonly Dictionary<T, List<int>> objects;

        // [constructors]

        public SpatialHash(int cellSizeX, int cellSizeY)
        {
            this.cellSizeX = cellSizeX;
            this.cellSizeY = cellSizeY;
            this.cells = new();
            this.objects = new();
        }

        // [public methods]

        public void Insert(RectangleF rectangle, T obj)
        {
            var rectCells = GetCellsOfRectangle(rectangle);

            if (obj is PlayerEntity)
                Debug.WriteLine(rectCells.Count);

            if (!objects.ContainsKey(obj))
                objects.Add(obj, new());

            for (int i = 0; i < rectCells.Count; i++)
            {
                var cell = rectCells[i];

                if (!cells.ContainsKey(cell))
                    cells.Add(cell, new());

                objects[obj].Add(rectCells[i]);
                cells[rectCells[i]].Add(obj);
            }
        }

        public void Update(RectangleF rectangle, T obj)
        {
            if (objects.ContainsKey(obj))
            {
                for (int i = 0; i < objects[obj].Count; i++)
                {
                    cells[objects[obj][i]].Remove(obj);
                }

                objects[obj].Clear();
            }

            Insert(rectangle, obj);
        }

        public bool ContainsCell(Vector2 vector)
            => cells.ContainsKey(GetCellAtPosition(vector));

        public void Clear()
        {
            cells.Clear();
            objects.Clear();
        }

        public List<T> GetObjectsIntersectsWithRect(RectangleF rectangle)
        {
            var rectCells = GetCellsOfRectangle(rectangle);
            var result = new List<T>();

            for (int i = 0; i < rectCells.Count; i++)
            {
                if (!cells.ContainsKey(rectCells[i])) continue;

                result.AddRange(cells[rectCells[i]]);
            }

            return result;
        }

        public int GetCellAtPosition(Vector2 position)
            => ((int)(position.X / cellSizeX) * 92837111) ^ ((int)(position.Y / cellSizeY) * 68928749);

        public List<int> GetCellsOfRectangle(RectangleF rectangle)
        {
            var l = MathF.Floor(rectangle.X / cellSizeX) * cellSizeX;
            var t = MathF.Floor(rectangle.Y / cellSizeY) * cellSizeY;
            var r = MathF.Ceiling((rectangle.X + rectangle.Width) / cellSizeX) * cellSizeX;
            var b = MathF.Ceiling((rectangle.Y + rectangle.Height) / cellSizeY) * cellSizeY;

            var result = new List<int>();

            for (var x = l; x < r; x += cellSizeX)
            {
                for (var y = t; y < b; y += cellSizeY)
                {
                    result.Add(GetCellAtPosition(new Vector2(x, y)));
                }
            }

            return result;
        }
    }
}