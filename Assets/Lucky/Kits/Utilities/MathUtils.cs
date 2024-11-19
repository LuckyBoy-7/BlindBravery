using System;
using UnityEngine;

namespace Lucky.Kits.Utilities
{
    public static class MathUtils
    {
        public const float SmallValue = 1E-5f;
        public const float Epsilon = 1.401298E-45f;
        public const float MaxValue = 3.40282347E+38f;
        public const float MinValue = -3.40282347E+38f;
        public const float NaN = 0.0f / 0.0f;
        public const float NegativeInfinity = -1.0f / 0.0f;
        public const float PositiveInfinity = 1.0f / 0.0f;

        public static Vector2 GreatVector2 => Vector2.one * MaxValue;


        public static float PI(float k = 1) => (float)Math.PI * k;

        public static float Max(float a, float b) => a > b ? a : b;
        public static int Max(int a, int b) => a > b ? a : b;
        public static float Min(float a, float b) => a < b ? a : b;
        public static int Min(int a, int b) => a < b ? a : b;
        public static float Abs(float x) => x > 0 ? x : -x;
        public static int Sign(float x) => x == 0 ? 0 : (x > 0 ? 1 : -1);
        public static int Sign2(float x) => x >= 0 ? 1 : -1;
        public static float Tan(float x) => (float)Math.Tan(x);

        public static float Floor(float x) => (float)Math.Floor(x);
        public static float Frac(float x) => x - Floor(x);
        public static float Trunc(float x) => (int)x;
        public static float Cos(float x) => (float)Math.Cos(x);
        public static float Sin(float x) => (float)Math.Sin(x);
        public static float RadiansToDegree(float x) => x * 360 / PI(2);
        public static float DegreeToRadians(float x) => x * PI(2) / 360;
        public static float Pow(float x, float y) => (float)Math.Pow(x, y);
        public static float Sqrt(float x) => (float)Math.Pow(x, 0.5f);

        /// 根据y, x返回对应弧度
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);

        public static Tuple<float, float> Divmod(float x, float y)
        {
            float remain = Mod(x, y);
            float quotient = (x - remain) / y;
            return new Tuple<float, float>(quotient, remain);
        }

        public static float VectorRadians(Vector2 vec)
        {
            float radians = Atan2(vec.y, vec.x);
            if (radians < 0)
                radians += PI(2);
            return radians;
        }

        public static float VectorAngle(Vector2 vec) => RadiansToDegree(VectorRadians(vec));

        public static Vector2 RadiansToVector(float angleRadians, float length)
        {
            return new Vector2(Cos(angleRadians), Sin(angleRadians)) * length;
        }

        public static Vector2 AngleToVector(float angle, float length) => RadiansToVector(DegreeToRadians(angle), length);

        /// 根据val在oldMin和oldMax之间的比例，放置到newMin和newMax之间
        public static float Map(float val, float min, float max, float newMin = 0f, float newMax = 1f)
        {
            return (val - min) / (max - min) * (newMax - newMin) + newMin;
        }

        /// Map后clamp
        public static float ClampedMap(float val, float min, float max, float newMin = 0f, float newMax = 1f)
        {
            return Clamp((val - min) / (max - min), 0f, 1f) * (newMax - newMin) + newMin;
        }

        // 向量往右转90度
        public static Vector2 TurnRight(this Vector2 vec)
        {
            return new Vector2(vec.y, -vec.x);
        }

        public static float Clamp(float value, float min, float max)
        {
            return Min(Max(value, min), max);
        }

        public static int Clamp(int value, int min, int max)
        {
            return Min(Max(value, min), max);
        }

        /// 相当于MoveTowards
        public static float Approach(float val, float target, float maxMove)
        {
            maxMove = Abs(maxMove);
            if (val <= target)
                return Min(val + maxMove, target);
            return Max(val - maxMove, target);
        }


        public static int Mod(int x, int m) => (x % m + m) % m;
        public static float Mod(float x, float m) => (x % m + m) % m;


        /// <summary>
        /// 把k的值clamp01了再lerp
        /// </summary>
        public static float LerpClamp(float value1, float value2, float k)
        {
            return Mathf.Lerp(value1, value2, Mathf.Clamp(k, 0f, 1f));
        }

        public static float Lerp(float value1, float value2, float k)
        {
            return Mathf.Lerp(value1, value2, k);
        }



        public static float AngleApproach(float val, float target, float maxMove)
        {
            float num = AngleDiff(val, target);
            if (Abs(num) < maxMove)
                return target;

            return val + Clamp(num, -maxMove, maxMove);
        }

        public static float AngleDiff(float radiansA, float radiansB)
        {
            float num = (radiansB - radiansA) % (Mathf.PI * 2);
            if (num > Mathf.PI)
                num -= Mathf.PI * 2;
            else if (num < -Mathf.PI)
                num += Mathf.PI * 2;
            return num;
        }


        /// <summary>
        /// 类似MoveTowards
        /// </summary>
        public static Vector2 Approach(Vector2 from, Vector2 to, float maxMove)
        {
            if (maxMove == 0f || from == to)
                return from;
            Vector2 delta = to - from;
            if (delta.magnitude < maxMove)
                return to;
            delta.Normalize();
            return from + delta * maxMove;
        }

        /// <summary>
        /// 当前位置是否要越过终点位置
        /// </summary>
        public static bool PassedTargetPoint(Vector2 from, Vector2 to, float maxMove) => Approach(from, to, maxMove) == to;

        /// <summary>
        /// 获取从from到to的弧度, 范围为[0, pi]
        /// </summary>
        public static float Radians(Vector2 from, Vector2 to)
        {
            // from * to / sqrt(from^2 + to^2) = cos<from, to>
            float x = (float)Math.Sqrt(from.sqrMagnitude * (double)to.sqrMagnitude);
            if (x < 1.0000000036274937E-15) // 应该是防止浮点误差
                return 0;
            return (float)Math.Acos(Mathf.Clamp(Vector2.Dot(from, to) / x, -1f, 1f));
        }

        public static float Angle(Vector2 from, Vector2 to) => RadiansToDegree(Radians(from, to));

        /// <summary>
        /// 获取从from到to的弧度, 范围为[-pi, pi]
        /// </summary>
        public static float SignedRadians(Vector2 from, Vector2 to)
        {
            return Radians(from, to) * Sign((float)(from.x * (double)to.y - from.y * (double)to.x));
            // 这里的from和to的顺序反过来应该是左手坐标系的问题, 所以也可以写成
            // return Vector2.Angle(from, to) * -Sign((float) (from.y * (double) to.x - from.x * (double) to.y));
        }

        public static float SignedAngle(Vector2 from, Vector2 to) => RadiansToDegree(SignedRadians(from, to));

    }
}