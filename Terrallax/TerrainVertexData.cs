using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics.PackedVector; //for NormalizedByte4

namespace Terrallax
{
    public class TerrainVertexData
    {
        public enum ReadyState
        {
            Idle,
            Loading,
            Ready
        }

        public enum ThreadState
        {
            Working,
            Done
        }

        public class GenerationCallParameters
        {
            public bool async;
            public int threadIndex;
            public TerrainVertexData cachedData;
        }

        public VertexPosNormalTanBinormal[] vertices;
        public List<int> indices;

        public ReadyState readyState;
        public Vector2 basePosition;
        public AreaParameters parameters;

        public static readonly int NUM_THREADS = 1;

        ThreadState[] threadStates;

        Dictionary<Vector2, int> LODPoints;

        public TerrainVertexData(List<VertexPosition2D> LODGrid, List<int> LODIndices, Dictionary<Vector2, int> LODPoints)
        {
            this.vertices = new VertexPosNormalTanBinormal[LODGrid.Count];
            this.indices = LODIndices;
            this.LODPoints = LODPoints;
            for (int i = 0; i < LODGrid.Count; i++)
            {
                vertices[i] = new VertexPosNormalTanBinormal(new Vector3(LODGrid[i].Position.X, 0, LODGrid[i].Position.Y), Vector3.Zero, Vector3.Zero, Vector3.Zero);
            }
            readyState = ReadyState.Idle;
            threadStates = new ThreadState[NUM_THREADS];
        }

        public void generate(Vector2 basePosition, TerrainVertexData cachedData, AreaParameters parameters, bool async)
        {
            this.basePosition = basePosition;
            this.parameters = parameters;
            readyState = ReadyState.Loading;
            for (int i = 0; i < threadStates.Length; i++)
            {
                threadStates[i] = ThreadState.Working;
            }

            if (!async)
            {
                GenerationCallParameters genParams = new GenerationCallParameters();
                genParams.cachedData = cachedData;
                genParams.async = async;
                genParams.threadIndex = 0;
                generate(genParams);
            }
            else
            {
                for (int i = 0; i < NUM_THREADS; i++)
                {
                    GenerationCallParameters genParams = new GenerationCallParameters();
                    genParams.cachedData = cachedData;
                    genParams.async = async;
                    genParams.threadIndex = i;
                    Thread thread = new Thread(generate);
                    thread.Priority = ThreadPriority.BelowNormal;
                    thread.Start(genParams);
                }
            }
        }

        private void generate(object data)
        {
            GenerationCallParameters genParams = data as GenerationCallParameters;
            TerrainVertexData cachedData = genParams.cachedData;
            bool async = genParams.async;
            int threadIndex = genParams.threadIndex;

            Vector2 relativePosition = Vector2.Zero;
            if (cachedData != null)
            {
                relativePosition = basePosition - cachedData.basePosition;
            }
            int numCached = 0;
            int numGenerated = 0;

            int startIndex = 0;
            int endIndex = vertices.Length - 1;
            if (async && NUM_THREADS > 1)
            {
                int verticesPerThread = vertices.Length / NUM_THREADS;
                startIndex = threadIndex * verticesPerThread;
                if (threadIndex != NUM_THREADS - 1)
                {
                    endIndex = startIndex + verticesPerThread - 1;
                }
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                Vector2 p2DRel = new Vector2(vertices[i].Position.X + relativePosition.X,
                                                                      
                    vertices[i].Position.Z + relativePosition.Y);
                int cachedIndex;

                if (cachedData != null && LODPoints.TryGetValue(p2DRel, out cachedIndex))
                {
                    vertices[i].Position.Y = cachedData.vertices[cachedIndex].Position.Y;
                    vertices[i].Binormal = cachedData.vertices[cachedIndex].Binormal;
                    vertices[i].Tangent = cachedData.vertices[cachedIndex].Tangent;
                    vertices[i].Normal = cachedData.vertices[cachedIndex].Normal;
                    numCached++;
                }
                else
                {
                    float vScale = parameters.vScale;
                    float hScale = parameters.hScale;
                    float offset = parameters.vOffset;

                    Vector2 p2D = new Vector2(vertices[i].Position.X + basePosition.X, vertices[i].Position.Z + basePosition.Y);
                    // calc the height displacement using a multifractal
                    vertices[i].Position.Y = mf(p2D / hScale, parameters) * vScale + offset;

                    //calculate the binormal and tangent by getting the height right next to the point for x and z
                    vertices[i].Binormal = Vector3.Normalize(new Vector3(0.1f, (mf((p2D + new Vector2(0.1f, 0)) / hScale, parameters) * vScale + offset) - vertices[i].Position.Y, 0));
                    vertices[i].Tangent = Vector3.Normalize(new Vector3(0, (mf((p2D + new Vector2(0, 0.1f)) / hScale, parameters) * vScale + offset) - vertices[i].Position.Y, 0.1f));
                    vertices[i].Normal = Vector3.Normalize(Vector3.Cross(vertices[i].Tangent, vertices[i].Binormal));
                    numGenerated++;
                }
            }

            if (async)
            {
                threadStates[threadIndex] = ThreadState.Done;

                bool allThreadsDone = true;

                foreach (ThreadState state in threadStates)
                {
                    if (state == ThreadState.Working)
                    {
                        allThreadsDone = false;
                    }
                }
                if (allThreadsDone)
                {
                    readyState = ReadyState.Ready;
                }
            }
            else
            {
                readyState = ReadyState.Ready;
            }
        }

        public void drawPrimitives()
        {
            Game1.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Count / 3);
        }

        float mf(Vector2 p, AreaParameters parameters)
        {
            return mf(p, parameters.octaves, parameters.spectral_exp, parameters.lacunarity, parameters.offset, parameters.threshold);
        }

        float mf(Vector2 p, int octaves, float spectral_exp, float lacunarity, float offset, float threshold)
        {
            float sum = 0;
            float freq = 1.0f;
            float amp = 0;
            float weight = 1;
            float signal;

            for (int i = octaves; i > 0; i--)
            {
                //Console.WriteLine(Noise3.noise(p.X / 2, p.Y / 2, 0)*5);
                signal = weight * 0.5f * (Noise3.noise(p.X, p.Y) + offset);
                p *= lacunarity;
                freq *= lacunarity;
                amp = (float)Math.Pow(freq, -spectral_exp);
                weight = Math.Max(0, Math.Min(1, (signal * threshold)));
                sum += amp * signal;
            }
            return sum;
        }
    }
}
