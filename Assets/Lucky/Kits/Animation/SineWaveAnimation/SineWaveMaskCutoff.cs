using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits;
using Lucky.Kits.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Lucky.Kits.Animation
{
    public class SineWaveMaskCutoff : ManagedBehaviour
    {
        [SerializeField] private SpriteMask mask = default;

        [SerializeField] private float speed = default;

        [SerializeField] private float minValue = default;

        [SerializeField] private float maxValue = default;

        float offset = default;

        private void Awake()
        {
            offset = RandomUtils.NextRadians(); // 在一个周期内随便选个点
        }

        protected override void ManagedFixedUpdate()
        {
            float sine = (Mathf.Sin(Time.time * speed + offset) * 0.5f) + 0.5f; // [0, 1]
            mask.alphaCutoff = Mathf.Lerp(minValue, maxValue, sine);
        }
    }
}
