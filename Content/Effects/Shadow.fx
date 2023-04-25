#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 TransformMatrix;
float2 LightPosition;

struct VertexShaderInput
{
	float4 Position : SV_Position0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = mul(float4(input.Position.xy - input.Position.z * LightPosition, 0, 1 - input.Position.z), TransformMatrix);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return float4(0, 0, 0, 1);
}

technique Shadow
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};