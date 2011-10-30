#include "inoise.fxh"

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDir;
float3 CameraPos;
float WaterLevel;

float4 SkyColorTop;
float4 SkyColorSunnySide;
float4 SkyColorFarSide;
float4 SunColor;

float4x4 MaterialColors;
float4x4 MaterialFunctions;
float4 Scaling;
float4 SpecularIntensity;
float4 SpecularPower;

float4 AmbientColor;
float4 DiffuseColor;
float4 SpecularColor;

float detail;

texture PerlinNoiseTexture;
texture UniformNoiseTexture;
texture MaterialMappingTexture;

//samplers
sampler perlinSampler = sampler_state 
{
    texture = <PerlinNoiseTexture>;
    AddressU  = Wrap;        
    AddressV  = Wrap;
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = NONE;   
};

sampler uniformSampler = sampler_state
{
	texture = <UniformNoiseTexture>;
	AddressU = Wrap;
	AddressV = Wrap;
	MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;   
};

sampler materialMappingSampler = sampler_state
{
	texture = <MaterialMappingTexture>;
	AddressU = Clamp;
	AddressV = Clamp;
	MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = NONE;   
};

struct VertexShaderInput
{
    float2 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 Info : TEXCOORD0; //stores info for texture generation
    float3 TanPos : TEXCOORD1;
    float3 Normal : TEXCOORD2;
    float3 TanLightDir : TEXCOORD3;
    float3 TanCamPos : TEXCOORD4;
    float2 TexCoord : TEXCOORD5;
    float2 MaxOffset : TEXCOORD6;
    float4 FogColor : TEXCOORD7;
};

struct PixelShaderOutputWithOffset
{
	float4 color0 : COLOR0;
	float4 color1 : COLOR1;
};

VertexShaderOutput TerrainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    // multiply by the WVP matrices
    float4 worldPosition = float4(input.Position.x, 0, input.Position.y, 1);
    worldPosition = mul(worldPosition, World);
    
    float4 testPos = worldPosition;
    testPos.y = 1600;
    float4 testCamPosA = mul(worldPosition, View);
    testCamPosA = mul(testCamPosA, Projection);
    testCamPosA.x /= testCamPosA.w;
    
    float4 testCamPosB = mul(testPos, View);
    testCamPosB = mul(testCamPosB, Projection);
    testCamPosB.x /= testCamPosB.w;
    
	float4 color = float4(0,0,0,0);
    if ((testCamPosA.x > -1.1 && testCamPosA.x < 1.1 && testCamPosA.z > 0) ||(testCamPosB.x > -1.1 && testCamPosB.x < 1.1 && testCamPosB.z > 0) )
    {
		// calc the height displacement using a multifractal
		float heightValue = mf(worldPosition.xz/1200, 8)*2;
	     
		//calculate the binormal and tangent by getting the height right next to the point for x and z
		float3 binormal = normalize(float3(0.00015, mf(worldPosition.xz/1200+float2(0.0001,0), 8) - heightValue/2, 0));
		float3 tangent = normalize(float3(0, mf(worldPosition.xz/1200+float2(0,0.0001), 8) - heightValue/2, 0.00015));
		float3 normal = normalize(cross(tangent, binormal));
		float3x3 tsm = {tangent, binormal, normal};

		// add the height displacement to the vertex position
		worldPosition.y = heightValue*400;
	    
		//worldPosition += float4(binormal,0) * test *10;
	   
	    
		output.Info = float4(normal.x, normal.z, worldPosition.y,0);

		output.TanPos = mul(tsm, worldPosition);
		output.Normal = normal;
		output.TanLightDir = mul(tsm, LightDir);
		output.TanCamPos = mul(tsm, CameraPos);
		output.TexCoord = worldPosition.xz;
	    
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);
		output.TexCoord = worldPosition.xz;
		
		float4 projectedNormal = mul(mul (worldPosition + float4(normal.x, normal.y, normal.z, 0), View), Projection);
		
		output.MaxOffset = (projectedNormal.xy/projectedNormal.w) - (output.Position.xy/output.Position.w);
		
		float distance = length(viewPosition.xyz);
		if(worldPosition.y <= WaterLevel || CameraPos.y < WaterLevel)
		{
			float waterDistance;
			if (CameraPos.y < WaterLevel)
			{
				waterDistance = distance;
			}
			else
			{
				waterDistance = length(float2((WaterLevel-worldPosition.y), length(worldPosition.xz-CameraPos.xz)*((WaterLevel-worldPosition.y) /(CameraPos.y-worldPosition.y)))); 
			}
			color.r = 1-pow(2.71828183, -(0.0075*waterDistance));
			distance -= waterDistance;
		}
		else
		{
			color.r = 0;
		}
		float fog = 1-pow(2.71828183, -pow(0.0003*distance,2));
		
		float3 cameraDir = normalize(worldPosition-CameraPos);
		
		float howSunny = saturate((dot(cameraDir, LightDir)+1)/2);
		float howHigh = saturate(dot(cameraDir, float3(0,1,0)));
		howHigh = 1-((1-howHigh)*(1-howHigh));
		float sunIntensity = 0.1/(pow(howSunny,0.325));

		
		output.FogColor = lerp(SkyColorSunnySide, SkyColorFarSide, howSunny);
		output.FogColor = lerp(output.FogColor, SkyColorTop, howHigh);
		output.FogColor += SunColor*sunIntensity;
		
		color.g = fog;
    }
    else
    {
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);
		output.Info = float4(0,0,0,0);
		output.TanPos = float4(0,0,0,0);
		output.Normal = float4(0,0,0,0);
		output.TanLightDir =float4(0,0,0,0);
		output.TanCamPos =float4(0,0,0,0);
		output.TexCoord = float4(0,0,0,0);
		output.FogColor = float4(0,0,0,0);
    }
	output.Color = color;
    return output;
}

