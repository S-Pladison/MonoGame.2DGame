using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core.Graphics.Renderers;
using Pladi.Core.Tiles;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;

namespace Pladi.Core.Graphics.Lighting
{
    public class LightRenderer : Renderer
    {
        private static readonly float scale;
        private static readonly Matrix scaleMatrix;

        private static readonly short[] lightIndices;

        private static readonly BlendState shadowBlendState;
        private static readonly BlendState lightBlendState;

        // ...

        static LightRenderer()
        {
            scale = 2.5f;
            scaleMatrix = Matrix.CreateScale(scale);

            lightIndices = new short[] { 0, 1, 2, 3 };

            shadowBlendState = new BlendState()
            {
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.Zero
            };

            lightBlendState = new BlendState()
            {
                ColorSourceBlend = Blend.InverseDestinationAlpha,
                ColorDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.Zero
            };
        }

        // ...

        public override Point Size => new((int)(base.Size.X / scale), (int)(base.Size.Y / scale));

        private readonly VertexPositionTexture[] lightVertices;
        private RenderTarget2D tempTarget;

        private VertexBuffer staticVertexBuffer;
        private IndexBuffer staticIndexBuffer;
        private int staticPrimitiveCount;

        private DynamicVertexBuffer dynamicVertexBuffer;
        private DynamicIndexBuffer dynamicIndexBuffer;
        private int dynamicMaxPrimitiveCount;
        private int dynamicPrimitiveCount;

        // ...

        public LightRenderer()
        {
            lightVertices = new VertexPositionTexture[4];

            lightVertices[0].TextureCoordinate = Vector2.Zero;
            lightVertices[1].TextureCoordinate = Vector2.UnitX;
            lightVertices[2].TextureCoordinate = Vector2.UnitY;
            lightVertices[3].TextureCoordinate = Vector2.One;
        }

        // ...

