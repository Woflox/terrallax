uniform extern texture sceneTexture;

sampler textureSampler = sampler_state
{
	Texture = <sceneTexture>;	
	minfilter = POINT;
	magfilter = POINT;
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

	color = color*1.1-0.1;
	color = pow(saturate(color), 1/2.2);
	
	return color;
	
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
