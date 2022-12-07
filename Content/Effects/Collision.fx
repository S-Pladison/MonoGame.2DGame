#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 OutlineColor;
float2 Offset;
float2 Texture1UvMult;
float OutlineWidth;
float OutlineHeight;

texture2D Texture0;
sampler2D Texture0Sampler = sampler_state
{
	texture = <Texture0>;
};

texture2D Texture1;
sampler2D Texture1Sampler = sampler_state
{
	texture = <Texture1>;
	AddressU = Wrap;
	AddressV = Wrap;
	AddressW = Wrap;
	MagFilter = Point;
	MinFilter = Point;
	Mipfilter = Point;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

// https://github.com/danny88881/Tutorials/blob/master/Pixel_Outline_Shader/outline.shader

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float sizeX = OutlineWidth;
	float sizeY = OutlineHeight;
	float2 uv = input.TextureCoordinates;
	float4 texColor = tex2D(Texture0Sampler, uv);
	float alpha = -4.0 * texColor.a;

	alpha += tex2D(Texture0Sampler, uv + float2(sizeX, 0)).a;
	alpha += tex2D(Texture0Sampler, uv + float2(-sizeX, 0)).a;
	alpha += tex2D(Texture0Sampler, uv + float2(0, sizeY)).a;
	alpha += tex2D(Texture0Sampler, uv + float2(0, -sizeY)).a;

	float4 finalColor = lerp(texColor, OutlineColor, clamp(alpha, 0, 1));

	if (!any(texColor))
	{
		return float4(finalColor.rgb, clamp(abs(alpha) + texColor.a, 0, 1));
	}

	float2 uv2 = (uv - 0.5);
	uv2 *= Texture1UvMult;
	uv2 += 0.5;

	return tex2D(Texture1Sampler, uv2 + Offset);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};