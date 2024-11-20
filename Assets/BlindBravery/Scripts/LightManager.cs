using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Managers;
using Lucky.Kits.Managers.ObjectPool_;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BlindBravery
{

    public class LightManager : Singleton<LightManager>
    {
        private SoundWave lightPrefab;

        protected override void Awake()
        {
            base.Awake();
            lightPrefab = Resources.Load<SoundWave>("Prefabs/SoundWave");
        }

        public void CreateLight(Vector2 pos, float radius = 8)
        {
            var sound = ObjectPoolManager.Instance.Get<SoundWave>();
            sound.transform.position = pos;
            sound.Do(radius);
        }
    }
}