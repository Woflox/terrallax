#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using System.Text;
#endregion

namespace Terrallax
{
    public static class Skybox
    {
        public static VertexPositionTexture[] createSkybox()
        {
            Vector3 v1 = new Vector3(1, 1, 1);
            Vector3 v2 = new Vector3(1, -0.2f, 1);
            Vector3 v3 = new Vector3(-1, 1, 1);
            Vector3 v4 = new Vector3(-1, -0.2f, 1);
            Vector3 v5 = new Vector3(1, 1, -1);
            Vector3 v6 = new Vector3(1, -0.2f, -1);
            Vector3 v7 = new Vector3(-1, 1, -1);
            Vector3 v8 = new Vector3(-1, -0.2f, -1);

            VertexPositionTexture[] vs = new VertexPositionTexture[30];
            createFace(v1, v2, v3, v4, vs, 0);
            createFace(v5, v6, v1, v2, vs, 6);
            createFace(v7, v3, v5, v1, vs, 12);
            createFace(v3, v4, v7, v8, vs, 18);
            createFace(v7, v8, v5, v6, vs, 24);

            return vs;
        }

        public static VertexPositionTexture[] createSun(float size, Vector3 lightingDir)
        {
            Vector3 dir = -lightingDir;
            Vector3 dx = Vector3.Cross(dir, Vector3.UnitY);
            Vector3 dy = Vector3.Cross(dir, dx);
            VertexPositionTexture[] vs = new VertexPositionTexture[4];
            vs[0] = new VertexPositionTexture(dir - dx - dy, new Vector2(0, 0));
            vs[1] = new VertexPositionTexture(dir + dx - dy, new Vector2(1, 0));
            vs[2] = new VertexPositionTexture(dir - dx + dy, new Vector2(0, 1));
            vs[3] = new VertexPositionTexture(dir + dx + dy, new Vector2(1, 1));
            return vs;
        }

        private static void createFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, VertexPositionTexture[] vs, int offset){
            vs[offset] = new VertexPositionTexture(v1, new Vector2(0, 0));
            vs[offset + 1] = new VertexPositionTexture(v2, new Vector2(1, 0));
            vs[offset + 2] = new VertexPositionTexture(v3, new Vector2(0, 1));
            vs[offset + 3] = new VertexPositionTexture(v2, new Vector2(1, 0));
            vs[offset + 4] = new VertexPositionTexture(v3, new Vector2(0, 1));
            vs[offset + 5] = new VertexPositionTexture(v4, new Vector2(1, 1));
        }
    }
}
