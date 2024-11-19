using System;
using Lucky.Kits.Utilities;
using UnityEngine;
using static Lucky.Kits.Utilities.MathUtils;

namespace Lucky.Kits.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 WithX(this Vector2 orig, float x)
        {
            orig.x = x;
            return orig;
        }
        
        public static Vector2 WithY(this Vector2 orig, float y)
        {
            orig.y = y;
            return orig;
        }

        public static Vector3 WithZ(this Vector2 orig, float z)
        {
            return new Vector3(orig.x, orig.y, z);
        }

        public static Vector2 Sign(this Vector2 orig)
        {
            return new Vector2(Mathf.Sign(orig.x), Mathf.Sign(orig.y));
        }

        public static void Deconstruct(this Vector2 vector, out float x, out float y)
        {
            x = vector.x;
            y = vector.y;
        }

        public static Vector2 Rotate(this Vector2 vec, float angleRadians)
        {
            return RadiansToVector(vec.Radians() + angleRadians, vec.magnitude);
        }

        public static float Radians(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.y, vector.x);
        }
        
        
        /// <summary>
        /// 逆时针转90度, reverse则顺时针转
        /// 由于坐标系不同, 转的方向有时可能会反过来, 所以这里先跟着unity走
        /// </summary>
        public static Vector2 Perpendicular(this Vector2 vector, bool reverse=false)
        {
            return reverse? new Vector2(-vector.y, vector.x) : new Vector2(vector.y, -vector.x);
        }

        public static Vector2 Floor(this Vector2 val)
        {
            return new Vector2((int)MathUtils.Floor(val.x), (int)MathUtils.Floor(val.y));
        }

    }
}