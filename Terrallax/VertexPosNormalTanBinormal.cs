using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrallax
{
    public struct VertexPosNormalTanBinormal
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 Binormal;

        public VertexPosNormalTanBinormal(Vector3 position, Vector3 normal, Vector3 tangent, Vector3 binormal)
        {
            Position = position;
            Normal = normal;
            Tangent = tangent;
            Binormal = binormal;
        }

        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Vector3,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Normal, 0),

            new VertexElement(0, 24, VertexElementFormat.Vector3,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Tangent, 0),

            new VertexElement(0, 36, VertexElementFormat.Vector3,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Binormal, 0),
        };

        public const int SizeInBytes = 48;
    }
}