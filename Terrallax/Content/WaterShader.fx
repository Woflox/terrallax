float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ViewAngleProjection;

float3 LightDir;
float3 CameraPos;

float4 SkyColorTop;
float4 SkyColorSunnySide;
float4 SkyColorFarSide;
float4 SunColor;

float t;

texture PerlinNoiseTexture;
texture ReflectTexture;

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

sampler reflectSampler = sampler_state 
{
    texture = <ReflectTexture>;
    AddressU  = Clamp;        
    AddressV  = Clamp;
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;   
};

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

struct DepthShaderOutput
{
	float4 Position : POSITION0;
	float4 pos : TEXCOORD0;
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

DepthShaderOutput DepthVertexShaderFunction(VertexShaderOutput input)
{
	DepthShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.pos = output.Position;
    
    return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput Output;
	float4 c1 = tex2D(perlinSampler, (input.TexCoord.xz+t*6)*0.006);
	float4 c2 = tex2D(perlinSampler, (input.TexCoord.xz-t*7.5)*0.012);
	float4 c3 = tex2D(perlinSampler, (input.TexCoord.xz+t*20)*0.001);
	float4 c4 = tex2D(perlinSampler, (input.TexCoord.xz-t*1)*0.009);
	
	float4 waterColor = float4(0.0,0.1,0.0,1);
	
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
	
	float fresnel = pow(saturate(dot(normal, directionToCamera)),0.5);
	fresnel = 1-((1-fresnel)*1.2);
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
	//color.r *= 0.7;
	
	//color = lerp(float4(0.7,0.9,1,1), float4(0.3,0.7,1,1), specular2);
	color.a = lerp(1, 0.01, fresnel);
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
		color.rgba = lerp ( color, waterColor, 1-pow(2.71828183, -(0.0175*waterDistance)));
	}
	color.a = 1 - color.a;
	Output.Color0 = color;
	Output.Color1.rg = float2(0.5, 0.5 + height*4/length(CameraPos - input.TexCoord.xyz));
	Output.Color1.b = 0;//floor(input.TexCoord.w*255)/255;
	Output.Color1.a = 0;//floor((input.TexCoord.w*255-Output.Color1.b)*255)/255;
	
    return Output;
}

PixelShaderOutput ReflectWorldPS(VertexShaderOutput input)
{
	PixelShaderOutput Output;
	float4 c1 = tex2D(perlinSampler, (input.TexCoord.xz+t*6)*0.006);
	float4 c2 = tex2D(perlinSampler, (input.TexCoord.xz-t*7.5)*0.012);
	float4 c3 = tex2D(perlinSampler, (input.TexCoord.xz+t*20)*0.001);
	float4 c4 = tex2D(perlinSampler, (input.TexCoord.xz-t*1)*0.009);
	
	float4 waterColor = float4(0.0,0.1,0.0,1);
	
	float4 color;
	
	float3 normal = (c1.rgb*0.5 + c2.rgb*0.2 + c4.rgb*0.2 + c3.rgb*0.1).rbg*2-1;
	float height = c1.a*0.5 + c2.a*(0.2*0.5) + c4.a*(0.2*0.666) + c3.a*(0.1*6);
	float3 directionToCamera = normalize(CameraPos - input.TexCoord);
	float underwater = directionToCamera.y;
	if (underwater < 0)
	{
		directionToCamera.y *= -1;
		normal.rb *= -1;
	}
    float3 reflectionVector =  normalize(reflect(-LightDir, normal));
    float3 reflectionVectorAmbient =  normalize(reflect(float3(0,1,0), normal));
    float specular = pow(saturate(dot(-reflectionVector, directionToCamera)),600);
    float specular2 = pow(saturate((dot(-reflectionVectorAmbient, directionToCamera))), 1);
	
	float fresnel = pow(saturate(dot(normal, directionToCamera)),0.5);
	fresnel = 1-((1-fresnel)*1.2);
	if (underwater < 0)
	{
		 fresnel = saturate(dot(normal, directionToCamera));
	}
	
	normal.rb *= 0.25;
	normal = normalize(normal);
	
	float3 cameraDir = reflect(-directionToCamera,normal);
	
	float4 reflectCoord = float4(cameraDir.x, -cameraDir.y, cameraDir.z, 1);
	
	if (underwater < 0)
	{
		reflectCoord.y *= -1;
	}
	reflectCoord.xyz *= 10000;
	
	reflectCoord = mul(reflectCoord, ViewAngleProjection);
	
	reflectCoord /= reflectCoord.w;
	
	float waterDistance = length(CameraPos - input.TexCoord);
		
	reflectCoord.y -= min(0.05, 0.05/(waterDistance*0.0025));
	
	color = tex2D(reflectSampler, (reflectCoord.xy+1)/2);
	
	color.a = lerp(1, 0.01, fresnel);
	color +=  float4(1.75,1.5,1.25,1.5) * specular;
	
	if (underwater < 0)
	{
		//waterfog
		color.rgba = lerp ( color, waterColor, 1-pow(2.71828183, -(0.0175*waterDistance)));
	}
	color.a = 1 - color.a;
	Output.Color0 = color;
	Output.Color1.rg = float2(0.5, 0.5 + height*4/length(CameraPos - input.TexCoord.xyz));
	Output.Color1.b = 0;//floor(input.TexCoord.w*255)/255;
	Output.Color1.a = 0;//floor((input.TexCoord.w*255-Output.Color1.b)*255)/255;
	
    return Output;
}

PixelShaderOutput DepthShaderFunction(DepthShaderOutput input)
{
	PixelShaderOutput Output;
	
	Output.Color0 = float4(0,0,0,1);
	
	float depth = input.pos.z / input.pos.w;
	Output.Color1.rg = float2(0,0);
	Output.Color1.b = floor(depth*255)/255;
	Output.Color1.a = (depth-Output.Color1.b)*255;
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
    
    pass Pass2
    {
		VertexShader = compile vs_3_0 DepthVertexShaderFunction();
		PixelShader = compile ps_3_0 DepthShaderFunction();
    }
}

technique reflectWorld
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 ReflectWorldPS();
    }
    
    pass Pass2
    {
		VertexShader = compile vs_3_0 DepthVertexShaderFunction();
		PixelShader = compile ps_3_0 DepthShaderFunction();
    }
}
