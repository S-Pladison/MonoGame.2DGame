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

float4x4 TransformMatrix;
float2 Texture0Size;

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
	float color = 0.0f;
	float blurSizeX = 1.0f / Texture0Size.x;
	float blurSizeY = 1.0f / Texture0Size.y;

	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x, input.TextureCoordinates.y - 3.0 * blurSizeY)).a * 0.09f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x, input.TextureCoordinates.y - 2.0 * blurSizeY)).a * 0.11f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x, input.TextureCoordinates.y - blurSizeY)).a * 0.18f;
	color += tex2D(Texture0Sampler, input.TextureCoordinates) * 0.24f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x, input.TextureCoordinates.y + blurSizeY)).a * 0.18f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x, input.TextureCoordinates.y + 2.0 * blurSizeY)).a * 0.11f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x, input.TextureCoordinates.y + 3.0 * blurSizeY)).a * 0.09f;

	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x - 3.0 * blurSizeX, input.TextureCoordinates.y)).a * 0.09f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x - 2.0 * blurSizeX, input.TextureCoordinates.y)).a * 0.11f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x - blurSizeX, input.TextureCoordinates.y)).a * 0.18f;
	color += tex2D(Texture0Sampler, input.TextureCoordinates) * 0.24f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x + blurSizeX, input.TextureCoordinates.y)).a * 0.18f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x + 2.0 * blurSizeX, input.TextureCoordinates.y)).a * 0.11f;
	color += tex2D(Texture0Sampler, float2(input.TextureCoordinates.x + 3.0 * blurSizeX, input.TextureCoordinates.y)).a * 0.09f;

	return color * input.Color;
}

technique TileEdgeShadow
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};