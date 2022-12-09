using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI.Events;
using Pladi.Utilities.DataStructures;
using System.Collections.Generic;

namespace Pladi.Core.UI.Elements
{
    public abstract class UIElement
    {
        public delegate void ResolutionChangeEvent(UIResolutionChangeEvent evt, UIElement affectedElement);
        public delegate void MouseEvent(UIMouseEvent evt, UIElement listeningElement);
        public delegate void ElementEvent(UIElement affectedElement);

        // ...

        public Vector2 Position;
        public int Width;
        public int Height;

        protected List<UIElement> children;

        // ...

        public IReadOnlyCollection<UIElement> Children => children;
        public UIElement Parent { get; private set; }

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

        public RectangleF BoundingRectangle
        {
            get => new(Position.X, Position.Y, Width, Height);
            set
            {
                Position = new Vector2((int)value.X, (int)value.Y);
                Width = (int)value.Width;
                Height = (int)value.Height;
            }
        }

        public bool IsMouseHovering { get; set; }

        // ...

        public UIElement()
        {
            children = new();
        }

        // ...

        public void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);

            OnPostUpdate(this);

            foreach (var child in children)
            {
                child.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            OnDraw(gameTime, spriteBatch);

            foreach (var child in children)
            {
                child.Draw(gameTime, spriteBatch);
            }
        }

        protected virtual void OnUpdate(GameTime gameTime) { }
        protected virtual void OnDraw(GameTime gameTime, SpriteBatch spriteBatch) { }

        public void Append(UIElement element)
        {
            element.Remove();
            element.Parent = this;
            children.Add(element);
        }

        public void Remove()
        {
            if (Parent is null) return;
            
            Parent.RemoveChild(this);
        }

        public void RemoveChild(UIElement child)
        {
            children.Remove(child);
            child.Parent = null;
        }

        public void RemoveAllChildren()
        {
            foreach (var child in children)
            {
                child.Parent = null;
            }

            children.Clear();
        }

        public void MouseOver(UIMouseEvent evt)
        {
            IsMouseHovering = true;
            OnMouseOver(evt, this);
        }

        public void MouseOut(UIMouseEvent evt)
        {
            IsMouseHovering = false;
            OnMouseOut(evt, this);
        }

        public void Click(UIMouseEvent evt)
        {
            OnMouseClick(evt, this);
        }

        public void ResolutionChanged(UIResolutionChangeEvent evt)
        {
            OnResolutionChanged(evt, this);

            foreach (var child in children)
            {
                child.ResolutionChanged(evt);
            }
        }

        public bool ContainsPoint(Vector2 point)
        {
            return BoundingRectangle.Contains(point);
        }

        public UIElement GetElementAt(Vector2 position)
        {
            UIElement uIElement = null;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                UIElement uIElement2 = children[i];

                if (uIElement2.ContainsPoint(position))
                {
                    uIElement = uIElement2;
                    break;
                }
            }

            if (uIElement != null) return uIElement.GetElementAt(position);
            if (!ContainsPoint(position)) return null;
            return this;
        }

        // ...

        public event MouseEvent OnMouseOver = Null_MouseEvent;
        public event MouseEvent OnMouseOut = Null_MouseEvent;
        public event MouseEvent OnMouseClick = Null_MouseEvent;
        public event ElementEvent OnPostUpdate = Null_ElementEvent;
        public event ResolutionChangeEvent OnResolutionChanged = Null_ResolutionChangeEvent;

        // ...

        private static readonly MouseEvent Null_MouseEvent = (evt, elem) => { };
        private static readonly ElementEvent Null_ElementEvent = (elem) => { };
        private static readonly ResolutionChangeEvent Null_ResolutionChangeEvent = (evt, elem) => { };
    }
}