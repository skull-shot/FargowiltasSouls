sampler noise : register(s1);
float radius;
float maxOpacity;
float time;
float2 scrollVector;

float2 screenPosition;
float2 screenSize;
float2 anchorPoint;
float2 playerPosition;

float InverseLerp(float a, float b, float t)
{
    return saturate((t - a) / (b - a));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 worldUV = screenPosition + screenSize * uv;
    float2 provUV = screenPosition / screenSize;
    float worldDistance = distance(worldUV, anchorPoint);
    
    float opacity = 1;
    
    // Pixelate
    float2 pixelatedUV = worldUV / screenSize;
    pixelatedUV.x -= worldUV.x % (0.5 / screenSize.x);
    pixelatedUV.y -= worldUV.y % (0.5 / (screenSize.y / 2) * 2);
    
    
    float2 noiseUV = pixelatedUV - (screenPosition / screenSize) * 0.96;
    float4 textureMesh = tex2D(noise, frac(noiseUV * 1.46 + scrollVector));
    textureMesh *= 0.3;
    
    bool border = worldDistance > radius && opacity > 0;
    float colorMult = 1;
    if (border)
    {
        colorMult = InverseLerp(radius * 1.4, radius, worldDistance);
    }
    else
    {
        colorMult = InverseLerp(radius * 1.4, radius, worldDistance);
    }
    
    float blackOpacity = 0;
    
    float4 black = float4(0, 0, 0, 1);
    
    return (black + (textureMesh)) * maxOpacity * colorMult;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
