using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits;
using Lucky.Kits.Extensions;


namespace Lucky.Kits.Animation
{
    /// <summary>
    /// 设置抖动，就是随机一个位置然后向那个方向移动，固定频率更改一次目的地
    /// </summary>
    public class JitterPosition : ManagedBehaviour
    {
        public bool UnscaledTime { private get; set; }

        [SerializeField] private bool onlyX = default;

        [SerializeField] private float jitterSpeed = 10f;

        public float jitterFrequency = 20f;

        [SerializeField] private bool toFullExtent = false; // 随机值是否沿圆的轮廓

        public float amount = 0.2f;  // 抖动程度

        private Vector2 currentJitterValue;
        private Vector2 intendedJitterValue;

        private float jitterTimer;

        private Vector2 originalPos;


        void Start()
        {
            originalPos = transform.localPosition;
            this.CreateFuncTimer(SetNewIntendedValue, () => 1 / jitterFrequency, isScaledTime: !UnscaledTime);
        }

        protected override void ManagedFixedUpdate()
        {
            FloorSubpixelAmount();

            float timeStep = UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            currentJitterValue = Vector2.Lerp(currentJitterValue, intendedJitterValue, timeStep * jitterSpeed);
            ApplyJitter(currentJitterValue);
        }

        public void Stop()
        {
            enabled = false;
            transform.localPosition = originalPos;
        }

        private void FloorSubpixelAmount() // 抖动太小的话就不抖动了
        {
            if (amount < 0.01f)
            {
                amount = 0f;
            }
        }

        private void SetNewIntendedValue()
        {
            intendedJitterValue = toFullExtent ? Random.insideUnitCircle.normalized : Random.insideUnitCircle;
        }

        private void ApplyJitter(Vector2 jitterValue)
        {
            Vector3 pos = originalPos;
            pos.x += jitterValue.x * amount;
            if (!onlyX)
                pos.y += jitterValue.y * amount;
            transform.localPosition = pos;
        }
    }
}