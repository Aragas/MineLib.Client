using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.PGL.World;

namespace MineLib.PGL.Extensions
{
    public static class BoundingFrustumExtension
    {
        public static bool FastIntersect(this BoundingFrustum frustum, BoundingBox box)
        {
            Vector3 normal = frustum.Left.Normal;
            Vector3 vector2;
            vector2.X = (normal.X >= 0f) ? box.Min.X : box.Max.X;
            vector2.Y = (normal.Y >= 0f) ? box.Min.Y : box.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? box.Min.Z : box.Max.Z;
            if ((((frustum.Left.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            normal = frustum.Right.Normal;
            vector2.X = (normal.X >= 0f) ? box.Min.X : box.Max.X;
            vector2.Y = (normal.Y >= 0f) ? box.Min.Y : box.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? box.Min.Z : box.Max.Z;
            if ((((frustum.Right.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            normal = frustum.Bottom.Normal;
            vector2.X = (normal.X >= 0f) ? box.Min.X : box.Max.X;
            vector2.Y = (normal.Y >= 0f) ? box.Min.Y : box.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? box.Min.Z : box.Max.Z;
            if ((((frustum.Bottom.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            normal = frustum.Top.Normal;
            vector2.X = (normal.X >= 0f) ? box.Min.X : box.Max.X;
            vector2.Y = (normal.Y >= 0f) ? box.Min.Y : box.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? box.Min.Z : box.Max.Z;
            if ((((frustum.Top.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            return true;
        }

        public static bool FastIntersect<T>(this BoundingFrustum frustum, ref SectionVBO section) where T : struct, IVertexType
        {
            Vector3 vector2;
            Vector3 normal = frustum.Left.Normal;
            vector2.X = (normal.X >= 0f) ? section.BoundingBox.Min.X : section.BoundingBox.Max.X;
            vector2.Y = (normal.Y >= 0f) ? section.BoundingBox.Min.Y : section.BoundingBox.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? section.BoundingBox.Min.Z : section.BoundingBox.Max.Z;
            if ((((frustum.Left.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            normal = frustum.Right.Normal;
            vector2.X = (normal.X >= 0f) ? section.BoundingBox.Min.X : section.BoundingBox.Max.X;
            vector2.Y = (normal.Y >= 0f) ? section.BoundingBox.Min.Y : section.BoundingBox.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? section.BoundingBox.Min.Z : section.BoundingBox.Max.Z;
            if ((((frustum.Right.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            normal = frustum.Bottom.Normal;
            vector2.X = (normal.X >= 0f) ? section.BoundingBox.Min.X : section.BoundingBox.Max.X;
            vector2.Y = (normal.Y >= 0f) ? section.BoundingBox.Min.Y : section.BoundingBox.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? section.BoundingBox.Min.Z : section.BoundingBox.Max.Z;
            if ((((frustum.Bottom.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            normal = frustum.Top.Normal;
            vector2.X = (normal.X >= 0f) ? section.BoundingBox.Min.X : section.BoundingBox.Max.X;
            vector2.Y = (normal.Y >= 0f) ? section.BoundingBox.Min.Y : section.BoundingBox.Max.Y;
            vector2.Z = (normal.Z >= 0f) ? section.BoundingBox.Min.Z : section.BoundingBox.Max.Z;
            if ((((frustum.Top.D + (normal.X * vector2.X)) + (normal.Y * vector2.Y)) + (normal.Z * vector2.Z)) > 0f)
                return false;
            
            return true;
        }
    }
}