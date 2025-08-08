sampler diagonalNoise : register(s1);

float colorMult;
float time;
float maxOpacity;
float radius;

float2 screenPosition;
float2 screenSize;
float2 anchorPoint;

float2 velocity;
float velAngle;

float InverseLerp(float a, float b, float t)
{
    return saturate((t - a) / (b - a));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 worldUV = screenPosition + screenSize * uv;
    float2 provUV = anchorPoint / screenSize;
    float worldDistance = distance(worldUV, anchorPoint);
    float adjustedTime = time * 0.6;
    
    // Pixelate the uvs
    float2 pixelatedUV = worldUV / screenSize;

    pixelatedUV.x -= worldUV.x % (1 / screenSize.x);
    pixelatedUV.y -= worldUV.y % (1 / (screenSize.y / 2) * 2);
    
    float2 noiseUV = pixelatedUV - (anchorPoint / screenSize);
    
    noiseUV *= 2;
    
    // Textures
    float noiseMesh1 = tex2D(diagonalNoise, frac(noiseUV * 1.46 + velocity * adjustedTime)).g;
    float noiseMesh2 = tex2D(diagonalNoise, frac(noiseUV * 1.57 + velocity * adjustedTime)).g;
    float noiseMesh3 = tex2D(diagonalNoise, frac(noiseUV * 1.57 + velocity * adjustedTime)).g;
    float textureMesh = noiseMesh1 * 0.3 + noiseMesh2 * 0.3 + noiseMesh3 * 0.3;
    
    float opacity = 0.88;
    
    // Awesome Coordinate Transformation
    // normalize (center is (0, 0), top is (0, -1), left is (1, 0) etc
    float2 coords = (worldUV - anchorPoint) / radius;
    // rotate to align with velocity
    float angle = atan2(velocity.y, velocity.x);
    coords = float2(coords.x * cos(angle) + coords.y * sin(angle), coords.x * sin(angle) - coords.y * cos(angle));
    // shift y to be 1-0-1; 1 is on edges, 0 is at center
    coords.y = abs(coords.y);
    
    if (coords.x > 1)
        return sampleColor;
    
    // shift x to be 0-1, 0 is at the front, 1 is at the back
    coords.x = abs(1 - coords.x) / 2;
    
    
    float colorMult = 1;
    float width = 0.85 * saturate(pow(coords.x * 3, 0.4));
    if (coords.x > 0.6)
        width *= pow(InverseLerp(1.32, 0.6, coords.x), 0.25);
    if (coords.y > width)
        opacity *= pow(InverseLerp(width * 1.6, width, coords.y), 2);
    float length = 1.1;
    
    opacity *= pow(InverseLerp(length * 1.2, 0, coords.x), 0.1);
    
    colorMult = 1.3 * textureMesh.r; // InverseLerp(radius * 1.4, radius, worldDistance);
    if (coords.x < 1)
        colorMult += 1.3 * pow((1 - coords.x) - coords.y * 0.25, 2);
    
    opacity = clamp(opacity, 0, maxOpacity);
    
    
    if (colorMult == 1 && opacity == 0)
        return sampleColor;
    
    float4 darkColor = float4(0.96, 0.34, 0.04, 1); //red
    float4 midColor = float4(0.98, 0.95, 0.53, 1); //yellow
    float4 lightColor = float4(1, 1, 1, 1); //white
    
    float colorLerp = pow(colorMult, 3);
    //colorLerp = lerp(colorLerp, colorLerp * textureMesh + 0.3, 0.2);
    float4 color;
    float split = 0.9;
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
    color *= pow(abs(textureMesh), 0.25);
    color *= 1.4;

    return color * colorMult * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
