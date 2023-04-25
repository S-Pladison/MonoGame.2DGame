using Microsoft.Xna.Framework;
using Pladi.Core.Collisions;
using Pladi.Core.Entities;
using Pladi.Core.Tiles;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace Pladi.Utilities
{
    public static class TileLayerUtils
    {
        public static bool IsCollideWithRect(this TileLayer layer, RectangleF rectangle)
        {
            if (layer.Palette is null)
                return false;

            layer.GetTileCoordsIntersectsWithRect(rectangle, out Point leftTop, out Point rightBottom);

            var tileWidth = layer.Palette.TileWidth * layer.Map.Scale;
            var tileHeight = layer.Palette.TileHeight * layer.Map.Scale;

            for (int x = leftTop.X; x <= rightBottom.X; x++)
            {
                for (int y = leftTop.Y; y <= rightBottom.Y; y++)
                {
                    ref var tile = ref layer.Tiles[x, y];

                    if (!tile.HasTile) continue;

                    var tileHitbox = new RectangleF(x * tileWidth, y * tileHeight, tileWidth, tileHeight);

                    if (tileHitbox.Intersects(rectangle)) return true;
                }
            }

            return false;
        }

        public static List<Entity> GetTileEntitiesIntersectsWithRect(this TileLayer layer, RectangleF rectangle)
        {
            layer.GetTileCoordsIntersectsWithRect(rectangle, out Point leftTop, out Point rightBottom);

            var tileWidth = layer.Palette.TileWidth * layer.Map.Scale;
            var tileHeight = layer.Palette.TileHeight * layer.Map.Scale;
            var boxes = new List<Entity>();

            for (int x = leftTop.X; x <= rightBottom.X; x++)
            {
                for (int y = leftTop.Y; y <= rightBottom.Y; y++)
                {
                    ref var tile = ref layer.Tiles[x, y];

                    if (!tile.HasTile) continue;

                    var tileHitbox = new RectangleF(x * tileWidth, y * tileHeight, tileWidth, tileHeight);

                    boxes.Add(new TileEntity(tileHitbox));
                }
            }

            return boxes;
        }

        public static EdgeF[] GetEdgesFromTiles(this TileLayer layer, Vector2 center, float radius)
        {
            var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            layer.GetTileCoordsIntersectsWithRect(rect, out Point leftTop, out Point rightBottom);

            var tileWidth = layer.Palette.TileWidth * layer.Map.Scale;
            var tileHeight = layer.Palette.TileHeight * layer.Map.Scale;
            var edges = new HashSet<EdgeF>();
            var duplicateEdges = new List<EdgeF>();

            for (int x = leftTop.X; x <= rightBottom.X; x++)
            {
                for (int y = leftTop.Y; y <= rightBottom.Y; y++)
                {
                    ref var tile = ref layer.Tiles[x, y];

                    if (!tile.HasTile) continue;

                    var hitbox = new RectangleF(x * tileWidth, y * tileHeight, tileWidth, tileHeight);

                    if (Vector2.Distance(rect.Center, center) > radius) continue;

                    var tileEdges = RectangleF.GetEdges(hitbox);

                    for (int a = 0; a < tileEdges.Length; a++)
                    {
                        ref var edge = ref tileEdges[a];
                        var invEdge = new EdgeF(edge.B, edge.A);

                        if (edges.Contains(edge) || edges.Contains(invEdge))
                        {
                            duplicateEdges.Add(edge);
                            duplicateEdges.Add(invEdge);
                            continue;
                        }

                        edges.Add(edge);
                    }
                }
            }

            for (int i = 0; i < duplicateEdges.Count; i++)
            {
                edges.Remove(duplicateEdges[i]);
            }

            return edges.ToArray();
        }

        public static EdgeF[] GetEdgesFromTiles(this TileLayer layer, RectangleF rectangle)
        {
            layer.GetTileCoordsIntersectsWithRect(rectangle, out Point leftTop, out Point rightBottom);

            var tileWidth = layer.Palette.TileWidth * layer.Map.Scale;
            var tileHeight = layer.Palette.TileHeight * layer.Map.Scale;
            var edges = new HashSet<EdgeF>();
            var duplicateEdges = new List<EdgeF>();

            for (int x = leftTop.X; x <= rightBottom.X; x++)
            {
                for (int y = leftTop.Y; y <= rightBottom.Y; y++)
                {
                    ref var tile = ref layer.Tiles[x, y];

                    if (!tile.HasTile) continue;

                    var hitbox = new RectangleF(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                    var tileEdges = RectangleF.GetEdges(hitbox);

                    for (int a = 0; a < tileEdges.Length; a++)
                    {
                        ref var edge = ref tileEdges[a];
                        var invEdge = new EdgeF(edge.B, edge.A);

                        if (edges.Contains(edge) || edges.Contains(invEdge))
                        {
                            duplicateEdges.Add(edge);
                            duplicateEdges.Add(invEdge);
                            continue;
                        }

                        edges.Add(edge);
                    }
                }
            }

            for (int i = 0; i < duplicateEdges.Count; i++)
            {
                edges.Remove(duplicateEdges[i]);
            }

            return edges.ToArray();
        }
    }
}