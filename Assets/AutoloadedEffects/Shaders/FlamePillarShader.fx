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
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float4 color = input.Color;
    
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float yFromCenter = distance(coords.y, 0.5);
    
    float2 adjustedCoords = float2(coords.x * 40, coords.y * 2.5);
    adjustedCoords.x += (0.5 - yFromCenter) / 3;
    float2 movement = float2(globalTime * 3, -globalTime * 6);
    float4 tex1 = tex2D(noiseScrollTexture, float2(frac(adjustedCoords.x * 0.7 + movement.x), adjustedCoords.y + movement.y));
    float4 tex2 = tex2D(noiseScrollTexture, float2(frac(adjustedCoords.x * 0.7 + movement.x * 0.93), adjustedCoords.y + movement.y * 1.72));
    float4 tex3 = tex2D(noiseScrollTexture, float2(frac(adjustedCoords.x * 0.7 + movement.x * 1.6), adjustedCoords.y + movement.y * 0.7));
    float4 textureMesh = tex1 * 0.33 + tex2 * 0.33 + tex3 * 0.33;
    
    float4 darkColor = float4(0.96, 0.34, 0.04, 1); //red
    float4 midColor = float4(0.98, 0.95, 0.53, 1); //yellow
    float4 lightColor = float4(1, 1, 1, 1); //white
    
    color = lerp(darkColor, midColor, textureMesh.r);
    
    float opacity = smoothstep(0.55, 0, yFromCenter * 1) * color.a;
    //color += glow * color.a;
    color *= pow(abs(textureMesh), 0.25);
    color *= 1.6;

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

