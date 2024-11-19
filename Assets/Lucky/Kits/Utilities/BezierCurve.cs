using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Kits.Utilities
{
    public struct BezierCurve
    {
        public Vector2 Begin;
        public Vector2 End;
        public Vector2 Control;

        public BezierCurve(Vector2 begin, Vector2 end, Vector2 control)
        {
            Begin = begin;
            End = end;
            Control = control;
        }

        public void DoubleControl()
        {
            // (Begin.x + (End.x - (double)Begin.x) / 2.0表示begin和end中点的向量
            // 感觉是把control向量翻倍然和和终点坐标blend了一下
            Control = new Vector2(
                (float)(Control.x + (double)Control.x - (Begin.x + (End.x - (double)Begin.x) / 2.0)),
                (float)(Control.y + (double)Control.y - (Begin.y + (End.y - (double)Begin.y) / 2.0))
            );
        }

        /// <summary>
        /// 获取路径上某个点的位置
        /// </summary>
        /// <param name="percent">点在路径位置中的"进度"</param>
        /// <returns></returns>
        public Vector2 GetPoint(float percent)
        {
            // 这里从易读性考虑把double float的转化都删了
            // 二阶贝塞尔曲线
            // B(t) = (1-t)**2 * P0 + 2t(1-t)P1 + t**2 *P2
            percent = MathUtils.Clamp(percent, 0, 1);
            float k = 1.0f - percent;
            return k * k * Begin + 2.0f * k * percent * Control + percent * percent * End;
        }

        /// <summary>
        /// 获取路径上的所有点
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public List<Vector2> GetPoints(int resolution = 100)
        {
            List<Vector2> res = new();
            for (int i = 0; i <= resolution; i++)
                res.Add(GetPoint((float)i / resolution));

            return res;
        }

        /// <summary>
        /// 获取曲线的长度
        /// </summary>
        /// <param name="resolution">一开始我还以为是什么分辨率呢，想想才知道是精度</param>
        /// <returns></returns>
        public float GetLength(int resolution)
        {
            Vector2 vector = Begin;
            float num = 0f;
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 point = GetPoint(i / (float)resolution);
                num += (point - vector).magnitude;
                vector = point;
            }

            return num;
        }
    }
}