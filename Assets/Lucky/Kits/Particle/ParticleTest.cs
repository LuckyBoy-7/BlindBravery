using System;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Lucky.Kits.Particle
{
    public class ParticleTest : ManagedBehaviour
    {
        public string particleName = "Test";

        public int amount = 15;
        [Range(0, 60)] public float rangeX = 20;
        [Range(0, 60)] public float rangeY = 20;


        public bool isLoop;
        [ShowIf("isLoop")] public float loopDuration;
        [ShowIf("isLoop")] public float loopTimer;
        private bool hasStart;


        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (!isLoop)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    ParticleSystem.Instance.Emit<Particle>(particleName, this, amount, Vector2.zero, new Vector2(rangeX, rangeY));
                hasStart = false;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    hasStart = !hasStart;
                    if (hasStart)
                        loopTimer = loopDuration;
                }

                if (hasStart)
                {
                    loopTimer -= Timer.FixedDeltaTime();
                    if (loopTimer <= 0)
                    {
                        loopTimer = loopDuration;
                        ParticleSystem.Instance.Emit<Particle>(particleName, this, amount, Vector2.zero, new Vector2(rangeX, rangeY));
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            ParticleData data = ParticleSystem.Instance[particleName];

            // 生成区域
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(rangeX, rangeY) * 2);
            // 运动方向
            Gizmos.color = Color.red;
            float length = 8;
            Vector2 startDir = MathUtils.RadiansToVector(data.direction - data.directionRange / 2, length);
            int resolution = 18;
            for (int i = 0; i < resolution; i++)
            {
                Gizmos.DrawRay(transform.position, startDir.Rotate(data.directionRange / resolution * i));
            }
        }
    }
}