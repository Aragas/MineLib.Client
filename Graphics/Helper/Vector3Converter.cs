namespace MineLib.PCL.Graphics.Helper
{
    public static class Vector3Converter
    {
        public static MineLib.Network.Data.Vector3 ToMineLibVector3(this Microsoft.Xna.Framework.Vector3 vector)
        {
            return new MineLib.Network.Data.Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Microsoft.Xna.Framework.Vector3 ToXNAVector3(this MineLib.Network.Data.Position pos)
        {
            return new Microsoft.Xna.Framework.Vector3(pos.X, pos.Y, pos.Z);
        }

        public static Microsoft.Xna.Framework.Vector3 ToXNAVector3(this MineLib.Network.Data.Vector3 vector)
        {
            return new Microsoft.Xna.Framework.Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
