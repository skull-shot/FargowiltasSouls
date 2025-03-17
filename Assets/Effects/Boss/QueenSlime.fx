sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);
sampler2D uImage2 : register(s2);

float4 uSourceRect : register(c6);
float uTime : register(c5);
float2 uImageSize0 : register(c7);

float3 SinCosApprox(float x)
{
    float3 result;
    float x2 = x * x;
    result.x = x * (1.0 - x2 * (1.0 / 6.0)); // Approximate sine
    result.y = 1.0 - x2 * 0.5; // Approximate cosine
    return result;
}

float4 main(float4 v0 : COLOR0, float2 t0 : TEXCOORD0) : COLOR0
{
    float4 c8 = float4(1, -1, 0, 0);
    float4 c9 = float4(0.4, 0.3, 0.2, 1);
    float4 c10 = float4(-2, 3, 0.241915509, 0.5);
    float4 c11 = float4(6.28318548, -3.14159274, 0.377197206, 0.5);
    // D3DSINCOSCONST1 // float4 c12 = float4(-1.55009923e-006, -2.17013894e-005, 0.00260416674, 0.00026041668);
    // D3DSINCOSCONST2 // float4 c13 = float4(-0.020833334, -0.125, 1, 0.5);
    
    float2 r0 = uImageSize0;
    r0.y = saturate((t0.y * r0.y) - uSourceRect.y) * (1.0 / uSourceRect.w);
    
    float r0z = (r0.y * c10.x) + c10.y;
    float r0y2 = r0.y * r0.y;
    float r0w = (r0z * r0y2) * c11.z + c11.w;
    r0w = frac(r0w);
    r0w = (r0w * c11.x) + c11.y;
    
    // float r1y;
    // sincos(r1y, r0w, c12, c13);
    float r1y = SinCosApprox(r0w).y;
    // float r1y = cos(r0w);
    
    float2 r2 = c9.xy;
    r0w = (r1y * r2.x) + (1.0 - ((r0z * r0y2) + r0w));
    float r1y2 = (r0z * r0y2) + r0w;
    
    float r0x = saturate((t0.x * r0.x) - uSourceRect.x) * (1.0 / uSourceRect.z);
    float r0y3 = (r0x * c10.x) + c10.y;
    float r0x2 = r0x * r0x;
    float r0z2 = (r0y3 * r0x2) * c10.z + c10.w;
    r0z2 = frac(r0z2);
    r0z2 = (r0z2 * c11.x) + c11.y;
    
    // float r3x;
    // sincos(r3x, r0z2, c12, c13);
    float r3x = SinCosApprox(r0z2).x;
    // float r3x = sin(r0z2);
    
    float r1x = (r3x * r2.y) + (0.5 + (c9.z + r1y2));
    
    float2 texCoord = (float2(r1x, r1y2) + c9.w) * 0.5;
    float4 texColor = tex2D(uImage2, texCoord);
    
    float4 texColor0 = tex2D(uImage0, t0);
    
    float r0y4 = texColor0.x + uTime;
    float r0x3 = -texColor.x + r0y4 + c9.w;
    float r0y5 = (r0x3 > 0) ? c8.x : c8.y;
    r0x3 = frac(r0x3 * r0y5) * r0y5;
    
    float4 texColor1 = tex2D(uImage1, float2(r0x3, 0));
    
    float4 r2Final = float4(texColor0.x, texColor0.x, texColor0.x, 1); // grayscale
    float4 r3Final = texColor1 - r2Final; // diff
    float4 r0Final = (texColor0.y * r3Final) + r2Final; // blended
    
    r0Final.w = texColor0.w * r0Final.w;
    r0Final *= v0;
    
    return r0Final;
}

technique Technique1
{
    pass QueenSlime
    {
        PixelShader = compile ps_2_0 main();
    }
}