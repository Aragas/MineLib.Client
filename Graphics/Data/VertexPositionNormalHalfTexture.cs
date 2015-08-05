using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace MineLib.PCL.Graphics.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexPositionColorHalfTexture : IVertexType
	{
		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
			new VertexElement(0,	VertexElementFormat.Vector3,		VertexElementUsage.Position,			0),
			new VertexElement(12,	VertexElementFormat.Color,			VertexElementUsage.Color,				0), 
			new VertexElement(16,	VertexElementFormat.HalfVector2,	VertexElementUsage.TextureCoordinate,	0));

		public Vector3 Position;
	    //public Vector3 Normal;
		public Color Color;
		public HalfVector2 TextureCoordinate;

		VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public VertexPositionColorHalfTexture(Vector3 position, Color color, Vector2 textureCoordinate)
		{
			Position = position;
			Color = color;
			TextureCoordinate = new HalfVector2(textureCoordinate.X, textureCoordinate.Y);
		}

        public VertexPositionColorHalfTexture(Vector3 position, Color color, HalfVector2 textureCoordinate)
		{
			Position = position;
			Color = color;
			TextureCoordinate = textureCoordinate;
		}

		public static bool operator ==(VertexPositionColorHalfTexture left, VertexPositionColorHalfTexture right)
		{
			if (left.Position == right.Position && left.Color == right.Color)
				return left.TextureCoordinate == right.TextureCoordinate;
			return false;
		}

		public static bool operator !=(VertexPositionColorHalfTexture left, VertexPositionColorHalfTexture right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			return "{{Position:" + Position + " Normal:" + (string) (object)Color + " TextureCoordinate:" + (string) (object) TextureCoordinate + "}}";
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
				return false;
			return this == (VertexPositionColorHalfTexture) obj;
		}
	}
}
