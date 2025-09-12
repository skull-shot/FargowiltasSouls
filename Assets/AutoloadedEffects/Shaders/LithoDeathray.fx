sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

float globalTime;
float realopacity;
float4 mainColor;
float4 secondColor;
matrix uWorldViewProjection;

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
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates.xy;

    float waveTime = sin(15.0 * globalTime - 15.2 * coords.x);
    float y = waveTime * 0.3;

    float dy = (coords.y - 0.5) / input.TextureCoordinates.z;
    coords.y = dy + 0.5;

    float widthScale = (y + (1.0 - coords.x * 0.25)) * 0.5;
    if (coords.x < 0.15)
        widthScale /= sqrt(coords.x / 0.15);

    coords.y = dy * clamp(widthScale, 0.0, 2.0) + 0.5;

    float4 map1 = tex2D(
        uImage1,
        float2(frac(coords.x * 9.0 - globalTime * 3.6), coords.y)
    );
    float4 map2 = tex2D(
        uImage1,
        float2(frac(coords.x * 9.0 - globalTime * 5.7), coords.y)
    );
    float4 fadeMapColor = (map1 + map2) / 2;

    float opacity = pow(fadeMapColor.r, 0.5);
    
    opacity *= waveTime * 0.15 + 0.85; // approximate 0.7+0.3*((waveTime+1)/2)

    // color blend
    float t = (waveTime + 1.0) * 0.5;
    float4 lerpColor = lerp(mainColor, secondColor, t * t);
    float4 colorCorrected = lerp(color, lerpColor, fadeMapColor.r);

    // top/bottom fade
    if (coords.y < 0.35)
    {
        float f = coords.y / 0.35;
        opacity *= f * f;
    }
    if (coords.y > 0.65)
    {
        float f = 1.0 - (coords.y - 0.65);
        opacity *= f * f;
    }

    return colorCorrected * opacity * 2.0 * realopacity;
}



technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}