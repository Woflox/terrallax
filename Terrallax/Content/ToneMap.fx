uniform extern texture sceneTexture;

float offset;

sampler textureSampler = sampler_state
{
	Texture = <sceneTexture>;	
	minfilter = POINT;
	magfilter = POINT;
	mipfilter = LINEAR;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Position = float4(input.Position.x, input.Position.y, 1, 1);
	output.TexCoord = input.TexCoord;
    return output;
}

float4 ToneMap(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(textureSampler, input.TexCoord);

	color = color*(1+offset)-offset;

	color = (sin((color-0.5)*3.14159)+1)/2;

	color = pow(saturate(color), 1/2.2);
	
	
	return color;
}

technique Technique1
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 ToneMap();
    }
}
