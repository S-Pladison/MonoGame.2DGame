using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace Pladi.Utilities.DataStructures
{
    public struct SpriteBatchData
    {
        // [private static properties and fields]

        private static readonly FieldInfo sortModeField;
        private static readonly FieldInfo blendStateField;
        private static readonly FieldInfo samplerStateField;
        private static readonly FieldInfo depthStencilStateField;
        private static readonly FieldInfo rasterizerStateField;
        private static readonly FieldInfo effectField;
        private static readonly FieldInfo spriteEffectField;

        // [static constructors]

        static SpriteBatchData()
        {
            var type = typeof(SpriteBatch);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            sortModeField = type.GetField("_sortMode", flags);
            blendStateField = type.GetField("_blendState", flags);
            samplerStateField = type.GetField("_samplerState", flags);
            depthStencilStateField = type.GetField("_depthStencilState", flags);
            rasterizerStateField = type.GetField("_rasterizerState", flags);
            effectField = type.GetField("_effect", flags);
            spriteEffectField = type.GetField("_spriteEffect", flags);
        }

        // [public properties and fields]

        public SpriteSortMode SortMode;
        public BlendState BlendState;
        public SamplerState SamplerState;
        public DepthStencilState DepthStencilState;
        public RasterizerState RasterizerState;
        public Effect Effect;

        // [private properties and fields]

        private SpriteEffect spriteEffect;

        // [constructors]

        public SpriteBatchData(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));

            spriteEffect = (SpriteEffect)spriteEffectField.GetValue(spriteBatch);

            SortMode = (SpriteSortMode)sortModeField.GetValue(spriteBatch);
            BlendState = (BlendState)blendStateField.GetValue(spriteBatch);
            SamplerState = (SamplerState)samplerStateField.GetValue(spriteBatch);
            DepthStencilState = (DepthStencilState)depthStencilStateField.GetValue(spriteBatch);
            RasterizerState = (RasterizerState)rasterizerStateField.GetValue(spriteBatch);
            Effect = (Effect)effectField.GetValue(spriteBatch);
        }

        // [public methods]

        public void Begin(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, spriteEffect.TransformMatrix);
        }

        public Matrix? GetTransformMatrix()
            => spriteEffect.TransformMatrix;

        public Matrix GetTransformMatrixOrIdentity()
            => spriteEffect.TransformMatrix ?? Matrix.Identity;
    }
}
