#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler textureSampler0 : register(s0);

texture Texture1  : register(s1);
sampler textureSampler1 = sampler_state
{
    texture = <Texture1>;
    AddressU = Wrap;
    AddressV = Wrap;
    AddressW = Wrap;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
};

texture Texture2 : register(s2);
sampler textureSampler2 = sampler_state
{
    texture = <Texture2>;
    AddressU = Wrap;
    AddressV = Wrap;
    AddressW = Wrap;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
};

float Time;
float2 Resolution;
float2 Offset;
float4 Color0;
float4 Color1;
float4 Color2;

float4 Background(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    
    return tex2D(textureSampler0, uv) * sampleColor;
}

technique Technique1
{
    pass Background
    {
        PixelShader = compile PS_SHADERMODEL Background();
    }
}