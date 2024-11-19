using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using UnityEngine;

namespace Lucky.Kits.Animation
{
    public class Pulsater : ManagedBehaviour
    {
        public float amount = 0.1f;
        public float speed = 1f;

        private float k = -1f;

        private Vector3 origSize;

        private void Awake()
        {
            origSize = transform.localScale;
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (k >= 0f)
            {
                k = Mathf.MoveTowards(k, 1f, Time.deltaTime * speed);
                var stepped = Mathf.SmoothStep(0f, 1f, k);
                // [1, 2]
                var size = Mathf.Sin(Mathf.PI * stepped) * amount + 1f;
                transform.localScale = size * origSize;
            }
        }

        public void Pulsate()
        {
            k = 0f;
        }
    }
}