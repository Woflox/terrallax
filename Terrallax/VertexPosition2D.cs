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
	public struct VertexPosition2D
	{
		public Vector2 Position;

        public static int SizeInBytes { get { return sizeof(float)*2; } }

        public VertexPosition2D(Vector2 position)
        {
            this.Position = position;
        }

        public readonly static VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0)
        };
	}
}
