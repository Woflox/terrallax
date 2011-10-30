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
        /*const float CELL_WIDTH = 100;
        const float LOD_CELL_WIDTH = 200;
        const int NUM_CELLS = 128;
        const int NUM_LODS = 7;

        List<VertexPosition2D> LODGrid;
        List<VertexPosition2D> offsetVertices;
        */
       // public float getHeightForPoint(Vector2)
        //{
         //   return 0;
       // }
        /*
        float mf(float p, int octaves, float spectral_exp, float lacunarity, float offset, float threshold)
        {
	        float sum = 0;
	        float freq = 1.0f; 
	        float amp = 0;
	        float weight = 1;
	        float signal;
	
	        for(int i=octaves; i>0; i--) 
            {
		        signal = weight * 0.5 * (inoise(p) + offset);
		        p *= lacunarity;
		        freq *= lacunarity;
		        amp = pow(freq, -spectral_exp);
		        weight = saturate(signal*threshold);
		        sum += amp * signal;
	        }
	        return sum;
        }
        */

    }
}