float calculateHeightForMaterialLOD(float2 coordinates, float4 materialMapping, float material, float mipLevel)
{
	float4 bigValue = tex2Dlod(perlinSampler, float4(coordinates*0.004,0,0));
	bigValue.rgb -= 0.5;
	
	float4 values = float4(bigValue.a,
						   tex2Dlod(perlinSampler, float4(coordinates*0.016,0,0)).a,
						   tex2Dlod(uniformSampler, float4(coordinates*0.032,0,mipLevel)).a,
						   1);
	float rawHeight = mul( MaterialFunctions[material], values);
	
	rawHeight *= 1-(material==1)*abs(bigValue.r*0.75);
	rawHeight *= 1-(material==3)*abs(bigValue.a-0.5);
	

	return rawHeight+materialMapping[material]*Scaling[material];;
}

PixelShaderOutputWithOffset ExpTerrainPS(VertexShaderOutput input)
{
	float4 color;
	float4 offset = float4(0.5, 0.5, 0.5, 1);
	PixelShaderOutputWithOffset output;
	
    //these are the uv multipliers for the three texture samples
    const float bigMultiplier = 0.004;
    const float mediumMultiplier = 0.016;
    const float smallMultiplier = 0.032;
	
    //get the material mapping based on the information passed from the vertex shader. (Info.xy represents the x and z of the normal and Info.z is the terrain height)
    float4 materialMapping = tex2D(materialMappingSampler, (float2(length(input.Info.xy),1-(input.Info.z-WaterLevel)*0.0025)));
    materialMapping = float4(0, //dirt 
                             materialMapping.r,   //sand
                             materialMapping.g,   //grass
                             materialMapping.b);  //rock
	
    //make rocks smoother as they get rockier
    MaterialFunctions[3][1] *= 1 - max(0,materialMapping[3] * 2 - 1);
	
    //make dirt bumpier on inclines
    MaterialFunctions[0].xyz *= length(input.Info.xy)+0.5;

    float2 offsetCoord = input.TexCoord.xy*smallMultiplier;
    
    //get the mip-map level based only on the distance from the camera
    float mipLevel = log(length(input.TanCamPos - input.TanPos)*0.25*detail);
	
	//variables needed within the ray-marching loop
    float4 bigValue;
    float4 values;
    float4 rawHeights;
    float currentMaterial;
  	
        //sample the three textures
        bigValue = tex2Dlod(perlinSampler, float4(offsetCoord*bigMultiplier/smallMultiplier,0,0));
        bigValue.r -= 0.5;
        values = float4(bigValue.a, 
                               tex2Dlod(perlinSampler, float4(offsetCoord*mediumMultiplier/smallMultiplier,0,0)).a, 
                               tex2Dlod(uniformSampler, float4(offsetCoord,0,mipLevel)).a,
                               1);
		
        //get the heights for each material (without taking the material mapping into account)
        rawHeights = mul( MaterialFunctions, values);
		
        //adjust rock and sand to add ridges
        rawHeights[1]*= 1-abs(bigValue.r*0.75);
        rawHeights[3]*= 1-abs(bigValue.a-0.5);
		
        currentMaterial = 0;
        float currentHeight = -10;
		
        //loop through each material
        for(int i =0; i < 4; i++)
        {
            //add an offset to the material based on the material mapping
            float height = rawHeights[i]+materialMapping[i]*Scaling[i];
            if (height > currentHeight)
            {
                //keep track of the highest material
                currentMaterial = i;
                currentHeight = height;
            }
        }
		
    offset.rg = (input.MaxOffset * (currentHeight - 1.25)*10)+0.5;
    offset.b = 0;
    offset.a = 1;
    
    //get neighbouring heights to calculate the normal
    float height2 = calculateHeightForMaterialLOD(offsetCoord/smallMultiplier+float2(0.05, 0), materialMapping, currentMaterial, mipLevel);
    float height3 = calculateHeightForMaterialLOD(offsetCoord/smallMultiplier+float2(0, 0.05), materialMapping, currentMaterial, mipLevel);
    float3 normal = normalize(-cross(float3(0, 0.01, height2-currentHeight), float3(0.01, 0, height3-currentHeight)));
									 
    if(currentMaterial >1)
    {
        //use the material color
        color = MaterialColors[currentMaterial];
        if (currentMaterial == 2)
        {
            //for grass, add some variance to the color based on the perlin noise textures
            color *= float4(-bigValue.a+2, bigValue.a*0.25+0.75, 1+values[1]*3,0);
        }
    }
    else
    {
        //if we have dirt or sand, blend the colours between them based on the sand's material mapping
        color = lerp(MaterialColors[0], MaterialColors[1], materialMapping[1]);
	    
        //make flat parts grass-coloured when in a grassy area
        color = lerp(color,MaterialColors[2],materialMapping[2]*(1-min(1.5,length(normal.xy)*4))*0.5);
        
         //darken the ground near rocks
        //color *= 0.75+ 2 * min(0.125,currentHeight-(rawHeights[3]+materialMapping[3]*Scaling[3]));

    }
    if (currentMaterial == 3)
    {
        //make the edges of rocks dark
        color *= clamp((currentHeight+0.95-rawHeights[0]),0.95,1);
        color *= values[2]*0.25 + 0.75;
        //make the specular intensity vary for rocks (based on the fine noise)
        SpecularPower[3] *= values[2];
        SpecularIntensity[3] *= values[2];
    }
	
    //add noise to the colour and make higher parts lighter
    color *= 0.5 + currentHeight*0.5*(values[2]*0.2+0.9);
	
    if(input.Info.z <= WaterLevel+1.2-currentHeight)
    {
        //make underwater terrain tinted blue
        //color = float4(0.8,0.8,0.4,0);
    }
	
    //do lighting calculations
    float3 directionToCamera = normalize(input.TanCamPos - input.TanPos);
    float3 reflectionVector =  normalize(reflect(-input.TanLightDir, normal));
    float specular = pow(saturate(dot(-reflectionVector, directionToCamera)), SpecularPower[currentMaterial]);
    float diffuse = saturate(dot(-input.TanLightDir, normal));
    float fakeShadow = saturate(input.TanLightDir.z);
    diffuse = saturate(diffuse - fakeShadow);
    
    if(currentMaterial == 2)
    {
        //for grass, make high parts unaffected by diffuse lighting
        diffuse = lerp( diffuse, 1, saturate(currentHeight-rawHeights[0]+materialMapping[0]*Scaling[0]));
    }
	
    //apply ambient, diffuse, and specular lighting
    color *= float4(0.12,0.12,0.15,1) + diffuse*float4(1.5,1,0.7,0) + specular*SpecularIntensity[currentMaterial];
	
    if(input.Info.z <= WaterLevel)
    {
        //apply water fog
        color = lerp(color, float4(0.1,0.2,0.04,1), input.Color.r);
    }
	
    //apply distance fog
    color = lerp(color, input.FogColor, input.Color.g);
    color.rgb = sqrt(color.rgb);
    color.a = 1;
    output.color0 = color;
    output.color1 = offset;
    return output;
}

technique ExpTerrain
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 TerrainVS();
		PixelShader = compile ps_3_0 ExpTerrainPS();
	}
}