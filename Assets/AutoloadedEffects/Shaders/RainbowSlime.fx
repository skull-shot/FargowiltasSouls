sampler uImage0 : register(s0);

float globalTime;

// HSV to RGB function with smooth transitions
float3 hsv2rgb(float hue, float saturation, float value)
{
    float3 p = abs(frac(hue + float3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
    return value * lerp(float3(1.0, 1.0, 1.0), saturate(p), saturation);
}

// Smooth blending function
float smoothBlend(float x)
{
    return 0.5 + 0.5 * cos(3.14159 * x);
}

float4 RainbowShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Create diagonal bands without requiring offset/scale
    float hue = fmod(globalTime * 0.2 + dot(coords, float2(1.0, 1.0)) * 2.0, 1.0);

    // Smooth the hue transition
    float smoothedHue = smoothBlend(hue);

    // Convert to RGB with a nice balance of saturation and brightness
    float3 rgb = hsv2rgb(smoothedHue, 0.8, 1.0);

    // Sample the texture
    float4 texColor = tex2D(uImage0, coords);

    // Blend the colors smoothly
    texColor.rgb = lerp(texColor.rgb, texColor.rgb * rgb, 0.7);

    return texColor * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 RainbowShader();
    }
}