        protected override void OnLoad()
        {
            OnRecreateRenderTarget += (device, width, height) =>
            {
                tempTarget = new(device, Size.X, Size.Y, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            };
        }

        protected override void OnRender(SpriteBatch spriteBatch)
        {
            if (extraData is not (IList<Light> lights, TileLayer collisionLayer)) return;

            var camera = ILoadable.GetInstance<CameraComponent>();
            var oldBlendState = spriteBatch.GraphicsDevice.BlendState;
            var oldSamplerState = spriteBatch.GraphicsDevice.SamplerStates[0];

            spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            PrepareEffects(camera);

            /*if (vertexBuffer is null)
            {
                var edges = collisionLayer.GetEdgesFromTiles(camera.VisibleArea.ToRectangleF());

                vertexBuffer = new DynamicVertexBuffer(spriteBatch.GraphicsDevice, typeof(VertexPosition), edges.Length * 4 * 10, BufferUsage.WriteOnly);

                var vertices = new VertexPosition[edges.Length * 4 * 10];

                for (int j = 0; j < edges.Length; j++)
                {
                    ref var a = ref edges[j].A;
                    ref var b = ref edges[j].B;

                    vertices[0 + j].Position = new Vector3(b.X, b.Y, 0);
                    vertices[1 + j].Position = new Vector3(b.X, b.Y, 1);
                    vertices[2 + j].Position = new Vector3(a.X, a.Y, 0);
                    vertices[3 + j].Position = new Vector3(a.X, a.Y, 1);
                }

                vertexBuffer.SetData(vertices);

                count = edges.Length;
            }*/




            // ...

            lightVertices[0].Position = new Vector3(0, 0, 0);
            lightVertices[1].Position = new Vector3(Size.X, 0, 0);
            lightVertices[2].Position = new Vector3(0, Size.Y, 0);
            lightVertices[3].Position = new Vector3(Size.X, Size.Y, 0);

            for (int i = 0; i < lights.Count; i++)
            {
                RenderLight(spriteBatch, lights[i], camera, collisionLayer);
            }

            if (ILoadable.GetInstance<TileRenderer>().TryGetTargetIfPrepared(out RenderTarget2D tileTarget)
                && ILoadable.GetInstance<EntityRenderer>().TryGetTargetIfPrepared(out RenderTarget2D entityTarget))
            {
                spriteBatch.GraphicsDevice.SetRenderTarget(tempTarget);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Matrix.Identity);
                spriteBatch.Draw(RenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(tileTarget, Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, 1f / scale, SpriteEffects.None, 0);
                spriteBatch.End();

                var effect = EffectAssets.TileEdgeShadow;

                effect.Parameters["TransformMatrix"].SetValue(camera.ProjectionMatrix);
                effect.Parameters["Texture0Size"].SetValue(new Vector2(tileTarget.Width, tileTarget.Height));

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, effect, Matrix.Identity);
                spriteBatch.Draw(tileTarget, Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(entityTarget, Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.End();

                (tempTarget, RenderTarget) = (RenderTarget, tempTarget);
            }

            // ...

            spriteBatch.GraphicsDevice.SamplerStates[0] = oldSamplerState;
            spriteBatch.GraphicsDevice.BlendState = oldBlendState;
        }

        // ...

        public void PrepareStaticShadowVertices(EdgeF[] edges)
        {
            if (edges is null)
            {
                staticVertexBuffer = null;
                staticIndexBuffer = null;
                staticPrimitiveCount = 0;
                return;
            }

            staticVertexBuffer = new DynamicVertexBuffer(Main.SpriteBatch.GraphicsDevice, typeof(VertexPosition), edges.Length * 4, BufferUsage.WriteOnly);
            staticIndexBuffer = new DynamicIndexBuffer(Main.SpriteBatch.GraphicsDevice, IndexElementSize.SixteenBits, edges.Length * 6, BufferUsage.WriteOnly);

            var vertices = new VertexPosition[edges.Length * 4];
            var indices = new short[edges.Length * 6];

            for (int j = 0; j < edges.Length; j++)
            {
                ref var a = ref edges[j].B;
                ref var b = ref edges[j].A;

                vertices[0 + j * 4].Position = new Vector3(b.X, b.Y, 0);
                vertices[1 + j * 4].Position = new Vector3(b.X, b.Y, 1);
                vertices[2 + j * 4].Position = new Vector3(a.X, a.Y, 0);
                vertices[3 + j * 4].Position = new Vector3(a.X, a.Y, 1);

                indices[0 + j * 6] = (short)(j * 4 + 1);
                indices[1 + j * 6] = (short)(j * 4 + 0);
                indices[2 + j * 6] = (short)(j * 4 + 2);

                indices[3 + j * 6] = (short)(j * 4 + 1);
                indices[4 + j * 6] = (short)(j * 4 + 2);
                indices[5 + j * 6] = (short)(j * 4 + 3);
            }

            staticVertexBuffer.SetData(vertices);
            staticIndexBuffer.SetData(indices);
            staticPrimitiveCount = edges.Length * 2;
        }

        public void PrepareDynamicBuffers(int maxShadowCount)
        {
            dynamicMaxPrimitiveCount = maxShadowCount * 2;
            dynamicVertexBuffer = new DynamicVertexBuffer(Main.SpriteBatch.GraphicsDevice, typeof(VertexPosition), maxShadowCount * 4, BufferUsage.WriteOnly);
            dynamicIndexBuffer = new DynamicIndexBuffer(Main.SpriteBatch.GraphicsDevice, IndexElementSize.SixteenBits, maxShadowCount * 6, BufferUsage.WriteOnly);
        }

        public void Seeee(List<EdgeF> edges)
        {
            var vertices = new VertexPosition[edges.Count * 4];
            var indices = new short[edges.Count * 6];

            for (int j = 0; j < edges.Count; j++)
            {
                var a = edges[j].A;
                var b = edges[j].B;

                vertices[0 + j * 4].Position = new Vector3(b.X, b.Y, 0);
                vertices[1 + j * 4].Position = new Vector3(b.X, b.Y, 1);
                vertices[2 + j * 4].Position = new Vector3(a.X, a.Y, 0);
                vertices[3 + j * 4].Position = new Vector3(a.X, a.Y, 1);

                indices[0 + j * 6] = (short)(j * 4 + 1);
                indices[1 + j * 6] = (short)(j * 4 + 0);
                indices[2 + j * 6] = (short)(j * 4 + 2);

                indices[3 + j * 6] = (short)(j * 4 + 1);
                indices[4 + j * 6] = (short)(j * 4 + 2);
                indices[5 + j * 6] = (short)(j * 4 + 3);
            }

            dynamicVertexBuffer.SetData(vertices);
            dynamicIndexBuffer.SetData(indices);
            dynamicPrimitiveCount = edges.Count * 2;
        }

        private void PrepareEffects(CameraComponent camera)
        {
            var e = EffectAssets.Shadow;
            e.Parameters["TransformMatrix"].SetValue(camera.TransformMatrix);

            e = EffectAssets.Light;
            e.Parameters["TransformMatrix"].SetValue(scaleMatrix * camera.ProjectionMatrix);
            e.Parameters["Resolution"].SetValue(scale * Size.ToVector2());
        }

        private void RenderLight(SpriteBatch spriteBatch, Light light, CameraComponent camera, TileLayer collisionLayer)
        {
            /*spriteBatch.GraphicsDevice.BlendState = shadowBlendState;

            var effect = EffectAssets.Shadow;
            effect.Parameters["LightPosition"].SetValue(light.Position);

            var edges = collisionLayer.GetEdgesFromTiles(RectangleF.Intersect(light.VisibleArea, camera.VisibleArea.ToRectangleF()));

            for (int j = 0; j < edges.Length; j++)
            {
                ref var a = ref edges[j].A;
                ref var b = ref edges[j].B;

                shadowVertices[0].Position = new Vector3(b.X, b.Y, 0);
                shadowVertices[1].Position = new Vector3(b.X, b.Y, 1);
                shadowVertices[2].Position = new Vector3(a.X, a.Y, 0);
                shadowVertices[3].Position = new Vector3(a.X, a.Y, 1);

                for (int k = 0; k < effect.CurrentTechnique.Passes.Count; k++)
                {
                    effect.CurrentTechnique.Passes[k].Apply();
                    spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, shadowVertices, 0, 4, indeces, 0, 2);
                }
            }*/

            spriteBatch.GraphicsDevice.BlendState = shadowBlendState;

            if (staticPrimitiveCount > 0)
            {
                //spriteBatch.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                var e = EffectAssets.Shadow;
                e.Parameters["LightPosition"].SetValue(light.Position);

                spriteBatch.GraphicsDevice.SetVertexBuffer(staticVertexBuffer);
                spriteBatch.GraphicsDevice.Indices = staticIndexBuffer;

                for (int k = 0; k < e.CurrentTechnique.Passes.Count; k++)
                {
                    e.CurrentTechnique.Passes[k].Apply();
                    spriteBatch.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, staticPrimitiveCount * 3, 0, staticPrimitiveCount);
                }
            }

            if (dynamicPrimitiveCount > 0)
            {
                var e = EffectAssets.Shadow;
                e.Parameters["LightPosition"].SetValue(light.Position);

                spriteBatch.GraphicsDevice.SetVertexBuffer(dynamicVertexBuffer);
                spriteBatch.GraphicsDevice.Indices = dynamicIndexBuffer;

                for (int k = 0; k < e.CurrentTechnique.Passes.Count; k++)
                {
                    e.CurrentTechnique.Passes[k].Apply();
                    spriteBatch.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, dynamicPrimitiveCount * 3, 0, dynamicPrimitiveCount);
                }
            }

        DrawLight:

            spriteBatch.GraphicsDevice.BlendState = lightBlendState;

            var effect = EffectAssets.Light;
            effect.Parameters["Color"].SetValue(light.Color.ToVector4());
            effect.Parameters["Center"].SetValue(Vector2.Transform(light.Position, camera.ViewMatrix));
            effect.Parameters["Radius"].SetValue(light.Radius);

            for (int k = 0; k < effect.CurrentTechnique.Passes.Count; k++)
            {
                effect.CurrentTechnique.Passes[k].Apply();
                spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, lightVertices, 0, 4, lightIndices, 0, 2);
            }
        }
    }
}