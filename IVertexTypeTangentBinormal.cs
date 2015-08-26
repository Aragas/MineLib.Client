using Microsoft.Xna.Framework;

namespace MineLib.PGL
{
    public interface IVertexTypeTangentBinormal
    {
        Vector3 Position { get; set; }
        Vector2 TextureCoordinate { get; set; }
        Vector3 Normal { get; set; }

        Vector3 Tangent { get; set; }
        Vector3 Binormal { get; set; }
    }
}