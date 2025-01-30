sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);


float globalTime;
float3 mainColor;
matrix uWorldViewProjection;

// These 3 are required if using primitives. They are the same for any shader being applied to them
// so you can copy paste them to any other prim shaders and use the VertexShaderOutput input in the
// PixelShaderFunction.
// -->
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
// <--

// The X coordinate is the trail completion, the Y coordinate is the same as any other.
// This is simply how the primitive TextCoord is layed out in the C# code.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // This can also be copy pasted along with the above.
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;

    
    float y = pow(0.07, globalTime) * (coords.x * 0.4);
    //sin(12.5 * globalTime - 12.2 * coords.x) * 0.4;
    
	coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float widthScale = float((y + (1 - coords.x * 0.05)) / 2.8);
    
    if (coords.x < 0.09)
        widthScale /= pow(coords.x / 0.09, 0.7);
    
    if (coords.x < 0.02)
        widthScale /= pow(coords.x / 0.02, 10);
   

    
    coords.y = ((coords.y - clamp(0.48, 0.45, 0.49)) * clamp(widthScale, 0, 2)) + 0.5;
    
    
    // Get the pixel of the fade map. What coords.x is being multiplied by determines
    // how many times the uImage1 is copied to cover the entirety of the prim. 2, 2
	float4 fadeMapColor = tex2D(uImage1, float2(frac(coords.x * 0.8 - globalTime * 0.6), coords.y));
    
    // Use the red value for the opacity, as the provided image *should* be grayscale.
    float opacity = fadeMapColor.r;
    if (coords.x > 0.9)
        opacity *= pow(1 - (coords.x - 0.9) / 0.1, 5);
    // Lerp between the base color, and the provided color based on the opacity of the fademap.
    float4 changedColor = lerp(float4(mainColor, 1), color, 0.5f);
    float4 colorCorrected = lerp(color, changedColor, fadeMapColor.r);
    
    
    return colorCorrected * opacity * 2;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}