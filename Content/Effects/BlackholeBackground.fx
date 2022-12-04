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

float4 BlackholeBackground(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float4 main = tex2D(textureSampler0, uv);
    main.a = main.r;
    main.rgb = sampleColor.rgb;
    uv.x *= (Resolution.x / Resolution.y);
    
    float4 noiseColor = tex2D(textureSampler2, uv + float2(0, -Time * 0.005f) + Offset);
    noiseColor.rgb *= Color1.rgb;
    
    float4 starColor = float4(0, 0, 0, 0);
    starColor = tex2D(textureSampler1, uv * 4 + float2(-0.3f, -Time * 0.04f) + Offset) * 0.3f;
    starColor += tex2D(textureSampler1, uv * 2.5f + float2(0, -Time * 0.1f) + Offset * 1.3f) * 0.65f;
    
    float4 color = noiseColor;
    noiseColor = tex2D(textureSampler2, uv * 2 + float2(0.3f, -Time * 0.03f) + Offset * 1.3f);
    noiseColor.rgb *= Color2.rgb;
    
    color += noiseColor;   
    color.rgb += starColor.rgb;
    color.a = main.a;
    
    return color;
}

technique Technique1
{
    pass BlackholeBackground
    {
        PixelShader = compile ps_2_0 BlackholeBackground();
    }
}