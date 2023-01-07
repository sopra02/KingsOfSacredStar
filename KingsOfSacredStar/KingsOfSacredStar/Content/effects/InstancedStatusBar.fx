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

float4 Color;

struct VertexShaderInput
{
    float4 Position : SV_Position;
	float3 InstancedPosition : TEXCOORD0;
	float HealthRatio : TEXCOORD1;
};


struct VertexShaderOutput
{
    float4 Position : SV_Position;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4x4 healthScale = {
        input.HealthRatio * 8, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1
    };
    
    float4 rawWorldPosition = mul(World, input.Position);
    // Apply the world and camera matrices to compute the output position.
    float4 worldPosition = mul(healthScale, rawWorldPosition);

    float4 instancedWorldPosition = worldPosition + float4(input.InstancedPosition, 0);
    float4 viewPosition = mul(instancedWorldPosition, View);
    output.Position = mul(viewPosition, Projection);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return Color;
}

technique HardwareInstancing
{
    pass ModelRender
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
