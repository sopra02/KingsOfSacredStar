#if OPENGL
  #define VS_SHADERMODEL vs_3_0
  #define PS_SHADERMODEL ps_3_0
#else
  #define VS_SHADERMODEL vs_5_0
  #define PS_SHADERMODEL ps_5_0
#endif

// Camera settings.
float4x4 World;
float4x4 View;
float4x4 Projection;

float4 Color = float4(1, 0, 0, 1);

struct VertexShaderInput
{
    float4 Position : SV_Position;
	float2 TextureCoordinate : TEXCOORD0;
};


struct VertexShaderOutput
{
    float4 Position : SV_Position;
	float2 TextureCoordinate : TEXCOORD0;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TextureCoordinate = input.TextureCoordinate;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 center = float2(0.5, 0.5);
    float dist = distance(input.TextureCoordinate, center);
    float distFromEdge = 0.5 - dist;
    float thresholdWidth = 0.005;
    float antialiasedCircle = saturate((distFromEdge / thresholdWidth) + 0.5);
    return lerp(float4(0, 0, 0, 0), Color, antialiasedCircle);
}

technique HardwareInstancing
{
    pass ModelRender
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
