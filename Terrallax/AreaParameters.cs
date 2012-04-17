using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terrallax
{
    public struct AreaParameters
    {
        public float vScale;
        public float hScale;
        public float vOffset;

        //mf params
        public int octaves;
        public float spectral_exp;
        public float lacunarity;
        public float offset;
        public float threshold;

        public static AreaParameters DefaultParameters()
        {
            AreaParameters parameters = new AreaParameters();
            parameters.vScale = 400;
            parameters.hScale = 2300;
            parameters.vOffset = 300;
            parameters.octaves = 8;
            parameters.spectral_exp = 0.75f;
            parameters.lacunarity = 2.2f;
            parameters.offset = 1.5f;
            parameters.threshold = 0.9f;
            return parameters;
        }

        public static bool operator ==(AreaParameters x, AreaParameters y)
        {
            return x.vScale == y.vScale &&
                x.hScale == y.hScale &&
                x.vOffset == y.vOffset &&
                x.octaves == y.octaves &&
                x.spectral_exp == y.spectral_exp &&
                x.lacunarity == y.lacunarity &&
                x.offset == y.offset &&
                x.threshold == y.threshold;
        }
        public static bool operator !=(AreaParameters x, AreaParameters y)
        {
            return !(x == y);
        }
    }
}
