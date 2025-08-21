

sampler noiseScrollTexture : register(s1);

float globalTime;
float2 laserDirection;
matrix uWorldViewProjection;
float2 screenPosition;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Position = mul(input.Position, uWorldViewProjection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates.xy;
    float4 color = input.Color;

    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;

    float yFromCenter = distance(coords.y, 0.5);

    float2 adjustedCoords = float2(coords.x * 30, coords.y * 2.2);

    adjustedCoords.x += sin(globalTime * 3 + coords.y * 10) * 0.05;

    float2 movement = float2(globalTime * 1.5, -globalTime * 3);

    float4 tex1 = tex2D(noiseScrollTexture, frac(float2(adjustedCoords.x * 0.8 + movement.x, adjustedCoords.y + movement.y)));
    float4 tex2 = tex2D(noiseScrollTexture, frac(float2(adjustedCoords.x * 1.1 + movement.x * 0.9, adjustedCoords.y + movement.y * 1.6)));
    float4 tex3 = tex2D(noiseScrollTexture, frac(float2(adjustedCoords.x * 1.3 + movement.x * 1.5, adjustedCoords.y + movement.y * 0.8)));

    float textureMesh = (tex1.r + tex2.r + tex3.r) / 3.0;

    float4 darkColor = float4(0.96, 0.34, 0.04, 1);
    float4 midColor = float4(0.98, 0.95, 0.53, 1);
    float4 lightColor = float4(1, 1, 1, 1);
    
    float split = 0.7;
    float colorLerp = textureMesh.r;
    if (colorLerp < split)
    {
        colorLerp = pow(colorLerp / split, 4);
        color = lerp(darkColor, midColor, colorLerp);
    }
    else
    {
        colorLerp = pow((colorLerp - split) / (1 - split), 7);
        color = lerp(midColor, lightColor, colorLerp);
    }
    
    float opacity = smoothstep(0.55, 0, yFromCenter * 1) * color.a;
    //color += glow * color.a;
    color *= pow(abs(textureMesh), 0.5);
    color *= 0.77;

    return color * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

