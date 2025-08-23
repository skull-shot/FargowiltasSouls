

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

    // Hotter yellow-orange palette (less red)
    float4 darkColor = float4(1.0, 0.45, 0.05, 1); // orange base
    float4 midColor = float4(1.0, 0.75, 0.15, 1); // bright yellow-orange
    float4 hotColor = float4(1.0, 0.9, 0.6, 1); // soft yellow-white
    float4 coreColor = float4(1.0, 1.0, 1.0, 1); // pure white

    float split = 0.65;
    float colorLerp = textureMesh;

    if (colorLerp < split)
    {
        colorLerp = pow(colorLerp / split, 3.0);
        color = lerp(darkColor, midColor, colorLerp);
    }
    else if (colorLerp < 0.9)
    {
        colorLerp = pow((colorLerp - split) / (0.9 - split), 2.5);
        color = lerp(midColor, hotColor, colorLerp);
    }
    else
    {
        colorLerp = pow((colorLerp - 0.9) / 0.1, 4.0);
        color = lerp(hotColor, coreColor, colorLerp);
    }

    // Opacity shaping
    float opacity = smoothstep(0.55, 0.0, yFromCenter) * color.a;

    // Sharper flame tongues and brightness boost
    color *= pow(abs(textureMesh), 0.6);
    color.rgb *= 1.35;

    return color * opacity * 0.7;
}



technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

