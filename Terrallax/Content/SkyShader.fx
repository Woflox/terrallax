float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDir;
float4 SkyColorTop;
float4 SkyColorSunnySide;
float4 SkyColorFarSide;
float4 SunColor;

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 position : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.position = worldPosition;

    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color;
	
    float3 cameraDir = normalize(input.position.xyz);
	float howSunny = saturate((dot(cameraDir, LightDir)+1)/2);
	float howHigh = saturate(dot(cameraDir, float3(0,1,0)));
	howHigh = 1-((1-howHigh)*(1-howHigh));
	float sunIntensity = 0.1/(pow(howSunny,0.325));
	
	color = lerp(SkyColorSunnySide, SkyColorFarSide, howSunny);
	color = lerp(color, SkyColorTop, howHigh);
	color += SunColor*sunIntensity;
	color.rgb = sqrt(color.rgb);
	
    return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
