#if OPENGL
  #define VS_SHADERMODEL vs_3_0
  #define PS_SHADERMODEL ps_3_0
#else
  #define VS_SHADERMODEL vs_5_0
  #define PS_SHADERMODEL ps_5_0
#endif

#define DECLARE_TEXTURE(Name, index) \
    Texture2D<float4> Name : register(t##index); \
    sampler Name##Sampler : register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord) Name.Sample(Name##Sampler, texCoord)

// Camera settings.
float4x4 World;
float4x4 View;
float4x4 Projection;


// This sample uses a simple Lambert lighting model.
float3 LightDirection = normalize(float3(0, -1, 0.75));
float3 DiffuseLight = 1.25;
float3 AmbientLight = 0.25;

float HighlightGridSize = 20;
float HighlightOffset = 0;

DECLARE_TEXTURE(Texture, 0);

struct VertexShaderInput
{
    float4 Position : SV_Position;
    float3 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};


struct VertexShaderOutput
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
    float4 FractionColor : TEXCOORD4;
    int Effect : TEXCOORD5;
};


// Vertex shader helper function shared between the two techniques.
VertexShaderOutput VertexShaderCommon(VertexShaderInput input, float4x4 instanceTransform, float4 fractionColor, int effect)
{
    VertexShaderOutput output;

    // Apply the world and camera matrices to compute the output position.
    float4 worldPosition = mul(input.Position, instanceTransform);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // Compute lighting, using a simple Lambert model.
    float3 worldNormal = (float3) mul(float4(input.Normal, 0.0), instanceTransform);
    
    float diffuseAmount = max(-dot(worldNormal, LightDirection), 0);
    
    float3 lightingResult = saturate(diffuseAmount * DiffuseLight + AmbientLight);
    
    output.Color = float4(lightingResult, 1);

    // Copy across the input texture coordinate.
    output.TextureCoordinate = input.TextureCoordinate;

    output.Effect = effect;

    output.FractionColor = fractionColor;

    return output;
}


// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
VertexShaderOutput HardwareInstancingVertexShader(VertexShaderInput input, float4x4 instanceTransform : BLENDWEIGHT, float4 fractionColor : TEXCOORD4, int effect : TEXCOORD5)
{
    return VertexShaderCommon(input, mul(World, transpose(instanceTransform)), fractionColor, effect);
}

bool ShouldHighlight(float coord)
{
    return (coord + HighlightOffset) % HighlightGridSize < HighlightGridSize / 2;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 sampledTexture = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
    if (sampledTexture.x == 1 && sampledTexture.y == 1 && sampledTexture.z == 1) {
        sampledTexture = input.FractionColor;
    }
    if (input.Effect == 1)
    { 
        if (ShouldHighlight(input.Position.x) != ShouldHighlight(input.Position.y))
        {
            return float4(0, 0, 0, 1);
        }
    }
    else if (input.Effect == 2)
    {
        return sampledTexture * input.Color * float4(1, 0, 0, 1);
    }
    return sampledTexture * input.Color;
}

technique HardwareInstancing
{
    pass ModelRender
    {
        VertexShader = compile VS_SHADERMODEL HardwareInstancingVertexShader();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
