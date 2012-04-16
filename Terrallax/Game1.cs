using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics.PackedVector; //for NormalizedByte4

namespace Terrallax
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
        public static Game1 instance;

		const bool FULL_SCREEN = true;
        const bool V_SYNC = false;
        const bool FIXED_TIME_STEP = false;
        const bool REFLECT_WORLD = true;

        public int SCREEN_WIDTH = 1280;
        public int SCREEN_HEIGHT = 800;

        const int REFLECT_WIDTH = 400;
        const int REFLECT_HEIGHT = 240;

        Vector4 SKY_COLOR_TOP = new Vector4(0.3f,0.6f,0.85f,1);
        Vector4 SKY_COLOR_SUNNY_SIDE = new Vector4(1.3f, 1.0f, 0.75f, 1);
        Vector4 SKY_COLOR_FAR_SIDE = new Vector4(0.85f, 0.8f, 0.75f, 1);
        Vector4 SUN_COLOR = new Vector4(0.5f, 0.25f, 0, 1);

        string CURRENT_TECHNIQUE = "ExpTerrain";
        
        const float VIEWING_ANGLE = 80;
        const float CAMERA_DISTANCE = 7.5f;
        const float MOUSE_SENSITIVITY = 1f;
        
        const float NEAR = 0.5f;
        const float FAR = 6400f;

        DepthStencilBuffer depthBuffer;
        DepthStencilBuffer depthBufferScene;


        //const int OFFSET_VERT_WIDTH = 640;
        //const int OFFSET_VERT_HEIGHT = 360;

		const float WATER_LEVEL = 440;

		Vector3 LIGHT_DIRECTION = Vector3.Normalize(new Vector3(1,-0.375f, 0));

		Matrix MATERIAL_COLORS = new Matrix(0.13f, 0.0925f , 0.05f, 0, //dirt
											1.0f, 0.65f , 0.2f, 0, //sand
											0.2f, 0.4f , 0.0525f, 0, //grass
											0.15f, 0.15f, 0.085f, 0);//rock

		Matrix MATERIAL_FUNCTIONS = new Matrix(0.4f , 0.2f,  0.06f, 1, //dirt
											   0.5f , 0.1f, 0.025f, 0.5f, //sand
											   0    , 0   , 1.75f   , -0.6f, //grass
											   0.6f , 1f   , 0.075f , -3f);//rock
        
		Vector4 SCALING = new Vector4(1,    //dirt 
                                      1,    //sand
                                      0.9f, //grass
                                      4.5f);   //rock

		Vector4 SPECULAR_INTENSITY = new Vector4(0.15f, 0.2f, 0.2f, 1f);

		Vector4 SPECULAR_POWER = new Vector4(20, 40, 10, 60);

		int fpsIndex = 0;
		float[] fps = new float[16];

        Terrain terrain;

		//camera properties
        public Vector3 camerapos;
        public Vector3 reflectedCameraPos;
        Vector3 up;
        float cameraPitch;
        float cameraYaw;
        float deltaT;

        //performance stats
        float totalT = 0;
        float totalNumFrames = 0;
        float avgFps = 0;
        float minFps = 1000;
        float maxFps = 0;

		Random r;

		Matrix world;
		Matrix view;
        Matrix reflectedView;
		Matrix projection;

        Matrix skyView;
        Matrix reflectedSkyView;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		VertexDeclaration terrainVertexDeclaration;
        VertexDeclaration flatVertexDeclaration;
		VertexDeclaration skyVertexDeclaration;
        VertexDeclaration sunVertexDeclaration;

        VertexBuffer vBufferOffset;
        IndexBuffer iBufferOffset;
        List<VertexPosition2D> offsetVertices;
        VertexPositionTexture[] FullScreenQuad;
        VertexPositionColor[] WaterVertices;
        List<int> offsetIndices;

		string displayText;
		SpriteFont font;

		Effect terrainShader;
        Effect offsetShader;
        Effect clearShader;
        Effect waterShader;
        Effect skyShader;
        Effect toneMapShader;
        Effect fullScreenTextureShader;
		Texture2D permTexture;
		Texture2D permTexture2d;
		Texture2D permGradTexture;
		Texture2D gradTexture4d;

		Texture2D perlinNoiseTexture;
		Texture2D uniformNoiseTexture;
		Texture2D materialMappingTexture;

        RenderTarget2D terrainTarget;
        RenderTarget2D offsetTarget;
        RenderTarget2D sceneTarget;
        RenderTarget2D reflectTarget;

        VertexPositionTexture[] sunVertices;
        VertexPositionTexture[] skyVertices;

        int deltaTIndex = 0;
        float[] deltaTArray = new float[4];

		#region Arrays used for texture generation
		// gradients for 3d noise
		static float[,] g3 =  
        {
            {1,1,0},
            {-1,1,0},
            {1,-1,0},
            {-1,-1,0},
            {1,0,1},
            {-1,0,1},
            {1,0,-1},
            {-1,0,-1}, 
            {0,1,1},
            {0,-1,1},
            {0,1,-1},
            {0,-1,-1},
            {1,1,0},
            {0,-1,1},
            {-1,1,0},
            {0,-1,-1}
        };

		// gradients for 4D noise
		static float[,] g4 = 
        {
	        {0, -1, -1, -1},
	        {0, -1, -1, 1},
	        {0, -1, 1, -1},
	        {0, -1, 1, 1},
	        {0, 1, -1, -1},
	        {0, 1, -1, 1},
	        {0, 1, 1, -1},
	        {0, 1, 1, 1},
	        {-1, -1, 0, -1},
	        {-1, 1, 0, -1},
	        {1, -1, 0, -1},
	        {1, 1, 0, -1},
	        {-1, -1, 0, 1},
	        {-1, 1, 0, 1},
	        {1, -1, 0, 1},
	        {1, 1, 0, 1},
        	
	        {-1, 0, -1, -1},
	        {1, 0, -1, -1},
	        {-1, 0, -1, 1},
	        {1, 0, -1, 1},
	        {-1, 0, 1, -1},
	        {1, 0, 1, -1},
	        {-1, 0, 1, 1},
	        {1, 0, 1, 1},
	        {0, -1, -1, 0},
	        {0, -1, -1, 0},
	        {0, -1, 1, 0},
	        {0, -1, 1, 0},
	        {0, 1, -1, 0},
	        {0, 1, -1, 0},
	        {0, 1, 1, 0},
	        {0, 1, 1, 0}
        };

		static int[] perm = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
			    96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37,
			    240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
			    94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56,
			    87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139,
			    48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133,
			    230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25,
			    63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
			    196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
			    64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255,
			    82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
			    223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
			    101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79,
			    113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242,
			    193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249,
			    14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204,
			    176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222,
			    114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
		#endregion

		#region Texture generation methods
		private void GeneratePermTexture()
		{
			permTexture = new Texture2D(GraphicsDevice, 256, 1, 1, TextureUsage.None, SurfaceFormat.Luminance8);
			byte[] data = new byte[256 * 1];
			for (int x = 0; x < 256; x++)
			{
				for (int y = 0; y < 1; y++)
				{
					data[x + (y * 256)] = (byte)(perm[x]);
				}
			}
			permTexture.SetData<byte>(data);
		}

		int perm2d(int i)
		{
			return perm[i % 256];
		}

		private void GeneratePermTexture2d()
		{
			permTexture2d = new Texture2D(GraphicsDevice, 256, 256, 1, TextureUsage.None, SurfaceFormat.Color);
			Color[] data = new Color[256 * 256];
			for (int x = 0; x < 256; x++)
			{
				for (int y = 0; y < 256; y++)
				{
					int A = perm2d(x) + y;
					int AA = perm2d(A);
					int AB = perm2d(A + 1);
					int B = perm2d(x + 1) + y;
					int BA = perm2d(B);
					int BB = perm2d(B + 1);
					data[x + (y * 256)] = new Color((byte)(AA), (byte)(AB),
													(byte)(BA), (byte)(BB));
				}
			}
			permTexture2d.SetData<Color>(data);
		}

		private void GeneratePermGradTexture()
		{
			permGradTexture = new Texture2D(GraphicsDevice, 256, 1, 1, TextureUsage.None, SurfaceFormat.NormalizedByte4);
			NormalizedByte4[] data = new NormalizedByte4[256 * 1];
			for (int x = 0; x < 256; x++)
			{
				for (int y = 0; y < 1; y++)
				{
					data[x + (y * 256)] = new NormalizedByte4(g3[perm[x] % 16, 0], g3[perm[x] % 16, 1], g3[perm[x] % 16, 2], 1);
				}
			}
			permGradTexture.SetData<NormalizedByte4>(data);
		}

		private void GenerateGradTexture4d()
		{
			gradTexture4d = new Texture2D(GraphicsDevice, 32, 1, 1, TextureUsage.None, SurfaceFormat.NormalizedByte4);
			NormalizedByte4[] data = new NormalizedByte4[32 * 1];
			for (int x = 0; x < 32; x++)
			{
				for (int y = 0; y < 1; y++)
				{
					data[x + (y * 32)] = new NormalizedByte4(g4[x, 0], g4[x, 1], g4[x, 2], g4[x, 3]);
				}
			}
			gradTexture4d.SetData<NormalizedByte4>(data);
		}

		/// <summary>
		/// Generates all of the needed textures on the CPU
		/// </summary>
		private void GenerateTextures()
		{
			GeneratePermTexture();
			GeneratePermTexture2d();
			GeneratePermGradTexture();
			GenerateGradTexture4d();
		}
		#endregion

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			this.IsFixedTimeStep = FIXED_TIME_STEP;
            graphics.SynchronizeWithVerticalRetrace = V_SYNC;
            instance = this;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            SCREEN_WIDTH = displayMode.Width;
            SCREEN_HEIGHT = displayMode.Height;
			this.graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			this.graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
			this.graphics.IsFullScreen = FULL_SCREEN;
			this.graphics.ApplyChanges();
			this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.graphics.PreferMultiSampling = true;
			initializeCamera();
			
			Mouse.SetPosition(GraphicsDevice.PresentationParameters.BackBufferWidth / 2,
							  GraphicsDevice.PresentationParameters.BackBufferHeight / 2);

			r = new Random();

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont>(@"ScreenFont");
			terrainShader = Content.Load<Effect>("TerrainShader");
            offsetShader = Content.Load<Effect>("OffsetShader");
            clearShader = Content.Load<Effect>("FullScreenClear");
            waterShader = Content.Load<Effect>("WaterShader");
            skyShader = Content.Load<Effect>("SkyShader");
            toneMapShader = Content.Load<Effect>("ToneMap");
            fullScreenTextureShader = Content.Load<Effect>("FullScreenTexture");
			perlinNoiseTexture = Content.Load<Texture2D>("perlinnoise");
			uniformNoiseTexture = Content.Load<Texture2D>("uniformnoise");
			materialMappingTexture = Content.Load<Texture2D>("materialmapping");

            generateOffsetVertices();
            generateFullScreenQuad();
            GenerateTextures();
            generateWater();
            sunVertices = Skybox.createSun(0.7f, LIGHT_DIRECTION);
            skyVertices = Skybox.createSkybox();

            vBufferOffset = new VertexBuffer(GraphicsDevice, offsetVertices.Count * VertexPosition2D.SizeInBytes, BufferUsage.WriteOnly);
            vBufferOffset.SetData(offsetVertices.ToArray());
            iBufferOffset = new IndexBuffer(GraphicsDevice, sizeof(int) * offsetIndices.Count, BufferUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
            iBufferOffset.SetData(offsetIndices.ToArray());

			skyVertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
			terrainVertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPosNormalTanBinormal.VertexElements);
            flatVertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPosition2D.VertexElements);
            sunVertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);

			terrainShader.Parameters["LightDir"].SetValue(LIGHT_DIRECTION);
			terrainShader.Parameters["WaterLevel"].SetValue(WATER_LEVEL);
			terrainShader.Parameters["PerlinNoiseTexture"].SetValue(perlinNoiseTexture);
			terrainShader.Parameters["UniformNoiseTexture"].SetValue(uniformNoiseTexture);

			terrainShader.Parameters["MaterialMappingTexture"].SetValue(materialMappingTexture);
			terrainShader.Parameters["MaterialColors"].SetValue(MATERIAL_COLORS);
			terrainShader.Parameters["MaterialFunctions"].SetValue(MATERIAL_FUNCTIONS);
			terrainShader.Parameters["Scaling"].SetValue(SCALING);
			terrainShader.Parameters["SpecularIntensity"].SetValue(SPECULAR_INTENSITY);
			terrainShader.Parameters["SpecularPower"].SetValue(SPECULAR_POWER);

			// Set the perlin noise permutation and gradient textures
			terrainShader.Parameters["permTexture"].SetValue(permTexture);
			terrainShader.Parameters["permTexture2d"].SetValue(permTexture2d);
			terrainShader.Parameters["permGradTexture"].SetValue(permGradTexture);
			terrainShader.Parameters["gradTexture4d"].SetValue(gradTexture4d);
            terrainShader.Parameters["detail"].SetValue(1 / (SCREEN_HEIGHT / 800f));

			terrainShader.CurrentTechnique = terrainShader.Techniques[CURRENT_TECHNIQUE];
			terrainShader.CommitChanges();

            waterShader.Parameters["PerlinNoiseTexture"].SetValue(perlinNoiseTexture);
            waterShader.Parameters["LightDir"].SetValue(LIGHT_DIRECTION);

            skyShader.Parameters["LightDir"].SetValue(LIGHT_DIRECTION);
            skyShader.Parameters["SkyColorTop"].SetValue(SKY_COLOR_TOP);
            skyShader.Parameters["SkyColorSunnySide"].SetValue(SKY_COLOR_SUNNY_SIDE);
            skyShader.Parameters["SkyColorFarSide"].SetValue(SKY_COLOR_FAR_SIDE);
            skyShader.Parameters["SunColor"].SetValue(SUN_COLOR);

            terrainShader.Parameters["SkyColorTop"].SetValue(SKY_COLOR_TOP);
            terrainShader.Parameters["SkyColorSunnySide"].SetValue(SKY_COLOR_SUNNY_SIDE);
            terrainShader.Parameters["SkyColorFarSide"].SetValue(SKY_COLOR_FAR_SIDE);
            terrainShader.Parameters["SunColor"].SetValue(SUN_COLOR);

            waterShader.Parameters["SkyColorTop"].SetValue(SKY_COLOR_TOP);
            waterShader.Parameters["SkyColorSunnySide"].SetValue(SKY_COLOR_SUNNY_SIDE);
            waterShader.Parameters["SkyColorFarSide"].SetValue(SKY_COLOR_FAR_SIDE);
            waterShader.Parameters["SunColor"].SetValue(SUN_COLOR);

            if (REFLECT_WORLD)
            {
                waterShader.CurrentTechnique = waterShader.Techniques[1];
            }

            terrainTarget = new RenderTarget2D(GraphicsDevice,
                                         GraphicsDevice.PresentationParameters.BackBufferWidth,
                                         GraphicsDevice.PresentationParameters.BackBufferHeight,
                                         1, SurfaceFormat.Color);
            offsetTarget = new RenderTarget2D(GraphicsDevice,
                                         GraphicsDevice.PresentationParameters.BackBufferWidth,
                                         GraphicsDevice.PresentationParameters.BackBufferHeight,
                                         1, SurfaceFormat.Color);
            sceneTarget = new RenderTarget2D(GraphicsDevice,
                                         GraphicsDevice.PresentationParameters.BackBufferWidth,
                                         GraphicsDevice.PresentationParameters.BackBufferHeight,
                                         1, SurfaceFormat.Color, MultiSampleType.FourSamples, GraphicsDevice.PresentationParameters.MultiSampleQuality );
            depthBuffer = new DepthStencilBuffer(GraphicsDevice,
                                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                                         GraphicsDevice.PresentationParameters.BackBufferHeight,
                                         DepthFormat.Depth24Stencil8);
            depthBufferScene = new DepthStencilBuffer(GraphicsDevice,
                                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                                         GraphicsDevice.PresentationParameters.BackBufferHeight,
                                         DepthFormat.Depth24Stencil8, MultiSampleType.FourSamples, GraphicsDevice.PresentationParameters.MultiSampleQuality);
            reflectTarget = new RenderTarget2D(GraphicsDevice,
                                        REFLECT_WIDTH,
                                        REFLECT_HEIGHT,
                                        0,
                                        SurfaceFormat.Color);

            terrain = new Terrain();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

            deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;


            displayText = "";

            if (deltaT != 0)
            {
                int displayFps = 0;
                fps[fpsIndex] = deltaT;
                float currentFps = 1 / (fps.Sum() / fps.Count());

                    minFps = (float)Math.Min(minFps, currentFps);
                    if (totalT > 1)
                    {
                        maxFps = (float)Math.Max(maxFps, currentFps);
                    }

                displayFps = (int)Math.Round(currentFps);
                displayText += "FPS: " + displayFps + "\n";
                fpsIndex = (fpsIndex + 1) % fps.Count();
            }
            else
            {
                displayText += "FPS: oo\n";
            }

            //smooth out the timestep a bit.. there was some jumping
            //actually there still is when laptop is unplugged...
            //seems to be an XNA thing as this doesn't happen in other games :S
            deltaTArray[deltaTIndex] = deltaT;
            deltaTIndex = (deltaTIndex + 1) % deltaTArray.Count();
            deltaT = deltaTArray.Sum() / deltaTArray.Count();


            totalNumFrames += 1;
                avgFps = totalNumFrames / totalT;
            totalT += deltaT;

            displayText += "Average FPS: " + (int)Math.Round(avgFps) + "\n";
            displayText += "Minimum FPS: " + (int)Math.Round(minFps) + "\n";
            displayText += "Maximum FPS: " + (int)Math.Round(maxFps) + "\n";

            terrain.update();
			updateCamera();
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
            if (REFLECT_WORLD)
            {
                //TODO: refactor duplicated code
                GraphicsDevice.SetRenderTarget(0, reflectTarget);
                GraphicsDevice.SetRenderTarget(1, null);


                GraphicsDevice.RenderState.AlphaBlendEnable = false;
                GraphicsDevice.RenderState.CullMode = CullMode.None;
                GraphicsDevice.RenderState.DepthBufferEnable = false;
                GraphicsDevice.RenderState.AlphaTestEnable = false;

                GraphicsDevice.Clear(Color.White);
                skyShader.Parameters["World"].SetValue(Matrix.Identity);
                skyShader.Parameters["View"].SetValue(reflectedSkyView);
                skyShader.Parameters["Projection"].SetValue(projection);

                GraphicsDevice.VertexDeclaration = sunVertexDeclaration;

                skyShader.CommitChanges();
                skyShader.Begin();
                skyShader.CurrentTechnique.Passes[0].Begin();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, skyVertices, 0, 10);
                skyShader.CurrentTechnique.Passes[0].End();
                skyShader.End();

                GraphicsDevice.RenderState.AlphaBlendEnable = false;
                GraphicsDevice.RenderState.AlphaTestEnable = false;
                GraphicsDevice.VertexDeclaration = terrainVertexDeclaration;
                GraphicsDevice.RenderState.DepthBufferEnable = true;
                GraphicsDevice.Indices = terrain.iBuffer;
                GraphicsDevice.Vertices[0].SetSource(terrain.vBuffer, 0, VertexPosNormalTanBinormal.SizeInBytes);
                GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
                GraphicsDevice.SamplerStates[1].MaxAnisotropy = 4;
                //GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

                // Set the WVP matrices in the shader
                terrainShader.Parameters["World"].SetValue(terrain.world);
                terrainShader.Parameters["View"].SetValue(reflectedView);
                terrainShader.Parameters["Projection"].SetValue(projection);
                terrainShader.Parameters["CameraPos"].SetValue(camerapos);
                terrainShader.CommitChanges();

                terrainShader.Begin();
                terrainShader.CurrentTechnique.Passes[0].Begin();
                terrain.drawPrimitives();
                terrainShader.CurrentTechnique.Passes[0].End();
                terrainShader.End();
            }

            GraphicsDevice.SetRenderTarget(0, terrainTarget);
            GraphicsDevice.SetRenderTarget(1, offsetTarget);
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            DepthStencilBuffer old = GraphicsDevice.DepthStencilBuffer;
            GraphicsDevice.DepthStencilBuffer = depthBuffer;

            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.AlphaTestEnable = false;
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.RenderState.DepthBufferEnable = false;
            GraphicsDevice.VertexDeclaration = sunVertexDeclaration;
            clearShader.Begin();
            clearShader.CurrentTechnique.Passes[0].Begin();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, FullScreenQuad, 0, 2);
            clearShader.CurrentTechnique.Passes[0].End();
            clearShader.End();

            skyShader.Parameters["World"].SetValue(Matrix.Identity);
            skyShader.Parameters["View"].SetValue(skyView);
            skyShader.Parameters["Projection"].SetValue(projection);

            GraphicsDevice.VertexDeclaration = sunVertexDeclaration;

            skyShader.CommitChanges();
            skyShader.Begin();
            skyShader.CurrentTechnique.Passes[0].Begin();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, skyVertices, 0, 10);
            skyShader.CurrentTechnique.Passes[0].End();
            skyShader.End();

            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.AlphaTestEnable = false;
			GraphicsDevice.VertexDeclaration = terrainVertexDeclaration;
			GraphicsDevice.RenderState.DepthBufferEnable = true;
			GraphicsDevice.Indices = terrain.iBuffer;
			GraphicsDevice.Vertices[0].SetSource(terrain.vBuffer, 0, VertexPosNormalTanBinormal.SizeInBytes);
            GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            GraphicsDevice.SamplerStates[1].MaxAnisotropy = 4;
			//GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

			// Set the WVP matrices in the shader
			terrainShader.Parameters["World"].SetValue(terrain.world);
			terrainShader.Parameters["View"].SetValue(view);
			terrainShader.Parameters["Projection"].SetValue(projection);
			terrainShader.Parameters["CameraPos"].SetValue(camerapos);
			terrainShader.CommitChanges();
            
			terrainShader.Begin();
			terrainShader.CurrentTechnique.Passes[1].Begin();
            terrain.drawPrimitives();
			terrainShader.CurrentTechnique.Passes[1].End();
			terrainShader.End();

            GraphicsDevice.VertexDeclaration = skyVertexDeclaration;
            this.graphics.GraphicsDevice.SamplerStates[0].MaxAnisotropy = 4;

            waterShader.Parameters["World"].SetValue(world);
            waterShader.Parameters["View"].SetValue(view);
            waterShader.Parameters["Projection"].SetValue(projection);
            waterShader.Parameters["ViewAngleProjection"].SetValue(skyView * projection);
            waterShader.Parameters["t"].SetValue(totalT);
            waterShader.Parameters["CameraPos"].SetValue(camerapos);
            
            if (REFLECT_WORLD)
            {
                waterShader.Parameters["ReflectTexture"].SetValue(reflectTarget.GetTexture());
            }

            waterShader.CommitChanges();

            GraphicsDevice.RenderState.AlphaBlendEnable = true;
            GraphicsDevice.RenderState.AlphaTestEnable = true;
            GraphicsDevice.RenderState.SourceBlend = Blend.InverseSourceAlpha;
            GraphicsDevice.RenderState.DestinationBlend = Blend.SourceAlpha;
            GraphicsDevice.RenderState.CullMode = CullMode.None;

            waterShader.Begin();
            waterShader.CurrentTechnique.Passes[0].Begin();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, WaterVertices, 0, 2);
            waterShader.CurrentTechnique.Passes[0].End();

            GraphicsDevice.RenderState.SourceBlend = Blend.One;
            GraphicsDevice.RenderState.DestinationBlend = Blend.One;

            waterShader.CurrentTechnique.Passes[1].Begin();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, WaterVertices, 0, 2);
            waterShader.CurrentTechnique.Passes[1].End();

            waterShader.End();

            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            GraphicsDevice.RenderState.AlphaTestEnable = false;

			GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            GraphicsDevice.SetRenderTarget(0, sceneTarget);
            GraphicsDevice.SetRenderTarget(1, null);
            GraphicsDevice.DepthStencilBuffer = depthBufferScene;
            GraphicsDevice.Clear(Color.White);

            Texture2D terrainTex = terrainTarget.GetTexture();
            Texture2D offsetTex = offsetTarget.GetTexture();


            GraphicsDevice.Vertices[0].SetSource(vBufferOffset, 0, VertexPosition2D.SizeInBytes);
            GraphicsDevice.Indices = iBufferOffset;

            offsetShader.Parameters["sceneTexture"].SetValue(terrainTex);
            offsetShader.Parameters["offsetTexture"].SetValue(offsetTex);
            offsetShader.CommitChanges();
            
            offsetShader.Begin();
            offsetShader.CurrentTechnique.Passes[0].Begin();
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, offsetVertices.Count, 0, offsetIndices.Count / 3);
            offsetShader.CurrentTechnique.Passes[0].End();
            offsetShader.End();
            
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.VertexDeclaration = sunVertexDeclaration;

            fullScreenTextureShader.Parameters["sceneTexture"].SetValue(terrainTex);
            fullScreenTextureShader.CommitChanges();
            fullScreenTextureShader.Begin();
            fullScreenTextureShader.CurrentTechnique.Passes[0].Begin();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, FullScreenQuad, 0, 2);
            fullScreenTextureShader.CurrentTechnique.Passes[0].End();
            fullScreenTextureShader.End();

            GraphicsDevice.SetRenderTarget(0, null);
            //GraphicsDevice.SetRenderTarget(1, null);
            GraphicsDevice.DepthStencilBuffer = old;

            Texture2D sceneTex = sceneTarget.GetTexture();

            toneMapShader.Parameters["sceneTexture"].SetValue(sceneTex);
            toneMapShader.Begin();
            toneMapShader.CurrentTechnique.Passes[0].Begin();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, FullScreenQuad, 0, 2);
            toneMapShader.CurrentTechnique.Passes[0].End();
            toneMapShader.End();

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.DrawString(font, displayText, new Vector2(5, 5), Color.Black);
            spriteBatch.End();

			base.Draw(gameTime);
		}


		private void initializeCamera()
		{
                this.camerapos = new Vector3(56262, 620, 4262);
			this.cameraPitch = 0;
			this.cameraYaw = -(float)Math.PI / 2;
			this.up = Vector3.Up;

			this.view = Matrix.Identity;
            this.reflectedView = Matrix.Identity;
			this.world = Matrix.Identity;
            this.skyView = Matrix.Identity;
            this.reflectedSkyView = Matrix.Identity;
            
			float anglerange = VIEWING_ANGLE;
			float aspect = (float)GraphicsDevice.Viewport.Width /
					(float)GraphicsDevice.Viewport.Height;
			Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(anglerange), aspect, NEAR, FAR, out projection);
		}

		private void updateCamera()
		{
            
                MouseState mouseState = Mouse.GetState();
                if (IsActive)
                {
                    Mouse.SetPosition(GraphicsDevice.PresentationParameters.BackBufferWidth / 2,
                                      GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
                    int dx = mouseState.X - GraphicsDevice.PresentationParameters.BackBufferWidth / 2;
                    int dy = mouseState.Y - GraphicsDevice.PresentationParameters.BackBufferHeight / 2;
                    cameraYaw = (float)((cameraYaw - (dx * MOUSE_SENSITIVITY) / 360) % (2 * Math.PI));
                    cameraPitch = (float)Math.Min(Math.PI / 2.05, Math.Max(-Math.PI / 2.05, cameraPitch + (dy * MOUSE_SENSITIVITY) / 360));
                }
                world = Matrix.CreateTranslation((float)Math.Round(camerapos.X / Terrain.LOD_CELL_WIDTH) * Terrain.LOD_CELL_WIDTH,
                                                  0,
                                                 (float)Math.Round(camerapos.Z / Terrain.LOD_CELL_WIDTH) * Terrain.LOD_CELL_WIDTH);

                Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0);
                Vector3 deltaPos = Vector3.Zero;
                Vector3 lookatPos = Vector3.Transform(-Vector3.Forward, rotationMatrix);

                KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.W))
                {
                    deltaPos -= Vector3.Forward;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    deltaPos -= Vector3.Backward;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    deltaPos -= Vector3.Left;
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    deltaPos -= Vector3.Right;
                }

                if (deltaPos != Vector3.Zero)
                {
                    deltaPos = Vector3.Transform(Vector3.Normalize(deltaPos), rotationMatrix);
                }

                float speed = 0;
                if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                {
                    speed = 1000;
                }
                else if (keyboardState.IsKeyDown(Keys.Space))
                {
                    speed = 100;
                }
                else
                {
                    speed = 20;
                }

                camerapos += deltaPos * deltaT * speed;

                reflectedCameraPos = new Vector3(camerapos.X, WATER_LEVEL - (camerapos.Y - WATER_LEVEL), camerapos.Z);
                Vector3 reflectedLookAtPos = new Vector3(lookatPos.X, -lookatPos.Y, lookatPos.Z);

                view = Matrix.CreateLookAt(camerapos, camerapos + lookatPos, up);
                reflectedView = Matrix.CreateLookAt(reflectedCameraPos, reflectedCameraPos + reflectedLookAtPos, up);
                skyView = Matrix.CreateLookAt(Vector3.Zero, lookatPos, up);
                reflectedSkyView = Matrix.CreateLookAt(Vector3.Zero, reflectedLookAtPos, up);
        }

        void generateOffsetVertices()
        {
            List<VertexPosition2D> vertices = new List<VertexPosition2D>();
            List<int> indices = new List<int>();

            int OFFSET_VERT_HEIGHT = SCREEN_HEIGHT / 1;
            int OFFSET_VERT_WIDTH = SCREEN_WIDTH / 1;

            for (int y = 0; y < OFFSET_VERT_HEIGHT; y++)
            {
                for (int x = 0; x < OFFSET_VERT_WIDTH; x++)
                {
                    vertices.Add(new VertexPosition2D(new Vector2(((x * 2.0f) / (OFFSET_VERT_WIDTH - 1)) - 1, ((y * 2.0f) / (OFFSET_VERT_HEIGHT - 1)) - 1)));
                    if (x < OFFSET_VERT_WIDTH - 1 && y < OFFSET_VERT_HEIGHT - 1)
                    {
                        indices.Add(x + y * OFFSET_VERT_WIDTH);
                        indices.Add(x + y * OFFSET_VERT_WIDTH + 1);
                        indices.Add(x + (y + 1) * OFFSET_VERT_WIDTH);
                        indices.Add(x + (y + 1) * OFFSET_VERT_WIDTH);
                        indices.Add(x + y * OFFSET_VERT_WIDTH + 1);
                        indices.Add(x + (y + 1) * OFFSET_VERT_WIDTH + 1);
                    }
                }
            }

            offsetVertices = vertices;
            offsetIndices = indices;
        }

        void generateFullScreenQuad()
        {
            Vector2 textureOffset = new Vector2(0.5f / SCREEN_WIDTH, 0.5f / SCREEN_HEIGHT);

            FullScreenQuad = new VertexPositionTexture[4];
            FullScreenQuad[0] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1) + textureOffset);
            FullScreenQuad[1] = new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0) + textureOffset);
            FullScreenQuad[2] = new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1) + textureOffset);
            FullScreenQuad[3] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0) + textureOffset);
        }

        void generateWater()
        {
            WaterVertices = new VertexPositionColor[4];
            WaterVertices[0] = new VertexPositionColor(new Vector3(-FAR, WATER_LEVEL, -FAR), Color.White);
            WaterVertices[1] = new VertexPositionColor(new Vector3(-FAR, WATER_LEVEL, FAR), Color.White);
            WaterVertices[2] = new VertexPositionColor(new Vector3(FAR, WATER_LEVEL, -FAR), Color.White);
            WaterVertices[3] = new VertexPositionColor(new Vector3(FAR, WATER_LEVEL, FAR), Color.White);
        }
    }
}
