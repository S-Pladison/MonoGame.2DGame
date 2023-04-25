#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture2D Normal;
sampler2D NormalSampler = sampler_state
{
	texture = <Normal>;
};

float4x4 TransformMatrix;
float4 Color;
float2 Resolution;
float2 Center;
float Radius;

struct VertexShaderInput
{
	float4 Position : SV_Position0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position0;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = mul(input.Position, TransformMatrix);
	output.TextureCoordinates = input.TextureCoordinates;
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.TextureCoordinates * Resolution;
	float2 vectToLight = Center - uv;
	float dist = 1.0 - length(vectToLight) / Radius;

	return Color * dist;
}

technique Light
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};