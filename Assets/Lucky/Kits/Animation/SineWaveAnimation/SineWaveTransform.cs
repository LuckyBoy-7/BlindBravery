using Lucky.Framework;
using Lucky.Kits;
using UnityEngine;

namespace Lucky.Kits.Animation
{
    public abstract class SineWaveTransform : ManagedBehaviour
    {
        public Vector3 magnitude = default; // 暴露给编辑器调整的
        public float speed = default;
        public float timeOffset = default; // sin初相

        private Vector3 originalMagnitude; // 初始值，相当于基准值

        private void Awake()
        {
            originalMagnitude = GetMagnitude();
        }

        protected override void ManagedFixedUpdate()
        {
            float sine = (Mathf.Sin(Time.time * speed + timeOffset) * 0.5f) + 0.5f; // [0, 1]
            float x = originalMagnitude.x + (sine * magnitude.x);
            float y = originalMagnitude.y + (sine * magnitude.y);
            float z = originalMagnitude.z + (sine * magnitude.z);
            ApplyTransformation(new Vector3(x, y, z));
        }

        protected abstract Vector3 GetMagnitude();
        protected abstract void ApplyTransformation(Vector3 value);
    }
}