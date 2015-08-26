using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalTextureTangentBinormalLight : IVertexType, IVertexTypeTangentBinormal
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 TextureCoordinate { get; set; }
        public Vector3 Tangent { get; set; }
        public Vector3 Binormal { get; set; }
        public float SunLight { get; set; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,    VertexElementFormat.Vector3,    VertexElementUsage.Position,            0),
            new VertexElement(12,   VertexElementFormat.Vector3,    VertexElementUsage.Normal,              0),
            new VertexElement(24,   VertexElementFormat.Vector2,    VertexElementUsage.TextureCoordinate,   0),
            new VertexElement(32,   VertexElementFormat.Vector3,    VertexElementUsage.Tangent,             0),
            new VertexElement(44,   VertexElementFormat.Vector3,    VertexElementUsage.Binormal,            0),
            new VertexElement(56,   VertexElementFormat.Single,     VertexElementUsage.Color,               0));

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public VertexPositionNormalTextureTangentBinormalLight(Vector3 position, Vector3 normal, Vector2 textureCoordinate, float sunLight) : this()
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            SunLight = sunLight;
        }

        public static int SizeInBytes { get { return 64; } }
    }
}