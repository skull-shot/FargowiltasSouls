sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

float globalTime;
float3 mainColor;
bool fadeStart;

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
    output.Position = mul(input.Position, uWorldViewProjection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 baseColor = input.Color;
    float2 coords = input.TextureCoordinates.xy;
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float adjustedTime = globalTime * 1.2;

    float texX = coords.x * 4;
    float4 tex1 = tex2D(uImage1, float2(frac(texX - adjustedTime * 2.0), coords.y));
    float4 tex2 = tex2D(uImage1, float2(frac(texX - adjustedTime * 2.64), coords.y + sin(coords.x * 68 - adjustedTime * 6.283) * 0.08));
    float4 tex3 = tex2D(uImage1, float2(frac(texX - adjustedTime * 5.12), coords.y));

    float noise = pow((tex1.r * 0.5 + tex2.r * 0.75 + tex3.r * 0.5), 1.4);

    float4 hotColor = float4(mainColor, 1.0);
    float4 coreColor = float4(1.0, 1.0, 1.0, 1.0);

    float4 flameColor = lerp(baseColor, hotColor, noise);
    flameColor = lerp(flameColor, coreColor, pow(noise, 6));

    float opacity = noise;

    float yFade = 0.5 - abs(coords.y - 0.5);
    opacity *= smoothstep(0.0, 0.25, yFade);

    if (fadeStart)
    {
        float startFade = 0.2;
        if (coords.x < startFade)
            opacity *= pow(coords.x / startFade, 2);
    }

    float endFade = 0.15;
    float endFactor = saturate(pow(1 - (coords.x - endFade) / (1 - endFade), 2));
    opacity *= lerp(0.75, 1.0, endFactor);

    flameColor.rgb *= 1.5;

    return flameColor * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

