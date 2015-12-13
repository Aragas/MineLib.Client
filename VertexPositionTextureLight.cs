using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTextureLight : IVertexType
    {
        public Vector3 Position { get; set; }
        public Vector2 TextureCoordinate { get; set; }
        public float SunLight { get; set; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,    VertexElementFormat.Vector3,    VertexElementUsage.Position,            0),
            new VertexElement(12,   VertexElementFormat.Vector2,    VertexElementUsage.TextureCoordinate,   0),
            new VertexElement(20,   VertexElementFormat.Single,     VertexElementUsage.Color,               0));

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionTextureLight(Vector3 position, Vector2 textureCoordinate, float sunLight) : this()
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            SunLight = sunLight;
        }

        public static int SizeInBytes => 24;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTextureLight1 : IVertexType
    {
        Vector3 Position { get; set; }
        Vector2 TextureCoordinate { get; set; }
        float SunLight { get; set; }
        float LightType { get; set; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,    VertexElementFormat.Vector3,    VertexElementUsage.Position,            0),
            new VertexElement(12,   VertexElementFormat.Vector2,    VertexElementUsage.TextureCoordinate,   0),
            new VertexElement(20,   VertexElementFormat.Single,     VertexElementUsage.Color,               0),
            new VertexElement(24,   VertexElementFormat.Single,     VertexElementUsage.Color,               1));

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionTextureLight1(Vector3 position, Vector2 textureCoordinate, float sunLight, byte lightType) : this()
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            SunLight = sunLight;
            LightType = lightType;
        }

        public static int SizeInBytes => 28;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTextureLight2 : IVertexType
    {
        Vector3 Position { get; set; }
        Vector2 TextureCoordinate { get; set; }
        float SunLight { get; set; }
        Vector3 LocalLight { get; set; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0,    VertexElementFormat.Vector3,    VertexElementUsage.Position,            0),
            new VertexElement(12,   VertexElementFormat.Vector2,    VertexElementUsage.TextureCoordinate,   0),
            new VertexElement(20,   VertexElementFormat.Single,     VertexElementUsage.Color,               0),
            new VertexElement(24,   VertexElementFormat.Color,      VertexElementUsage.Color,               1));

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionTextureLight2(Vector3 position, Vector2 textureCoordinate, float sunLight, Vector3 localLight) : this()
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            SunLight = sunLight;
            LocalLight = localLight;
        }

        public static int SizeInBytes => 40;
    }
}
