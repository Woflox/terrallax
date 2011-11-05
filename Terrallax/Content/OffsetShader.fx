
texture sceneTexture;
texture offsetTexture;

//samplers
sampler sceneSampler = sampler_state 
{
    texture = <sceneTexture>;
    AddressU  = CLAMP;   
    AddressV  = CLAMP;
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = NONE;   
};

sampler offsetSampler = sampler_state 
{
    texture = <offsetTexture>;
    AddressU  = CLAMP;   
    AddressV  = CLAMP;
    MAGFILTER = POINT;
    MINFILTER = POINT;
    MIPFILTER = NONE;   
};



struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = input.Position;
    float2 texCoord = (input.Position.xy+1)/2;
    texCoord.y = 1-texCoord.y;
    float4 offset = tex2Dlod(offsetSampler, float4(texCoord.x, texCoord.y,0,0));
    output.Position.z = offset.b+offset.a/255;
	output.Position.xy += offset.rg-0.5;
    output.Position.w = 1;
    output.TexCoord = texCoord;
    
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(sceneSampler, input.TexCoord);
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
