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
	AddressU = Wrap;
	AddressV = Wrap;
	AddressW = Wrap;
	MagFilter = Point;
	MinFilter = Point;
	Mipfilter = Point;
};

float4x4 TransformMatrix;
float Power;


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

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 textureColor = tex2D(Texture0Sampler, input.TextureCoordinates);
	float4 lightColor = tex2D(Texture1Sampler, input.TextureCoordinates);

	if (!any(textureColor)) return textureColor;

	return textureColor * input.Color + lightColor * Power * textureColor.a;
}

technique He
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};