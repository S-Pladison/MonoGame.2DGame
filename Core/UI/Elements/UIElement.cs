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

        public bool ClippingOutsideRectangle;

        protected List<UIElement> children;
        protected Vector2 position;
        protected float width;
        protected float height;
        protected RectangleF boundingRectangle;

        // ...

        public IReadOnlyCollection<UIElement> Children { get => children; }
        public UIElement Parent { get; private set; }
        public Vector2 Size { get => new(width, height); }
        public bool IsMouseHovering { get; protected set; }

        // ...

        public UIElement()
        {
            children = new();
        }

        // ...

        public void SetPosition(float x, float y)
        {
            position.X = x;
            position.Y = y;

            Recalculate();
        }

        public void SetRectangle(RectangleF rectangle)
            => SetRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        public void SetRectangle(float x, float y, float width, float height)
        {
            position.X = x;
            position.Y = y;

            this.width = width;
            this.height = height;

            Recalculate();
        }

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
            element.Recalculate();
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

        public virtual void MouseOver(UIMouseEvent evt)
        {
            IsMouseHovering = true;
            OnMouseOver(evt, this);
            Parent?.MouseOver(evt);
        }

        public virtual void MouseOut(UIMouseEvent evt)
        {
            IsMouseHovering = false;
            OnMouseOut(evt, this);
            Parent?.MouseOut(evt);
        }

        public virtual void Click(UIMouseEvent evt)
        {
            OnMouseClick(evt, this);
            Parent?.Click(evt);
        }

        public virtual void ResolutionChanged(UIResolutionChangeEvent evt)
        {
            OnResolutionChanged(evt, this);

            foreach (var child in children)
            {
                child.ResolutionChanged(evt);
            }
        }

        public virtual void Recalculate()
        {
            var parrentPosition = Parent?.boundingRectangle.Location ?? Vector2.Zero;

            boundingRectangle.X = position.X + parrentPosition.X;
            boundingRectangle.Y = position.Y + parrentPosition.Y;
            boundingRectangle.Width = width;
            boundingRectangle.Height = height;

            foreach (var child in children)
            {
                child.Recalculate();
            }
        }

        public virtual bool ContainsPoint(Vector2 point)
            => boundingRectangle.Contains(point);

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
            var vector = this.position;
            var position = new Vector2(this.width, this.height) + vector;
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