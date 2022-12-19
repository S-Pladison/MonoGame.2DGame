using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI.Events;
using Pladi.Utilities.DataStructures;
using System.Collections.Generic;

namespace Pladi.Core.UI.Elements
{
    public class UIElement
    {
        public delegate void ResolutionChangeEvent(UIResolutionChangeEvent evt, UIElement affectedElement);
        public delegate void MouseEvent(UIMouseEvent evt, UIElement listeningElement);
        public delegate void ElementEvent(UIElement affectedElement);

        // ...

        public Vector2 Position;
        public int Width;
        public int Height;
        public bool ClippingOutsideRectangle;

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
            UpdateThis(gameTime);

            OnPostUpdate(this);

            UpdateChildren(gameTime);
        }

        protected virtual void UpdateThis(GameTime gameTime) { }

        protected virtual void UpdateChildren(GameTime gameTime)
        {
            foreach (var child in children)
            {
                child.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawThis(gameTime, spriteBatch);

            if (!ClippingOutsideRectangle)
            {
                DrawChildren(gameTime, spriteBatch);
                return;
            }

            spriteBatch.End();

            var spriteBatchData = new SpriteBatchData(spriteBatch);
            var rasterizerState = spriteBatchData.RasterizerState;
            var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatchData.RasterizerState = ClippingRasterizerState;
            spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(GetClippingRectangle(spriteBatchData, scissorRectangle), scissorRectangle);
            spriteBatchData.Begin(spriteBatch);

            DrawChildren(gameTime, spriteBatch);

            spriteBatch.End();
            spriteBatchData.RasterizerState = rasterizerState;
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatchData.Begin(spriteBatch);
        }

        protected virtual void DrawThis(GameTime gameTime, SpriteBatch spriteBatch) { }

        protected virtual void DrawChildren(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var child in children)
            {
                child.Draw(gameTime, spriteBatch);
            }
        }

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
            Parent?.MouseOver(evt);
        }

        public void MouseOut(UIMouseEvent evt)
        {
            IsMouseHovering = false;
            OnMouseOut(evt, this);
            Parent?.MouseOut(evt);
        }

        public void Click(UIMouseEvent evt)
        {
            OnMouseClick(evt, this);
            Parent?.Click(evt);
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
            => BoundingRectangle.Contains(point);

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

        private Rectangle GetClippingRectangle(SpriteBatchData spriteBatchData, Rectangle scissorRectangle)
        {
            var vector = new Vector2(Position.X, Position.Y);
            var position = new Vector2(Width, Height) + vector;
            var matrix = spriteBatchData.GetTransformMatrixOrIdentity();

            vector = Vector2.Transform(vector, matrix);
            position = Vector2.Transform(position, matrix);

            var rectangle = new Rectangle((int)vector.X, (int)vector.Y, (int)(position.X - vector.X), (int)(position.Y - vector.Y));
            var x = MathHelper.Clamp(rectangle.Left, scissorRectangle.Left, scissorRectangle.Right);
            var y = MathHelper.Clamp(rectangle.Top, scissorRectangle.Top, scissorRectangle.Bottom);
            var width = MathHelper.Clamp(rectangle.Right, scissorRectangle.Left, scissorRectangle.Right);
            var height = MathHelper.Clamp(rectangle.Bottom, scissorRectangle.Top, scissorRectangle.Bottom);

            return new(x, y, width - x, height - y);
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
        private static readonly RasterizerState ClippingRasterizerState = new() { CullMode = CullMode.None, ScissorTestEnable = true };
    }
}