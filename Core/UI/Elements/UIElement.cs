using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI.Events;
using Pladi.Utilities;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;

namespace Pladi.Core.UI.Elements
{
    public class UIElement
    {
        public delegate void ResolutionChangeEvent(UIResolutionChangeEvent evt, UIElement affectedElement);
        public delegate void MouseEvent(UIMouseEvent evt, UIElement listeningElement);
        public delegate void ElementEvent(UIElement affectedElement);

        // ...

        public bool ReqClippingOutsideRectangle;

        protected List<UIElement> children;

        public PositionStyle Left;
        public PositionStyle Top;
        public PositionStyle Width;
        public PositionStyle Height;

        public Vector2 Size;
        public Vector2 Position;

        // ...

        public IReadOnlyCollection<UIElement> Children { get => children; }
        public UIElement Parent { get; private set; }
        public bool IsMouseHovering { get; protected set; }
        public RectangleF HitboxRectangle { get => new(Position.X, Position.Y, Size.X, Size.Y); }
        public RectangleF ClippingOutsideRectangle
        {
            get
            {
                var vector = Position + Size;
                return new() {
                    X = Position.X,
                    Y = Position.Y,
                    Width = MathF.Max(vector.X - Position.X, 0.0f),
                    Height = MathF.Max(vector.Y - Position.Y, 0.0f)
                };
            }
        }

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

            if (!ReqClippingOutsideRectangle)
            {
                DrawChildren(gameTime, spriteBatch);
                return;
            }

            spriteBatch.End();

            var spriteBatchData = new SpriteBatchData(spriteBatch);
            var rasterizerState = spriteBatchData.RasterizerState;
            var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatchData.RasterizerState = ClippingRasterizerState;
            spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(scissorRectangle, ClippingOutsideRectangle.ToRectangle());
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
            float parentPosX, parentPosY, parentSizeX, parentSizeY;

            if (Parent is null)
            {
                parentPosX = 0;
                parentPosY = 0;
                parentSizeX = Main.ScreenSize.X;
                parentSizeY = Main.ScreenSize.Y;
            }
            else
            {
                parentPosX = Parent.Position.X;
                parentPosY = Parent.Position.Y;
                parentSizeX = Parent.Size.X;
                parentSizeY = Parent.Size.Y;
            }

            Position.X = parentPosX + Left.GetPixelBaseParent(parentSizeX);
            Position.Y = parentPosY + Top.GetPixelBaseParent(parentSizeY);
            Size.X = Width.GetPixelBaseParent(parentSizeX);
            Size.Y = Height.GetPixelBaseParent(parentSizeY);

            foreach (var child in children)
            {
                child.Recalculate();
            }
        }

        public virtual bool ContainsPoint(Vector2 point)
            => HitboxRectangle.Contains(point);

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
        private static readonly RasterizerState ClippingRasterizerState = new() { CullMode = CullMode.None, ScissorTestEnable = true };
    }
}