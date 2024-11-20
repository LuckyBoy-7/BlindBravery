using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Managers.ObjectPool_;
using Lucky.Kits.Utilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BlindBravery
{
    public class SoundWave : ManagedBehaviour, IRecycle
    {
        private Light2D light;
        private float fadeInSpeed = 10f;
        private float fadeOutSpeed = 3f;
        private float waitTime = 1;

        private void Awake()
        {
            light = GetComponent<Light2D>();
        }

        public void OnGet()
        {
        }

        public void OnRelease()
        {
        }

        public void Do(float radius)
        {
            StartCoroutine(DoSoundWave(radius));
        }

        IEnumerator DoSoundWave(float radius)
        {
            light.pointLightOuterRadius = 0;
            while (light.pointLightOuterRadius != radius)
            {
                light.pointLightOuterRadius = MathUtils.Approach(light.pointLightOuterRadius, radius, fadeInSpeed * Timer.DeltaTime());
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);

            while (light.pointLightOuterRadius != 0)
            {
                light.pointLightOuterRadius = MathUtils.Approach(light.pointLightOuterRadius, 0, fadeOutSpeed * Timer.DeltaTime());
                yield return null;
            }
        }
    }
}