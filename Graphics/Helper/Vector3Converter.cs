namespace MineLib.Client.Graphics.Helper
{
    public static class Vector3Converter
    {
        public static Microsoft.Xna.Framework.Vector3 ToXNAVector3(this Network.Data.Vector3 vector)
        {
            return new Microsoft.Xna.Framework.Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Network.Data.Vector3 ToMineLibVector3(this Microsoft.Xna.Framework.Vector3 vector)
        {
            return new Network.Data.Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Microsoft.Xna.Framework.Vector3 ToXNAVector3(this Network.Data.Position pos)
        {
            return new Microsoft.Xna.Framework.Vector3(pos.X, pos.Y, pos.Z);
        }
    }
}
