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
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
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
	//color = saturate(pow(color, 1/2.4)*1.4-0.4);
	color = pow(saturate(color*1.125-0.125), 1/2.2);
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
