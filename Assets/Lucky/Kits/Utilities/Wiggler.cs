using System;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits;
using UnityEngine;

namespace Lucky.Kits.Utilities
{
    /// <summary>
    /// 创造一个随时间正弦摆动的值
    /// 可以调存活时间和摆动频率，至于magnitude，它就是Counter，所以这个摆动幅度是越来越小的，力的损失（
    /// </summary>
    public class Wiggler : ManagedBehaviour
    {
        private float Magnitude { get; set; }
        public float Value { get; private set; }

        public bool UseRawDeltaTime;
        public bool StartZero;
        private float Duration = 0.3f;
        private float Frequency = 2;
        private bool RemoveSelfOnFinish = true;
        private float sinCounter;

        private float Increment => 1f / Duration;
        private float SinAdd => Mathf.PI * 2 * Frequency; // A * sin(w * x + f) + y，这里周期为1/frequency
        public Action<float> OnChange;

        private static List<Wiggler> cache = new();

        public static Wiggler Create(float duration, float frequency, Action<float> onChange = null, bool start = false, bool removeSelfOnFinish = false)
        {
            Wiggler wiggler;
            if (cache.Count > 0)
                wiggler = cache.Pop();
            else
                wiggler = new Wiggler();
            wiggler.Init(duration, frequency, onChange, start, removeSelfOnFinish);
            return wiggler;
        }

        private void Init(float duration, float frequency, Action<float> onChange, bool start = false, bool removeSelfOnFinish = true, bool isStartZero = true)
        {
            Duration = duration;
            Frequency = frequency;
            UseRawDeltaTime = false;
            Magnitude = sinCounter = 0f;
            OnChange = onChange;
            RemoveSelfOnFinish = removeSelfOnFinish;
            StartZero = isStartZero;
            if (start)
                Start();
        }

        public void Start()
        {
            Magnitude = 1f;
            if (StartZero)
            {
                sinCounter = 0;
                Value = 0f;
                OnChange?.Invoke(0f);
            }
            else
            {
                sinCounter = MathUtils.PI(0.5f);
                Value = 1f;
                OnChange?.Invoke(1f);
            }
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            // 现在对应的x
            float deltaTime = Timer.DeltaTime(UseRawDeltaTime);
            sinCounter += SinAdd * deltaTime;
            Magnitude -= Increment * deltaTime;

            if (Magnitude <= 0f)
            {
                Magnitude = 0f;
            }

            Value = (float)Math.Sin(sinCounter) * Magnitude;
            OnChange?.Invoke(Value);
        }
        // todo: do_wiggle
    }
}