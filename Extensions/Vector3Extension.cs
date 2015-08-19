using System;

using MineLib.Core.Data;

namespace MineLib.PGL.Extensions
{
    public static class Vector3Extension
    {
        public static Vector3 ToMineLibVector3(this Microsoft.Xna.Framework.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Position ToMineLibPosition(this Microsoft.Xna.Framework.Vector3 vector)
        {
            return new Position((int)Math.Floor(vector.X), (int)Math.Floor(vector.Y), (int)Math.Floor(vector.Z));
        }


        public static Microsoft.Xna.Framework.Vector3 ToXNAVector3(this Position pos)
        {
            return new Microsoft.Xna.Framework.Vector3(pos.X, pos.Y, pos.Z);
        }

        public static Microsoft.Xna.Framework.Vector3 ToXNAVector3(this Vector3 vector)
        {
            return new Microsoft.Xna.Framework.Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
