using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PCL.Graphics.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTextureLight : IVertexType
    {
        public Vector3 Position { get; private set; }
        public Vector2 TextureCoordinate { get; private set; }
        public float SunLight { get; private set; }
        public Vector3 LocalLight { get; private set; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,    VertexElementFormat.Vector3,    VertexElementUsage.Position,            0),
            new VertexElement(12,   VertexElementFormat.Vector2,    VertexElementUsage.TextureCoordinate,   0),
            new VertexElement(20,   VertexElementFormat.Single,     VertexElementUsage.Color,               0),
            new VertexElement(24,   VertexElementFormat.Vector3,    VertexElementUsage.Color,               1));

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }


        public VertexPositionTextureLight(Vector3 position, Vector2 textureCoordinate, float sunLight, Color localLight) : this()
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            SunLight = sunLight;
            LocalLight = localLight.ToVector3();
        }

        public VertexPositionTextureLight(Vector3 position, Vector2 textureCoordinate, float sunLight, Vector3 localLight) : this()
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            SunLight = sunLight;
            LocalLight = localLight;
        }

        public static int SizeInBytes { get { return 36; } }
    }
}
