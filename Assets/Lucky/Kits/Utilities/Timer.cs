using UnityEngine;

namespace Lucky.Kits.Utilities
{
    public static class Timer
    {
        public static float GetTime(bool realtime = false) => realtime ? Time.realtimeSinceStartup : Time.time;
        public static float DeltaTime(bool realtime = false) => realtime ? Time.unscaledDeltaTime : Time.deltaTime;
        public static float FixedDeltaTime(bool realtime = false) => realtime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;

        /// 每一个interval开始的时候触发一次, 相当于 true false false false ... true false false false ... 
        public static bool OnInterval(float interval, bool realtime = false)
        {
            return (int)((GetTime(realtime) - (double)FixedDeltaTime(realtime)) / interval) < (int)((double)GetTime(realtime) / interval);
        }

        /// 获取按interval交替的bool, 相当于 true true true ... false false false ... true true true
        public static bool BetweenInterval(float interval, bool realtime = false)
        {
            return GetTime(realtime) % (interval * 2f) > interval;
        }

        /// 同上, 只不过这里时间流逝是跟着val的 
        public static bool BetweenInterval(float val, float interval)
        {
            return val % (interval * 2f) > interval;
        }
    }
}