﻿using Microsoft.Xna.Framework;
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

        protected List<UIElement> children;
        protected Vector2 position;
        protected Vector2 size;

        public StyleDimension Left;
        public StyleDimension Top;
        public StyleDimension Width;
        public StyleDimension Height;
        public float HorizontalAlign;
        public float VerticalAlign;

        public bool ClippingOutsideRectangle;

        // ...

        public IReadOnlyCollection<UIElement> Children { get => children; }
        public UIElement Parent { get; private set; }
        public bool IsMouseHovering { get; private set; }
        public RectangleF Dimensions { get => new(position.X, position.Y, size.X, size.Y); }

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

        private void UpdateChildren(GameTime gameTime)
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
            spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(scissorRectangle, Dimensions.ToRectangle());
            spriteBatchData.Begin(spriteBatch);

            DrawChildren(gameTime, spriteBatch);

            spriteBatch.End();
            spriteBatchData.RasterizerState = rasterizerState;
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatchData.Begin(spriteBatch);
        }

        protected virtual void DrawThis(GameTime gameTime, SpriteBatch spriteBatch) { }

        private void DrawChildren(GameTime gameTime, SpriteBatch spriteBatch)
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

        public void Recalculate()
        {
            RecalculateThis();
            RecalculateChildren();
        }

        private void RecalculateThis()
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
                parentPosX = Parent.position.X;
                parentPosY = Parent.position.Y;
                parentSizeX = Parent.size.X;
                parentSizeY = Parent.size.Y;
            }

            position.X = parentPosX + Left.GetPixelBaseParent(parentSizeX);
            position.Y = parentPosY + Top.GetPixelBaseParent(parentSizeY);
            size.X = Width.GetPixelBaseParent(parentSizeX);
            size.Y = Height.GetPixelBaseParent(parentSizeY);
        }

        private void RecalculateChildren()
        {
            foreach (var child in children)
            {
                child.Recalculate();
            }
        }

        public bool ContainsPoint(Vector2 point)
            => Dimensions.Contains(point);

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