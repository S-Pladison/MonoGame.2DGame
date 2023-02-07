using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Utilities.DataStructures;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Core.Entities
{
    public class Entity
    {
        public Vector2 Position;
        public float Width;
        public float Height;

        // ...

        public RectangleF Hitbox
        {
            get => new(Position.X, Position.Y, Width, Height);
            set
            {
                Position = new(value.X, value.Y);
                Width = value.Width;
                Height = value.Height;
            }
        }

        // ...

        public Entity()
        {
            Position = Vector2.Zero;
        }

        // ...

        public virtual void Update() { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}