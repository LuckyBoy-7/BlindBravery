using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using MathUtils = Lucky.Kits.Utilities.MathUtils;
using Random = System.Random;

namespace Lucky.Kits.Generator.Noise
{
    /// <summary>
    /// 如果要实现更自然地柏林噪声, 那么可以像正弦函数那样叠加多个柏林噪声
    /// </summary>
    public class PerlinNoiseGenerator : NoiseGenerator
    {
        public bool useColor = false;
        protected override string FileName => "PerlinNoise.png";
        public int cellSize = 8;
        public int segments = 4;

        protected override Color GetPixelColor(int x, int y)
        {
            // Perlin p = new Perlin(cellSize);
            // return Color.white * (float)p.perlin((float)x / width * segments, (float)y / height * segments, 0);

            int bottomLeftX = x / cellSize;
            int bottomLeftY = y / cellSize;
            float fx = MathUtils.Frac((float)x / cellSize);
            float fy = MathUtils.Frac((float)y / cellSize);

            // 网格对应位置的随机梯度向量
            Vector2 bottomLeftG = MathUtils.AngleToVector(RandomUtils.RandomNoise(bottomLeftX, bottomLeftY) * 360, 1);
            Vector2 bottomRightG = MathUtils.AngleToVector(RandomUtils.RandomNoise(bottomLeftX + 1, bottomLeftY) * 360, 1);
            Vector2 topLeftG = MathUtils.AngleToVector(RandomUtils.RandomNoise(bottomLeftX, bottomLeftY + 1) * 360, 1);
            Vector2 topRightG = MathUtils.AngleToVector(RandomUtils.RandomNoise(bottomLeftX + 1, bottomLeftY + 1) * 360, 1);
            // 点乘后的结果
            float bottomLeftDot = Vector2.Dot(bottomLeftG, new Vector2(fx, fy));
            float bottomRightDot = Vector2.Dot(bottomRightG, new Vector2(fx, fy) - new Vector2(1, 0));
            float topLeftDot = Vector2.Dot(topLeftG, new Vector2(fx, fy) - new Vector2(0, 1));
            float topRightDot = Vector2.Dot(topRightG, new Vector2(fx, fy) - new Vector2(1, 1));
            // 拿到最lerp出来的值
            fx = Fade(fx);
            fy = Fade(fy);
            float bottomDot = MathUtils.Lerp(bottomLeftDot, bottomRightDot, fx);
            float topDot = MathUtils.Lerp(topLeftDot, topRightDot, fx);
            float dot = MathUtils.Lerp(bottomDot, topDot, fy);

            float ans = (dot + 1) / 2;
            if (useColor)
            {
                if (ans > 0.7f)
                    return Color.blue;
                if (ans > 0.6f)
                    return Color.green;
                if (ans > 0.5f)
                    return Color.red;
                if (ans > 0.4f)
                    return Color.cyan;
                if (ans > 0.3f)
                    return Color.magenta;
            }


            return (Color.white * ans).WithA(1);
        }

        protected static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10); // 6t^5 - 15t^4 + 10t^3
        }
    }
}