using System;
using System.Collections.Generic;

namespace Lucky.Kits.Utilities
{
    /// <summary>
    /// 现在有一系列连续的时间段, 有各种类型的值会在这些段之间Lerp, 而Segments就是管理这些的抽象
    /// </summary>
    public class SerialSegments<T>
    {
        private List<Tuple<float, T>> pairs;
        private List<T> values;
        private Func<T, T, float, T> ease; // T1, T2, k, ans

        public SerialSegments(List<Tuple<float, T>> pairs, Func<T, T, float, T> ease)
        {
            this.pairs = pairs;
            this.ease = ease;
        }

        public SerialSegments(List<T> values, Func<T, T, float, T> ease)
        {
            this.values = values;
            this.ease = ease;
        }

        /// <summary>
        /// 如果只是单次用, 或者说该类很分散, 那么一个类记录pair比较方便
        /// </summary>
        public T Get(float x)
        {
            x = MathUtils.Clamp(x, pairs[0].Item1, pairs[^1].Item1);
            int right = Itertools.BisectLeft(pairs, x, tuple => tuple.Item1);
            if (right == 0) // 说明x刚好和第一项的值一样(最极端的情况)
                right = 1;
            int left = right - 1;

            var (leftTime, leftVal) = pairs[left];
            var (rightTime, rightVal) = pairs[right];
            return ease(leftVal, rightVal, (x - leftTime) / (rightTime - leftTime));
        }

        /// <summary>
        /// 如果多次用, 且很多类都共用一个时间段, 那么只记录值, 用传进来的list索引比较方便
        /// </summary>
        public T GetByExistTimes(List<float> times, float x)
        {
            x = MathUtils.Clamp(x, times[0], times[^1]);
            int right = Itertools.BisectLeft(times, x);
            if (right == 0) // 说明x刚好和第一项的值一样(最极端的情况)
                right = 1;
            int left = right - 1;

            var (leftTime, leftVal) = (times[left], values[left]);
            var (rightTime, rightVal) = (times[right], values[right]);
            return ease(leftVal, rightVal, (x - leftTime) / (rightTime - leftTime));
        }
    }
}