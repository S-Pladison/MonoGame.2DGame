#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture2D Texture0;
sampler2D Texture0Sampler = sampler_state
{
	texture = <Texture0>;
};

texture2D Texture1;
sampler2D Texture1Sampler = sampler_state
{
	texture = <Texture1>;
};

float4x4 TransformMatrix;
float2 Resolution;

struct VertexShaderInput
{
	float4 Position : SV_Position0;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position0;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = mul(input.Position, TransformMatrix);
	output.Color = input.Color;
	output.TextureCoordinates = input.TextureCoordinates;
	return output;
}

float4 GetLightColor(VertexShaderOutput input, float2 uvOffset)
{
	return tex2D(Texture1Sampler, input.TextureCoordinates + uvOffset);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 textureColor = tex2D(Texture0Sampler, input.TextureCoordinates);

	float4 lightColor = float4(0, 0, 0, 0);
	float blurSizeX = 2.0f / Resolution.x;
	float blurSizeY = 2.0f / Resolution.y;

	lightColor += GetLightColor(input, float2(0, -3.0f * blurSizeY)) * 0.15f;
	lightColor += GetLightColor(input, float2(0, -2.0f * blurSizeY)) * 0.18f;
	lightColor += GetLightColor(input, float2(0, -blurSizeY)) * 0.25f;
	lightColor += GetLightColor(input, float2(0, blurSizeY)) * 0.25f;
	lightColor += GetLightColor(input, float2(0, 2.0f * blurSizeY)) * 0.18f;
	lightColor += GetLightColor(input, float2(0, 3.0f * blurSizeY)) * 0.15f;

	lightColor += GetLightColor(input, float2(-3.0f * blurSizeX, 0)) * 0.15f;
	lightColor += GetLightColor(input, float2(-2.0f * blurSizeX, 0)) * 0.18f;
	lightColor += GetLightColor(input, float2(-blurSizeX, 0)) * 0.17f;
	lightColor += GetLightColor(input, float2(blurSizeX, 0)) * 0.17f;
	lightColor += GetLightColor(input, float2(2.0f * blurSizeX, 0)) * 0.18f;
	lightColor += GetLightColor(input, float2(3.0f * blurSizeX, 0)) * 0.15f;

	return textureColor * input.Color + lightColor * 2.0f * textureColor.a;
}

technique Tile
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};