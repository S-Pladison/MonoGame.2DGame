using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.UI
{
    public abstract class UIElement
    {
        public delegate void MouseEvent(UIMouseEvent evt, UIElement listeningElement);
		public delegate void ElementEvent(UIElement affectedElement);

		// ...

		public Vector2 Position;
		public int Width;
		public int Height;

		// ...

		public Vector2 Center
		{
			get => new(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
			set => Position = new Vector2(value.X - (float)(Width / 2), value.Y - (float)(Height / 2));
		}

		public Vector2 Left
		{
			get => new(Position.X, Position.Y + (float)(Height / 2));
			set => Position = new Vector2(value.X, value.Y - (float)(Height / 2));
		}

		public Vector2 Right
		{
			get => new(Position.X + (float)Width, Position.Y + (float)(Height / 2));
			set => Position = new Vector2(value.X - (float)Width, value.Y - (float)(Height / 2));
		}

		public Vector2 Top
		{
			get => new(Position.X + (float)(Width / 2), Position.Y);
			set => Position = new Vector2(value.X - (float)(Width / 2), value.Y);
		}

		public Vector2 TopLeft
		{
			get => Position;
			set => Position = value;
		}

		public Vector2 TopRight
		{
			get => new(Position.X + (float)Width, Position.Y);
			set => Position = new Vector2(value.X - (float)Width, value.Y);
		}

		public Vector2 Bottom
		{
			get => new(Position.X + (float)(Width / 2), Position.Y + (float)Height);
			set => Position = new Vector2(value.X - (float)(Width / 2), value.Y - (float)Height);
		}

		public Vector2 BottomLeft
		{
			get => new(Position.X, Position.Y + (float)Height);
			set => Position = new Vector2(value.X, value.Y - (float)Height);
		}

		public Vector2 BottomRight
		{
			get => new(Position.X + (float)Width, Position.Y + (float)Height);
			set => Position = new Vector2(value.X - (float)Width, value.Y - (float)Height);
		}

		public Vector2 Size
		{
			get => new(Width, Height);
			set
			{
				Width = (int)value.X;
				Height = (int)value.Y;
			}
		}

		public Rectangle Hitbox
		{
			get => new((int)Position.X, (int)Position.Y, Width, Height);
			set
			{
				Position = new Vector2(value.X, value.Y);
				Width = value.Width;
				Height = value.Height;
			}
		}

		public bool IsMouseHovering { get; set; }

        // ...

        public void Update(GameTime gameTime)
        {
			OnUpdate(gameTime);
			OnPostUpdate(this);
        }

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
			OnDraw(gameTime, spriteBatch);
        }

		protected virtual void OnUpdate(GameTime gameTime) { }
        protected virtual void OnDraw(GameTime gameTime, SpriteBatch spriteBatch) { }

		public void MouseOver(UIMouseEvent evt)
		{
			IsMouseHovering = true;
			OnMouseOver(evt, this);
		}

		public void Click(UIMouseEvent evt)
		{
			OnMouseClick(evt, this);
		}

		public bool ContainsPoint(Vector2 point)
        {
			var rectangle = Hitbox;

            if (point.X > rectangle.X
                && point.Y > rectangle.Y
                && point.X < rectangle.X + rectangle.Width)

                return point.Y < rectangle.Y + rectangle.Height;

            return false;
        }

        // ...

        public event MouseEvent OnMouseOver = (evt, elem) => { };
		public event MouseEvent OnMouseClick = (evt, elem) => { };
		public event ElementEvent OnPostUpdate = (elem) => { };
	}
}