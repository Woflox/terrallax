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

        public VertexPosNormalTanBinormal[] vertices;
        public List<int> indices;

        public ReadyState readyState;
        public Vector2 basePosition;
        public AreaParameters parameters;

        Dictionary<Vector2, int> LODPoints;
        TerrainVertexData cachedData;

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
        }

        public void generate(Vector2 basePosition, TerrainVertexData cachedData, AreaParameters parameters, bool async)
        {
            this.basePosition = basePosition;
            this.cachedData = cachedData;
            this.parameters = parameters;
            if (!async)
            {
                generate();
            }
            else
            {
                new Thread(generate).Start();
            }
        }

        private void generate()
        {
            Vector2 relativePosition = Vector2.Zero;
            if (cachedData != null)
            {
                relativePosition = basePosition - cachedData.basePosition;
            }
            readyState = ReadyState.Loading;
            int numCached = 0;
            int numGenerated = 0;
            for (int i = 0; i < vertices.Length; i++)
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
            Console.WriteLine("NumCached: " + numCached + "NumGenerated: " + numGenerated);
            readyState = ReadyState.Ready;
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
