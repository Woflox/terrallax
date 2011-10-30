float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDir;
float3 CameraPos;

float4 SkyColorTop;
float4 SkyColorSunnySide;
float4 SkyColorFarSide;
float4 SunColor;

float t;

texture PerlinNoiseTexture;

//samplers
sampler perlinSampler = sampler_state 
{
    texture = <PerlinNoiseTexture>;
    AddressU  = Wrap;        
    AddressV  = Wrap;
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;   
};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
};

struct PixelShaderOutput
{
	float4 Color0 : COLOR0;
	float4 Color1 : COLOR1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = worldPosition;

    return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput Output;
	float4 c1 = tex2D(perlinSampler, (input.TexCoord.xz+t*6)*0.006);
	float4 c2 = tex2D(perlinSampler, (input.TexCoord.xz-t*7.5)*0.012);
	float4 c3 = tex2D(perlinSampler, (input.TexCoord.xz+t*20)*0.001);
	float4 c4 = tex2D(perlinSampler, (input.TexCoord.xz-t*1)*0.009);
	
	float4 waterColor = float4(0.1,0.2,0.04,1);
	
	float4 color;
	
	float3 normal = (c1.rgb*0.5 + c2.rgb*0.2 + c4.rgb*0.2 + c3.rgb*0.1).rbg*2-1;
	float height = c1.a*0.5 + c2.a*(0.2*0.5) + c4.a*(0.2*0.666) + c3.a*(0.1*6);
	float3 directionToCamera = normalize(CameraPos - input.TexCoord);
	float underwater = directionToCamera.y;
	if (underwater < 0)
	{
		directionToCamera.y *= -1;
		//SkyColorTop = waterColor;
		SkyColorSunnySide = waterColor;
		SkyColorFarSide = waterColor;
		normal.rb *= -1;
	}
    float3 reflectionVector =  normalize(reflect(-LightDir, normal));
    float3 reflectionVectorAmbient =  normalize(reflect(float3(0,1,0), normal));
    float specular = pow(saturate(dot(-reflectionVector, directionToCamera)),600);
    float specular2 = pow(saturate((dot(-reflectionVectorAmbient, directionToCamera))), 1);
	
	float fresnel = pow(saturate(dot(normal, directionToCamera)),0.35);
	if (underwater < 0)
	{
		 fresnel = saturate(dot(normal, directionToCamera));
	}
	float3 cameraDir = reflect(-directionToCamera,normal);
	
	float howSunny = saturate((dot(cameraDir, LightDir)+1)/2);
	float howHigh = saturate(dot(cameraDir, float3(0,1,0)));
	howHigh = 1-((1-howHigh)*(1-howHigh));
	float sunIntensity = 0.1/(pow(howSunny,0.325));
	
	color = lerp(SkyColorSunnySide, SkyColorFarSide, howSunny);
	color = saturate(lerp(color, SkyColorTop, howHigh));
	color += SunColor*sunIntensity;
	color.r *= 0.6;
	
	//color = lerp(float4(0.7,0.9,1,1), float4(0.3,0.7,1,1), specular2);
	color.a = lerp(1, 0.05, fresnel);
	color +=  float4(1.75,1.5,1.25,1.5) * specular;
	
	//fog
	cameraDir = -directionToCamera;
	howSunny = saturate((dot(cameraDir, LightDir)+1)/2);
		
	float4 fogColor = lerp(SkyColorSunnySide, SkyColorFarSide, howSunny);
	
	float distance = length(input.TexCoord.xyz-CameraPos);
	float fog = 1-pow(2.71828183, -pow(0.0006*distance,2));
	
	color.rgb = lerp(color.rgb, fogColor.rgb, fog);
	
	if (underwater < 0)
	{
		//waterfog
		float waterDistance = length(CameraPos - input.TexCoord);
		color.rgba = lerp ( color, waterColor, 1-pow(2.71828183, -(0.0075*waterDistance)));
	}
	
	color.rgb = sqrt(color.rgb);
	Output.Color0 = color;
	Output.Color1 = float4(0.5, 0.5 + height*4/length(CameraPos - input.TexCoord), 0.5, 1);
    return Output;
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