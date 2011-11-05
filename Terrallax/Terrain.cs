using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
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

    public class Terrain
    {
        public const float CELL_WIDTH = 100;
        public const float LOD_CELL_WIDTH = 100;
        public const int NUM_CELLS = 128;
        public const int NUM_LODS = 7;


        public List<VertexPosition2D> LODGrid;
        public List<int> LODIndices;

        public TerrainVertexData currentVertexData;
        public TerrainVertexData nextVertexData;


        public VertexBuffer vBuffer;
        public IndexBuffer iBuffer;

        public Matrix world;

        int currentIndex = 0;

        Dictionary<Vector2, int> LODPoints;

        public Terrain()
        {
            Noise3.init();

            LODGrid = new List<VertexPosition2D>();
            LODIndices = new List<int>();
            LODPoints = new Dictionary<Vector2, int>();
            generateLODGrid(NUM_LODS, NUM_CELLS, CELL_WIDTH);

            vBuffer = new VertexBuffer(Game1.instance.GraphicsDevice, LODGrid.Count * VertexPosNormalTanBinormal.SizeInBytes, BufferUsage.WriteOnly);
            iBuffer = new IndexBuffer(Game1.instance.GraphicsDevice, sizeof(int) * LODIndices.Count, BufferUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
            iBuffer.SetData(LODIndices.ToArray());

            currentVertexData = new TerrainVertexData(LODGrid, LODIndices, LODPoints);
            nextVertexData = new TerrainVertexData(LODGrid, LODIndices, LODPoints);

            currentVertexData.generate(new Vector2((float)Math.Round(Game1.instance.camerapos.X / Terrain.LOD_CELL_WIDTH) * Terrain.LOD_CELL_WIDTH,
                                 (float)Math.Round(Game1.instance.camerapos.Z / Terrain.LOD_CELL_WIDTH) * Terrain.LOD_CELL_WIDTH), null);
            vBuffer.SetData(currentVertexData.vertices);
        }
        int numTicks = 0;
        public void update()
        {
            Vector2 LODCellPosition = new Vector2((float)Math.Round(Game1.instance.camerapos.X / Terrain.LOD_CELL_WIDTH) * Terrain.LOD_CELL_WIDTH,
                                 (float)Math.Round(Game1.instance.camerapos.Z / Terrain.LOD_CELL_WIDTH) * Terrain.LOD_CELL_WIDTH);
            numTicks++;
            if (LODCellPosition != currentVertexData.basePosition)
            {
                if (nextVertexData.readyState == TerrainVertexData.ReadyState.Idle)
                {
                    nextVertexData.generate(LODCellPosition, currentVertexData);
                    numTicks = 0;
                }
                else if (nextVertexData.readyState == TerrainVertexData.ReadyState.Ready)
                {
                    TerrainVertexData tmp = currentVertexData;
                    currentVertexData = nextVertexData;
                    nextVertexData = tmp;
                    nextVertexData.readyState = TerrainVertexData.ReadyState.Idle;
                    vBuffer.SetData(currentVertexData.vertices);
                    Console.WriteLine(numTicks);
                }
            }

            world = Matrix.CreateTranslation(currentVertexData.basePosition.X,
                                  0,
                                 currentVertexData.basePosition.Y);

        }

        public void drawPrimitives()
        {
            currentVertexData.drawPrimitives();
        }

        void addPolygon(float x1, float y1, float x2, float y2, float x3, float y3, float offset, float scale)
        {
            int index1, index2, index3;
            Vector2 point1 = new Vector2(x1 * scale + offset, y1 * scale + offset);
            Vector2 point2 = new Vector2(x2 * scale + offset, y2 * scale + offset);
            Vector2 point3 = new Vector2(x3 * scale + offset, y3 * scale + offset);

            index1 = getIndex(point1);
            index2 = getIndex(point2);
            index3 = getIndex(point3);

            LODIndices.Add(index1);
            LODIndices.Add(index2);
            LODIndices.Add(index3);
        }

        void generateLODGrid(int LODLevels, int width, float scale)
        {

            float offset = -scale * width / 2;
            for (int LODLevel = 0; LODLevel < LODLevels; LODLevel++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        int edge1 = (width / 4) - 1;
                        int edge2 = (width / 4) * 3;

                        if (LODLevel == LODLevels - 1 ||
                            Math.Min(x, y) < edge1 || Math.Max(x, y) > edge2 ||
                            ((x == edge1 || x == edge2) && (y == edge1 || y == edge2)))
                        {
                            //draw a normal square
                            addPolygon(x, y, x + 1, y, x, y + 1, offset, scale);
                            addPolygon(x, y + 1, x + 1, y, x + 1, y + 1, offset, scale);
                        }
                        else if (x == edge1)
                        {
                            //draw a left edge square
                            addPolygon(x, y, x + 1, y, x, y + 1, offset, scale);
                            addPolygon(x + 1, y, x + 1, y + 0.5f, x, y + 1, offset, scale);
                            addPolygon(x + 1, y + 0.5f, x + 1, y + 1, x, y + 1, offset, scale);
                        }
                        else if (x == edge2)
                        {
                            //draw a right edge square
                            addPolygon(x, y, x + 1, y, x, y + 0.5f, offset, scale);
                            addPolygon(x, y + 0.5f, x + 1, y, x, y + 1, offset, scale);
                            addPolygon(x, y + 1, x + 1, y, x + 1, y + 1, offset, scale);
                        }
                        else if (y == edge1)
                        {
                            //draw a top edge square
                            addPolygon(x, y, x + 1, y, x, y + 1, offset, scale);
                            addPolygon(x + 1, y, x + 0.5f, y + 1, x, y + 1, offset, scale);
                            addPolygon(x + 1, y, x + 1, y + 1, x + 0.5f, y + 1, offset, scale);
                        }
                        else if (y == edge2)
                        {
                            //draw a bottom edge square
                            addPolygon(x, y, x + 0.5f, y, x, y + 1, offset, scale);
                            addPolygon(x + 0.5f, y, x + 1, y, x, y + 1, offset, scale);
                            addPolygon(x + 1, y, x + 1, y + 1, x, y + 1, offset, scale);
                        }
                    }
                }
                offset += (scale * width) / 4;
                scale /= 2;
            }
            LODIndices.Reverse(); //we want to draw the close up LODs first to decrease overdraw
        }

        int getIndex(Vector2 point)
        {
            if (LODPoints.ContainsKey(point))
            {
                return (int)LODPoints[point];
            }
            else
            {

                LODGrid.Add(new VertexPosition2D(new Vector2(point.X, point.Y)));
                LODPoints.Add(point, currentIndex);
                return currentIndex++;
            }
        }

        public float getHeightForPoint(Vector2 point)
        {
            return 0;
        }
    }
}
